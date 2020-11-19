using System.ComponentModel.DataAnnotations;

namespace MedStat.Core.BE.Company
{
	// Owned class
	public class CompanyMainRequisites
	{
		[Display(Name = "Сокращенное наименование")]
		public string Name { get; set; }

		[Display(Name = "Полное наименование")]
		public string FullName { get; set; }


		[Display(Name = "Юридический адрес")]
		public string LegalAddress { get; set; }

		[Display(Name = "Почтовый адрес")]
		public string PostalAddress { get; set; }


		// Exp: 1089848059366
		[Display(Name = "ОГРН")]
		public string OGRN { get; set; }

		// Exp: 89041828
		[Display(Name = "ОКПО")]
		public string OKPO { get; set; }

		// Exp: 40278562000
		[Display(Name = "ОКАТО")]
		public string OKATO { get; set; }

		// Exp: 7842399004
		[Display(Name = "ИНН")]
		public string INN { get; set; }

		// Exp: 780601001
		[Display(Name = "КПП")]
		public string KPP { get; set; }


		// ОКВЭД
	}
}
