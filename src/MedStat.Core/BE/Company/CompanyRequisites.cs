using System;

namespace MedStat.Core.BE.Company
{
	public class CompanyRequisites
	{
		public int Id { get; set; }

		public DateTime UpdatedUtc { get; set; }


		/* Requisites: */

		public CompanyMainRequisites MainRequisites { get; set; }

		public CompanyBankRequisites BankRequisites { get; set; }


		// Navigation properties:

		public int CompanyId { get; set; }
	}
}
