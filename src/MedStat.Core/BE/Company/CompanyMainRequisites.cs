using System.ComponentModel.DataAnnotations;

namespace MedStat.Core.BE.Company
{
	// Owned class
	public class CompanyMainRequisites
	{
		[Display(Name = "Short Name")] // "Сокращенное наименование"
		public string Name { get; set; }

		[Display(Name = "Full Name")] // "Полное наименование"
		public string FullName { get; set; }


		[Display(Name = "Legal Address")] // "Юридический адрес"
		public string LegalAddress { get; set; }

		[Display(Name = "Postal Address")] // "Почтовый адрес"
		public string PostalAddress { get; set; }


		// Exp: 1089848059366
		[Display(Name = "OGRN")] // "ОГРН"
		public string OGRN { get; set; }

		// Exp: 89041828
		[Display(Name = "OKPO")] // "ОКПО"
		public string OKPO { get; set; }

		// Exp: 40278562000
		[Display(Name = "OKATO")] // "ОКАТО"
		public string OKATO { get; set; }

		// Exp: 7842399004
		[Display(Name = "INN")] // "ИНН"
		public string INN { get; set; }

		// Exp: 780601001
		[Display(Name = "KPP")] // "КПП"
		public string KPP { get; set; }


		// ОКВЭД
	}
}
