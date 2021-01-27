using System.ComponentModel.DataAnnotations;
using MedStat.Core.BE.Company;

namespace MedStat.Core.Info.Company
{
	public class CompanyUserInfo
	{
		public CompanyUser User { get; set; }


		// Rights:

		[Display(Name = "Power user")] // "Привилегированный пользователь"
		public bool IsPowerUser { get; set; }
	}
}
