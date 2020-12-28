using System;
using System.Collections.Generic;
using System.Reflection;
using MedStat.Core.BE.Company;
using MedStat.Core.Identity;
using MedStat.Core.Info.Company;
using MedStat.WebAdmin.Pages.Account;
using Microsoft.Extensions.Localization;

namespace MedStat.WebAdmin.Classes
{
	public static class DataAnnotationsLocalizer
	{
		private static IStringLocalizer _companyLocalizer;
		private static readonly Func<IStringLocalizerFactory, IStringLocalizer> GetCompanyLocalizer
			= (factory) =>
			{
				if (_companyLocalizer == null)
				{
					_companyLocalizer = factory.Create("DataAnnotations.Company",
						typeof(DataAnnotationsLocalizer).GetTypeInfo().Assembly.FullName);
				}

				return _companyLocalizer;
			};

		private static IStringLocalizer _identityLocalizer;
		private static readonly Func<IStringLocalizerFactory, IStringLocalizer> GetIdentityUserLocalizer
			= (factory) =>
			{
				if (_identityLocalizer == null)
				{
					_identityLocalizer = factory.Create("DataAnnotations.Identity",
						 typeof(DataAnnotationsLocalizer).GetTypeInfo().Assembly.FullName);
				}

				return _identityLocalizer;
			};


		private static Dictionary<string, Func<IStringLocalizerFactory, IStringLocalizer>> _localizerDictionary
			= new Dictionary<string, Func<IStringLocalizerFactory, IStringLocalizer>>
			{
				// MedStat.Core types:

				{ typeof(Company).FullName, GetCompanyLocalizer },
				{ typeof(CompanyMainRequisites).FullName, GetCompanyLocalizer },
				{ typeof(CompanyBankRequisites).FullName, GetCompanyLocalizer },
				{ typeof(CompanyUser).FullName, GetCompanyLocalizer },
				{ typeof(CompanyUserInfo).FullName, GetCompanyLocalizer },

				{ typeof(SystemUser).FullName, GetIdentityUserLocalizer },


				// MedStat.WebAdmin types:

				{ typeof(LoginPageModel).FullName, GetIdentityUserLocalizer },
				{ typeof(ChangePasswordPageModel).FullName, GetIdentityUserLocalizer }
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
