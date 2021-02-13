using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedStat.Core.BE.Device;
using MedStat.Core.DAL;
using MedStat.Core.Helpers;
using MedStat.Core.Info;
using MedStat.Core.Info.Company;
using MedStat.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedStat.Core.Repositories
{
	public partial class DeviceRepository : BaseRepository, IDeviceRepository
	{
		public DeviceRepository(MedStatDbContext dbContext, 
			ILogger<CompanyRepository> logger, string userUid)
			: base(dbContext, logger, userUid)
		{
		}


		#region Create

		public async Task<int> CreateDeviceAsync(string deviceModelUid,
			string inventoryNumber, string wifiMac, string ethernetMac)
		{
			#region Validation

			if (string.IsNullOrEmpty(deviceModelUid))
				throw new ArgumentNullException(nameof(deviceModelUid));

			if (string.IsNullOrEmpty(inventoryNumber))
				throw new ArgumentNullException(nameof(inventoryNumber));

			await this.CheckForUniqueInventoryNumberAsync(inventoryNumber);
			await this.CheckForUniqueMacAddressAsync(wifiMac);
			await this.CheckForUniqueMacAddressAsync(ethernetMac);

			#endregion

			DeviceModel model = this.DeviceModels.FirstOrDefault(dv => dv.Uid == deviceModelUid);
			if (model == null)
			{
				throw new OperationCanceledException(string.Format(
					this.MessagesManager.GetString("Device Model with UID = {0} is not found"),
						deviceModelUid));
			}

			try
			{
				var newDevice = new Device
				{
					InventoryNumber = inventoryNumber.Trim(),

					NormalizedEthernetMac = !string.IsNullOrEmpty(ethernetMac)
						? DeviceManager.NormalizeMacAddress(ethernetMac)
						: null,
					NormalizedWifiMac = !string.IsNullOrEmpty(wifiMac) 
						? DeviceManager.NormalizeMacAddress(wifiMac) 
						: null,

					CreatedUtc = DateTime.UtcNow,

					DeviceModelUid = model.Uid,
					CompanyId = null
				};

				this.DbContext.Devices.Add(newDevice);
				await this.DbContext.SaveChangesAsync();

				this.Logger.LogInformation("Device {@Device} was created successfully by {UserUid}",
					new { newDevice.Id, newDevice.InventoryNumber }, this.UserUid);

				return newDevice.Id;
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "Device creation was failed. {@params}",
					new { deviceModelUid, inventoryNumber, wifiMac, ethernetMac });
				throw;
			}
		}

		#endregion


		protected async Task CheckForUniqueInventoryNumberAsync(string inventoryNumber)
		{
			string normalizedInvNumber = inventoryNumber.Trim().ToUpper();

			bool isInvNumberExist = await this.DbContext.Devices
				.AnyAsync(d => d.InventoryNumber.ToUpper() == normalizedInvNumber);

			if (isInvNumberExist)
			{
				throw new OperationCanceledException(string.Format(
					this.MessagesManager.GetString("Device with inventory number {0} is already registered in the system"),
					normalizedInvNumber));
			}
		}

		protected async Task CheckForUniqueMacAddressAsync(string macAddress)
		{
			if(string.IsNullOrEmpty(macAddress))
				return;

			string normalizedMac = DeviceManager.NormalizeMacAddress(macAddress);

			bool isMacAddressExist = await this.DbContext.Devices
				.AnyAsync(d => d.NormalizedWifiMac == normalizedMac || d.NormalizedEthernetMac == normalizedMac);

			if (isMacAddressExist)
			{
				throw new OperationCanceledException(string.Format(
					this.MessagesManager.GetString("MAC address {0} is already registered in the system"),
					normalizedMac));
			}
		}
	}
}
