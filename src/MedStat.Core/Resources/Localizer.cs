using System;
using System.Collections.Generic;
using System.Reflection;
using MedStat.Core.BE.Company;
using MedStat.Core.Identity;
using MedStat.Core.Info.Company;
using Microsoft.Extensions.Localization;

namespace MedStat.Core.Resources
{
	public static class Localizer
	{
		// !!! NOTE: Не забудь убрать символы '\' при копировании в файл ресурса !!!
		public struct DataAnnotations
		{
			public const string RequiredErrorMessage = "The \"{0}\" field is required.";
			public const string InvalidPhoneNumber = "The \"{0}\" field is not a valid phone number.";
		}
		

		private static IStringLocalizer _companyLocalizer;
		private static readonly Func<IStringLocalizerFactory, IStringLocalizer> GetCompanyLocalizer
			= (factory) =>
			{
				if (_companyLocalizer == null)
				{
					_companyLocalizer = factory.Create("BE.Company",
						typeof(Company).GetTypeInfo().Assembly.FullName);
				}

				return _companyLocalizer;
			};

		private static IStringLocalizer _systemUserLocalizer;
		private static readonly Func<IStringLocalizerFactory, IStringLocalizer> GetSystemUserLocalizer
			= (factory) =>
			{
				if (_systemUserLocalizer == null)
				{
					_systemUserLocalizer = factory.Create("Identity.SystemUser",
						typeof(SystemUser).GetTypeInfo().Assembly.FullName);
				}

				return _systemUserLocalizer;
			};

		private static Dictionary<string, Func<IStringLocalizerFactory, IStringLocalizer>> _localizerDictionary
			= new Dictionary<string, Func<IStringLocalizerFactory, IStringLocalizer>>
			{
				{ typeof(Company).FullName, GetCompanyLocalizer },
				{ typeof(CompanyMainRequisites).FullName, GetCompanyLocalizer },
				{ typeof(CompanyBankRequisites).FullName, GetCompanyLocalizer },
				{ typeof(CompanyUser).FullName, GetCompanyLocalizer },
				{ typeof(CompanyUserInfo).FullName, GetCompanyLocalizer },

				{ typeof(SystemUser).FullName, GetSystemUserLocalizer }
			};


		public static Func<Type, IStringLocalizerFactory, IStringLocalizer> GetDataAnnotationLocalizer
		 = (type, factory) =>
		 {
			 string typeName = type.FullName ?? type.Name;

			 if (_localizerDictionary.ContainsKey(typeName))
			 {
				 return _localizerDictionary[typeName](factory);
			 }

			 return null;
		 };
	}
}
