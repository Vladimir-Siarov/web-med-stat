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


		#region Get

		public async Task<Device> GetDeviceAsync(int deviceId)
		{
			Device device = await this.DbContext.Devices
				//.Include(d => d.Model)
				.AsNoTracking()
				.FirstOrDefaultAsync(d => d.Id == deviceId);

			if (device != null)
			{
				device.Model = this.DeviceModels.First(m => m.Uid == device.DeviceModelUid);
			}

			return device;
		}

		#endregion


		#region Create / Update

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

		public async Task UpdateDeviceAsync(int deviceId,
			string deviceModelUid, string inventoryNumber, string wifiMac, string ethernetMac)
		{
			#region Validation

			if (string.IsNullOrEmpty(deviceModelUid))
				throw new ArgumentNullException(nameof(deviceModelUid));

			if (string.IsNullOrEmpty(inventoryNumber))
				throw new ArgumentNullException(nameof(inventoryNumber));

			await this.CheckForUniqueInventoryNumberAsync(inventoryNumber, deviceId);
			await this.CheckForUniqueMacAddressAsync(wifiMac, deviceId);
			await this.CheckForUniqueMacAddressAsync(ethernetMac, deviceId);

			#endregion

			Device dbDevice = this.DbContext.Devices.FirstOrDefault(d => d.Id == deviceId);
			if (dbDevice == null)
			{
				throw new OperationCanceledException(string.Format(
					this.MessagesManager.GetString("Device with ID = {0} is not found"),
					deviceId));
			}

			DeviceModel model = this.DeviceModels.FirstOrDefault(dv => dv.Uid == deviceModelUid);
			if (model == null)
			{
				throw new OperationCanceledException(string.Format(
					this.MessagesManager.GetString("Device Model with UID = {0} is not found"),
					deviceModelUid));
			}

			try
			{
				dbDevice.InventoryNumber = inventoryNumber.Trim();
				dbDevice.DeviceModelUid = model.Uid;

				dbDevice.NormalizedEthernetMac = !string.IsNullOrEmpty(ethernetMac)
					? DeviceManager.NormalizeMacAddress(ethernetMac)
					: null;
				dbDevice.NormalizedWifiMac = !string.IsNullOrEmpty(wifiMac)
					? DeviceManager.NormalizeMacAddress(wifiMac)
					: null;
				
				await this.DbContext.SaveChangesAsync();

				this.Logger.LogInformation("Device {@Device} was updated by {UserUid}",
					new { dbDevice.Id, dbDevice.InventoryNumber }, this.UserUid);
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "Device update action was failed. {@params}",
					new { deviceId, deviceModelUid, inventoryNumber, wifiMac, ethernetMac });
				throw;
			}
		}

		#endregion


		protected async Task CheckForUniqueInventoryNumberAsync(string inventoryNumber, int? exceptId = null)
		{
			string normalizedInvNumber = inventoryNumber.Trim().ToUpper();

			bool isInvNumberExist = exceptId.HasValue
				? await this.DbContext.Devices
						.AnyAsync(d => d.Id != exceptId && d.InventoryNumber.ToUpper() == normalizedInvNumber)
				: await this.DbContext.Devices
						.AnyAsync(d => d.InventoryNumber.ToUpper() == normalizedInvNumber);

			if (isInvNumberExist)
			{
				throw new OperationCanceledException(string.Format(
					this.MessagesManager.GetString("Device with inventory number {0} is already registered in the system"),
					normalizedInvNumber));
			}
		}

		protected async Task CheckForUniqueMacAddressAsync(string macAddress, int? exceptId = null)
		{
			if(string.IsNullOrEmpty(macAddress))
				return;

			string normalizedMac = DeviceManager.NormalizeMacAddress(macAddress);

			bool isMacAddressExist = exceptId.HasValue
				? await this.DbContext.Devices
						.AnyAsync(d => d.Id != exceptId 
						               && (d.NormalizedWifiMac == normalizedMac || d.NormalizedEthernetMac == normalizedMac))
				: await this.DbContext.Devices
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
