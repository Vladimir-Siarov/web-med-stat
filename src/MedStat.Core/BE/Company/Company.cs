using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MedStat.Core.Resources;

namespace MedStat.Core.BE.Company
{
	public class Company
	{
		public int Id { get; set; }

		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[StringLength(50)]
		[Display(Name = "Name")] // "Название"
		public string Name { get; set; }
		
		[Display(Name = "Description")] // "Описание"
		public string Description { get; set; }

		// TODO: Created, Updated


		/* Requisites: */

		public CompanyMainRequisites MainRequisites { get; set; }

		public CompanyBankRequisites BankRequisites { get; set; }
	}
}
