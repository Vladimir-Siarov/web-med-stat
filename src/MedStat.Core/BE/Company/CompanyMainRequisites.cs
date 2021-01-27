using System.ComponentModel.DataAnnotations;
using MedStat.Core.Resources;

namespace MedStat.Core.BE.Company
{
	// Owned class
	public class CompanyMainRequisites
	{
		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[StringLength(50)]
		[Display(Name = "Short Name")] // "Сокращенное наименование"
		public string Name { get; set; }

		[StringLength(150)]
		[Display(Name = "Full Name")] // "Полное наименование"
		public string FullName { get; set; }


		[StringLength(300)]
		[Display(Name = "Legal Address")] // "Юридический адрес"
		public string LegalAddress { get; set; }

		[StringLength(300)]
		[Display(Name = "Postal Address")] // "Почтовый адрес"
		public string PostalAddress { get; set; }


		// Exp: 1089848059366
		[StringLength(50)]
		[Display(Name = "OGRN")] // "ОГРН"
		public string OGRN { get; set; }

		// Exp: 89041828
		[StringLength(50)]
		[Display(Name = "OKPO")] // "ОКПО"
		public string OKPO { get; set; }

		// Exp: 40278562000
		[StringLength(50)]
		[Display(Name = "OKATO")] // "ОКАТО"
		public string OKATO { get; set; }

		// Exp: 7842399004
		[StringLength(50)]
		[Display(Name = "INN")] // "ИНН"
		public string INN { get; set; }

		// Exp: 780601001
		[StringLength(50)]
		[Display(Name = "KPP")] // "КПП"
		public string KPP { get; set; }


		// ОКВЭД
	}
}
