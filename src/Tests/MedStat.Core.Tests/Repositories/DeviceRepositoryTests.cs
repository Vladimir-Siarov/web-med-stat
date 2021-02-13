using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using MedStat.Core.BE.Device;
using MedStat.Core.DAL;
using MedStat.Core.Helpers;
using MedStat.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace MedStat.Core.Tests.Repositories
{
	public class DeviceRepositoryTests : BaseRepositoryTests, IClassFixture<DatabaseFixture>
	{
		public DeviceRepositoryTests(ITestOutputHelper outputHelper, DatabaseFixture fixture)
			: base(outputHelper, fixture)
		{
		}


		#region Create / Update / Delete

		[Theory]
		[MemberData(nameof(CreateDeviceAsync_Data))]
		public async void CreateDeviceAsync(string deviceModelUid,
			string inventoryNumber, string wifiMac, string ethernetMac,
			Type expectedExceptionType)
		{
			// Arrange:

			
			// Act:

			int? deviceId = null;
			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var dvRepository = sp.GetRequiredService<IDeviceRepository>();

				if (expectedExceptionType != null)
				{
					// act + assert
					await Assert.ThrowsAsync(expectedExceptionType, () => dvRepository.CreateDeviceAsync(deviceModelUid,
						inventoryNumber, wifiMac, ethernetMac));

					return;
				}
				else
				{
					deviceId = dvRepository.CreateDeviceAsync(deviceModelUid,
						inventoryNumber, wifiMac, ethernetMac).Result;
				}
			}


			// Assert:

			deviceId.Should().NotBeNull();

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var device = dbContext.Devices
					//.Include(d => d.Model)
					.FirstOrDefault(d => d.Id == deviceId.Value);

				device.Should().NotBeNull();

				device.InventoryNumber.Should().BeEquivalentTo(inventoryNumber);
				device.CreatedUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

				device.DeviceModelUid.Should().NotBeEmpty();
				device.DeviceModelUid.ToUpper().Should().BeEquivalentTo(deviceModelUid.ToUpper());

				if (!string.IsNullOrEmpty(wifiMac))
				{
					device.NormalizedWifiMac.Should().BeEquivalentTo(DeviceManager.NormalizeMacAddress(wifiMac));
				}
				if (!string.IsNullOrEmpty(ethernetMac))
				{
					device.NormalizedEthernetMac.Should().BeEquivalentTo(DeviceManager.NormalizeMacAddress(ethernetMac));
				}
			}
		}

		[Fact]
		public async void CreateDeviceAsync_NonUniqueValues()
		{
			// Arrange:

			var watch = this.AddNewDeviceToDb(EnDeviceType.SmartWatch);

			var nonUniqueInvNumber = DataHelper.GetDeviceData(EnDeviceType.Gateway);
			nonUniqueInvNumber.InventoryNumber = watch.InventoryNumber;

			var nonUniqueWifiMac = DataHelper.GetDeviceData(EnDeviceType.SmartWatch);
			nonUniqueWifiMac.WifiMac = watch.NormalizedWifiMac;

			var nonUniqueEthernetMac = DataHelper.GetDeviceData(EnDeviceType.Gateway);
			nonUniqueEthernetMac.EthernetMac = watch.NormalizedWifiMac;


			// Act + Assert:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var dvRepository = sp.GetRequiredService<IDeviceRepository>();

				// 1) Non unique Inventory number
				await Assert.ThrowsAsync<OperationCanceledException>(() => dvRepository
					.CreateDeviceAsync(nonUniqueInvNumber.DeviceModelUid, nonUniqueInvNumber.InventoryNumber,
						nonUniqueInvNumber.WifiMac, nonUniqueInvNumber.EthernetMac));

				// 2) Non unique WiFi MAC address
				await Assert.ThrowsAsync<OperationCanceledException>(() => dvRepository
					.CreateDeviceAsync(nonUniqueWifiMac.DeviceModelUid, nonUniqueWifiMac.InventoryNumber,
						nonUniqueWifiMac.WifiMac, null));

				// 3) Non unique Ethernet MAC address
				await Assert.ThrowsAsync<OperationCanceledException>(() => dvRepository
					.CreateDeviceAsync(nonUniqueEthernetMac.DeviceModelUid, nonUniqueEthernetMac.InventoryNumber,
						nonUniqueEthernetMac.WifiMac, nonUniqueEthernetMac.EthernetMac));
			}
		}

		#endregion


		public static IEnumerable<object[]> CreateDeviceAsync_Data
		{
			get
			{
				var gatewayDevice1 = DataHelper.GetDeviceData(EnDeviceType.Gateway);
				var watchDevice1 = DataHelper.GetDeviceData(EnDeviceType.SmartWatch);
				var watchDevice2 = DataHelper.GetDeviceData(EnDeviceType.SmartWatch);

				return
					new List<object[]>
					{
						// deviceModelUid,
						// inventoryNumber, wifiMac, ethernetMac,
						// expectedExceptionType

						// Valid data:

						new object[] { gatewayDevice1.DeviceModelUid, 
							gatewayDevice1.InventoryNumber, gatewayDevice1.WifiMac, gatewayDevice1.EthernetMac,
							null },
						new object[] { watchDevice1.DeviceModelUid, 
							watchDevice1.InventoryNumber, watchDevice1.NormalizedWifiMac, null,
							null },


						// Invalid data:

						// Nullable arguments
						new object[] { string.Empty,
							gatewayDevice1.InventoryNumber, gatewayDevice1.WifiMac, gatewayDevice1.EthernetMac,
							typeof(ArgumentNullException)},
						new object[] { gatewayDevice1.DeviceModelUid,
							null, gatewayDevice1.WifiMac, gatewayDevice1.EthernetMac,
							typeof(ArgumentNullException) },

						// Invalid arguments
						new object[] { "Un-existed model UID",
							watchDevice2.InventoryNumber, watchDevice2.NormalizedWifiMac, null,
							typeof(OperationCanceledException) }
					};
			}
		}


		// Helpers:

		private Device AddNewDeviceToDb(EnDeviceType type)
		{
			using (var context = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var device = DataHelper.GetDeviceData(type);

				context.Devices.Add(device);
				context.SaveChanges();

				return device;
			}
		}
	}
}
