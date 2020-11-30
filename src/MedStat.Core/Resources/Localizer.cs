using System;
using System.Reflection;
using MedStat.Core.BE.Company;
using Microsoft.Extensions.Localization;

namespace MedStat.Core.Resources
{
	public static class Localizer
	{
		public static readonly string CoreAssemblyName = typeof(Company).GetTypeInfo().Assembly.FullName;
		
		private static readonly Type CompanyType = typeof(Company);
		private static readonly Type CompanyMainRequisitesType = typeof(CompanyMainRequisites);
		private static readonly Type CompanyBankRequisitesType = typeof(CompanyBankRequisites);


		public static Func<Type, IStringLocalizerFactory, IStringLocalizer> GetDataAnnotationLocalizer
		 = (type, factory) =>
		 {
			 if (type == CompanyType
					 || type == CompanyMainRequisitesType
					 || type == CompanyBankRequisitesType)
			 {
				 //var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
				 return factory.Create("BE.Company", CoreAssemblyName);
			 }

			 return null;
		 };
	}
}
