using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MedStat.Core.BE.Company
{
	public class Company
	{
		public int Id { get; set; }

		
		/* Requisites: */

		public CompanyMainRequisites MainRequisites { get; set; }

		public CompanyBankRequisites BankRequisites { get; set; }
	}
}
