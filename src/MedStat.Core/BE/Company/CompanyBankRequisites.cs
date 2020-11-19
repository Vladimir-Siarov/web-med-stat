using System.ComponentModel.DataAnnotations;

namespace MedStat.Core.BE.Company
{
	// Owned class
	public class CompanyBankRequisites
	{
		// Exp: 40702810124000011658
		[Display(Name = "Р/С")]
		public string AccountNumber { get; set; }

		// Exp: 044525976
		[Display(Name = "БИК")]
		public string BIC { get; set; }

		// Exp: 30101810500000000976
		[Display(Name = "К/С")]
		public string CorrespondentAccount { get; set; }

		// Exp: КБ "АБСОЛЮТ БАНК" (ПАО) (127051, Г.МОСКВА, ЦВЕТНОЙ Б-Р,18, ИНН 7736046991, КПП 770201001)
		[Display(Name = "Банк")]
		public string Bank { get; set; }
	}
}
