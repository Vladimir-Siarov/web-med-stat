using System.ComponentModel.DataAnnotations;
using MedStat.Core.Identity;

namespace MedStat.Core.BE.Company
{
	public class CompanyUser
	{
		public int Id { get; set; }

		public SystemUser Login { get; set; }

		[Display(Name = "Description")] // "Описание"
		public string Description { get; set; }


		// Navigation properties:

		public int CompanyId { get; set; }

		public int SystemUserId { get; set; }
	}
}
