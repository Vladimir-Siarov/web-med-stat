using System;
using System.Collections.Generic;
using System.Text;
using MedStat.Core.BE.Company;

namespace MedStat.Core.Helpers
{
	public partial class CopyHelper
	{
		public static CompanyMainRequisites CreateCopy(this CompanyMainRequisites requisites)
		{
			return
				new CompanyMainRequisites
				{
					Name = requisites.Name,
					FullName = requisites.FullName,

					LegalAddress = requisites.LegalAddress,
					PostalAddress = requisites.PostalAddress,

					OGRN = requisites.OGRN,
					OKPO = requisites.OKPO,
					OKATO = requisites.OKATO,
					INN = requisites.INN,
					KPP = requisites.KPP
				};
		}

		public static CompanyBankRequisites CreateCopy(this CompanyBankRequisites requisites)
		{
			return
				new CompanyBankRequisites
				{
					AccountNumber = requisites.AccountNumber,
					BIC = requisites.BIC,

					CorrespondentAccount = requisites.CorrespondentAccount,
					Bank = requisites.Bank
				};
		}
	}
}
