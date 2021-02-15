using System;
using System.Collections.Generic;
using System.Linq;
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
				else
				{
					device.NormalizedWifiMac.Should().BeNullOrWhiteSpace();
				}

				if (!string.IsNullOrEmpty(ethernetMac))
				{
					device.NormalizedEthernetMac.Should().BeEquivalentTo(DeviceManager.NormalizeMacAddress(ethernetMac));
				}
				else
				{
					device.NormalizedEthernetMac.Should().BeNullOrWhiteSpace();
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
			nonUniqueWifiMac.WifiMac = watch.WifiMac;

			var nonUniqueEthernetMac = DataHelper.GetDeviceData(EnDeviceType.Gateway);
			nonUniqueEthernetMac.EthernetMac = watch.WifiMac;


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


		[Theory]
		[MemberData(nameof(UpdateDeviceAsync_Data))]
		public async void UpdateDeviceAsync(Device existedDevice,
			string deviceModelUid, string inventoryNumber, string wifiMac, string ethernetMac,
			Type expectedExceptionType)
		{
			// Arrange:

			int deviceId = -1;

			if (existedDevice != null)
			{
				deviceId = this.AddDeviceToDb(existedDevice);
			}


			// Act:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var dvRepository = sp.GetRequiredService<IDeviceRepository>();

				if (expectedExceptionType != null)
				{
					// act + assert
					await Assert.ThrowsAsync(expectedExceptionType, () => dvRepository.UpdateDeviceAsync(deviceId,
						deviceModelUid, inventoryNumber, wifiMac, ethernetMac));

					return;
				}
				else
				{
					await dvRepository.UpdateDeviceAsync(deviceId, 
						deviceModelUid, inventoryNumber, wifiMac, ethernetMac);
				}
			}


			// Assert:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var device = dbContext.Devices
					//.Include(d => d.Model)
					.FirstOrDefault(d => d.Id == deviceId);

				device.Should().NotBeNull();

				device.InventoryNumber.Should().BeEquivalentTo(inventoryNumber);
				
				device.DeviceModelUid.Should().NotBeEmpty();
				device.DeviceModelUid.ToUpper().Should().BeEquivalentTo(deviceModelUid.ToUpper());

				if (!string.IsNullOrEmpty(wifiMac))
				{
					device.NormalizedWifiMac.Should().BeEquivalentTo(DeviceManager.NormalizeMacAddress(wifiMac));
				}
				else
				{
					device.NormalizedWifiMac.Should().BeNullOrWhiteSpace();
				}

				if (!string.IsNullOrEmpty(ethernetMac))
				{
					device.NormalizedEthernetMac.Should().BeEquivalentTo(DeviceManager.NormalizeMacAddress(ethernetMac));
				}
				else
				{
					device.NormalizedEthernetMac.Should().BeNullOrWhiteSpace();
				}
			}
		}

		[Fact]
		public async void UpdateDeviceAsync_NonUniqueValues()
		{
			// Arrange:

			var watch = this.AddNewDeviceToDb(EnDeviceType.Gateway);
			var gataway = this.AddNewDeviceToDb(EnDeviceType.Gateway);

			var nonUniqueInvNumber = DataHelper.GetDeviceData(EnDeviceType.Gateway);
			nonUniqueInvNumber.InventoryNumber = watch.InventoryNumber;

			var nonUniqueWifiMac = DataHelper.GetDeviceData(EnDeviceType.SmartWatch);
			nonUniqueWifiMac.WifiMac = watch.WifiMac;

			var nonUniqueEthernetMac = DataHelper.GetDeviceData(EnDeviceType.Gateway);
			nonUniqueEthernetMac.EthernetMac = watch.WifiMac;


			// Act + Assert:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var dvRepository = sp.GetRequiredService<IDeviceRepository>();

				// 1) Non unique Inventory number
				await Assert.ThrowsAsync<OperationCanceledException>(() => dvRepository
					.UpdateDeviceAsync(gataway.Id, 
						nonUniqueInvNumber.DeviceModelUid, nonUniqueInvNumber.InventoryNumber,
						nonUniqueInvNumber.WifiMac, nonUniqueInvNumber.EthernetMac));

				// 2) Non unique WiFi MAC address
				await Assert.ThrowsAsync<OperationCanceledException>(() => dvRepository
					.UpdateDeviceAsync(gataway.Id, 
						nonUniqueWifiMac.DeviceModelUid, nonUniqueWifiMac.InventoryNumber,
						nonUniqueWifiMac.WifiMac, null));

				// 3) Non unique Ethernet MAC address
				await Assert.ThrowsAsync<OperationCanceledException>(() => dvRepository
					.UpdateDeviceAsync(gataway.Id, 
						nonUniqueEthernetMac.DeviceModelUid, nonUniqueEthernetMac.InventoryNumber,
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

		public static IEnumerable<object[]> UpdateDeviceAsync_Data
		{
			get
			{
				var gateway1 = DataHelper.GetDeviceData(EnDeviceType.Gateway);
				var gateway2 = DataHelper.GetDeviceData(EnDeviceType.Gateway);
				var gateway3 = DataHelper.GetDeviceData(EnDeviceType.Gateway);
				var gateway4 = DataHelper.GetDeviceData(EnDeviceType.Gateway);
				var gateway5 = DataHelper.GetDeviceData(EnDeviceType.Gateway);

				var watch1 = DataHelper.GetDeviceData(EnDeviceType.SmartWatch);
				var watch2 = DataHelper.GetDeviceData(EnDeviceType.SmartWatch);

				return
					new List<object[]>
					{
						// existedDevice,
						// deviceModelUid, inventoryNumber, wifiMac, ethernetMac,
						// expectedExceptionType

						// Valid data:

						// update with full new data
						new object[] { gateway1,
							gateway2.DeviceModelUid, gateway2.InventoryNumber, gateway2.WifiMac, gateway2.EthernetMac,
							null },
						// update with the same data
						new object[] { gateway3,
							gateway3.DeviceModelUid, gateway3.InventoryNumber, gateway3.WifiMac, gateway3.EthernetMac,
							null },
						// change device type
						new object[] { gateway4,
							watch1.DeviceModelUid, watch1.InventoryNumber, watch1.WifiMac, null,
							null },


						// Invalid data:

						// Nullable arguments
						new object[] { DataHelper.GetDeviceData(EnDeviceType.Gateway),
							string.Empty, gateway5.InventoryNumber, gateway5.WifiMac, gateway5.EthernetMac,
							typeof(ArgumentNullException)},
						new object[] { DataHelper.GetDeviceData(EnDeviceType.Gateway),
							gateway5.DeviceModelUid, null, gateway5.WifiMac, gateway5.EthernetMac,
							typeof(ArgumentNullException) },

						// Invalid arguments
						new object[] {  DataHelper.GetDeviceData(EnDeviceType.SmartWatch),
							"Un-existed model UID", watch2.InventoryNumber, watch2.NormalizedWifiMac, null,
							typeof(OperationCanceledException) }
					};
			}
		}


		// Helpers:

		private DataHelper.DeviceData AddNewDeviceToDb(EnDeviceType type)
		{
			using (var context = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var device = DataHelper.GetDeviceData(type);

				context.Devices.Add(device);
				context.SaveChanges();

				return device;
			}
		}

		private int AddDeviceToDb(Device device)
		{
			using (var context = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				context.Devices.Add(device);
				context.SaveChanges();

				return device.Id;
			}
		}
	}
}
