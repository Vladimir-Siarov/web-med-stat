using System;
using System.ComponentModel.DataAnnotations;
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

		public DateTime CreatedUtc { get; set; }

		public DateTime UpdatedUtc { get; set; }


		/* Requisites: */

		public CompanyRequisites Requisites { get; set; }
	}
}
