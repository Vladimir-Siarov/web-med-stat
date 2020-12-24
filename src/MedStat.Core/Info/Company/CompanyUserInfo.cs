using System.ComponentModel.DataAnnotations;
using MedStat.Core.BE.Company;

namespace MedStat.Core.Info.Company
{
	public class CompanyUserInfo
	{
		public CompanyUser User { get; set; }


		// Rights:

		[Display(Name = "Company access control")] // "Управление доступом к компании"
		public bool CanManageCompanyAccess { get; set; }

		[Display(Name = "Company staff management")] // "Управление сотрудниками компании"
		public bool CanManageCompanyStaff { get; set; }
	}
}
