using System;
using System.Collections.Generic;
using System.Reflection;
using MedStat.Core.BE.Company;
using Microsoft.Extensions.Localization;

namespace MedStat.Core.Resources
{
	public static class Localizer
	{
		public struct DataAnnotations
		{
			public const string RequiredErrorMessage = "The \"{0}\" field is required.";
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

		private static Dictionary<string, Func<IStringLocalizerFactory, IStringLocalizer>> _localizerDictionary
			= new Dictionary<string, Func<IStringLocalizerFactory, IStringLocalizer>>
			{
				{ typeof(Company).FullName, GetCompanyLocalizer },
				{ typeof(CompanyMainRequisites).FullName, GetCompanyLocalizer },
				{ typeof(CompanyBankRequisites).FullName, GetCompanyLocalizer },
			};


		public static Func<Type, IStringLocalizerFactory, IStringLocalizer> GetDataAnnotationLocalizer
		 = (type, factory) =>
		 {
			 string typeName = type.FullName;

			 if (_localizerDictionary.ContainsKey(typeName))
			 {
				 return _localizerDictionary[typeName](factory);
			 }

			 return null;
		 };
	}
}
