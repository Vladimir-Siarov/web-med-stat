using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MedStat.Core.BE.Company;
using MedStat.Core.BE.Device;
using MedStat.Core.Helpers;
using MedStat.Core.Identity;

namespace MedStat.Core.Tests.Repositories
{
	internal static class DataHelper
	{
		private static long _initialPhoneNumValue = 1111111;
		private static int _initialInventoryNumValue = 1111;
		private static int _initialMacNumValue = 1111;


		// Company

		public static Company GetCompanyData()
		{
			var guid = Guid.NewGuid();
			var companyData = new Company
			{
				Name = guid.ToString(),
				Description = $"{guid} description",
				CreatedUtc = DateTime.UtcNow.AddDays(-1),
				UpdatedUtc = DateTime.UtcNow.AddDays(-1)
			};

			return companyData;
		}

		public static CompanyRequisites GetCompanyRequisitesFullData()
		{
			return
				new CompanyRequisites
				{
					UpdatedUtc = DateTime.UtcNow.AddDays(-1),

					MainRequisites = new CompanyMainRequisites
					{
						Name = "test_Name",
						FullName = "test_FullName",

						PostalAddress = "test_PostalAddress",
						LegalAddress = "test_LegalAddress",

						OKATO = "test_OKATO",
						INN = "test_INN",
						OGRN = "test_OGRN",
						OKPO = "test_OKPO",
						KPP = "test_KPP"
					},

					BankRequisites = new CompanyBankRequisites
					{
						AccountNumber = "test_AccountNumber",
						CorrespondentAccount = "test_CorrespondentAccount",
						BIC = "test_BIC",
						Bank = "test_Bank"
					}
				};
		}


		// CompanyUser

		public static CompanyUser GetCompanyUserData(bool includeLoginData)
		{
			var cmpUserData = new CompanyUser
			{
				Description = $"{Guid.NewGuid()} description",
				Login = includeLoginData
					? GetSystemUserNewData()
					: null
			};

			return cmpUserData;
		}


		// SystemUser

		public static SystemUser GetSystemUserNewData()
		{
			_initialPhoneNumValue += 1;

			var phoneNumber = $"+7 911 {_initialPhoneNumValue:###-##-##}";
			var normalizedPhoneNumber = PhoneHelper.NormalizePhoneNumber(phoneNumber);
			var guid = Guid.NewGuid();

			var user = new SystemUser
			{
				UserName = normalizedPhoneNumber,
				//NormalizedUserName = normalizedPhoneNumber,
				PhoneNumber = phoneNumber,
				NormalizedPhoneNumber = normalizedPhoneNumber,
				FirstName = $"FirstName {guid}",
				Surname = $"Surname {guid}",
				Patronymic = $"Patronymic {guid}"
			};

			return user;
		}

		public static string GenerateUserPassword()
		{
			return
				$"{Guid.NewGuid().ToString("N")}_A";
		}


		// Device

		public static DeviceData GetDeviceData(EnDeviceType type)
		{
			_initialInventoryNumValue += 1;

			var invNumber = $"{_initialInventoryNumValue:####}";
			var wifiMacAddress = $"AA-00-00-00-{_initialInventoryNumValue:##-##}";
			var ethernetMacAddress = type == EnDeviceType.Gateway
				? wifiMacAddress.Replace("AA", "BB")
				: null;
			
			var device = new DeviceData
			{
				InventoryNumber = invNumber,

				WifiMac = wifiMacAddress,
				EthernetMac = ethernetMacAddress,
				NormalizedWifiMac = DeviceManager.NormalizeMacAddress(wifiMacAddress),
				NormalizedEthernetMac = DeviceManager.NormalizeMacAddress(ethernetMacAddress),

				CreatedUtc = DateTime.UtcNow.AddDays(-1),

				DeviceModelUid = DeviceManager.GetDeviceModels().FirstOrDefault(m => m.Type == type)?.Uid
			};

			return device;
		}



		public class DeviceData : Device
		{
			public string WifiMac { get; set; }
			public string EthernetMac { get; set; }
		}
	}
}
