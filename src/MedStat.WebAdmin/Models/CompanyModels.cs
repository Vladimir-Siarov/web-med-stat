
namespace MedStat.WebAdmin.Models
{
	public enum EnCompanySection
	{
		Main,
		Requisites,
		Accounts,
		Trekking
	}

	public class CompanyPageNavModel
	{
		public int? CompanyId { get; set; }

		public EnCompanySection Section { get; set; }
	}
}
