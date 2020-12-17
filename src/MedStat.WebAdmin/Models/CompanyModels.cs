using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using MedStat.Core.Repositories;
using MedStat.WebAdmin.Classes.SharedResources;

namespace MedStat.WebAdmin.Models
{
	public enum EnCompanySection
	{
		Main,
		Requisites,
		Accounts,
		Trekking
	}


	/// <summary>
	/// Model for "Companies/_PageNavTopPanel.cshtml" view.
	/// </summary>
	public class CompanyPageNavModel
	{
		public int CompanyId { get; set; }

		public EnCompanySection Section { get; set; }
	}


	/// <summary>
	/// Define data set required for "Companies/_CompanyLayout.cshtml" layout page.<br/>
	/// Contains common page data. Used for Header, navigation and etc.
	/// </summary>
	public interface ICompanyLayoutData
	{
		int CompanyId { get; set; }

		string CompanyName { get; set; }

		EnCompanySection Section { get; }


		// Properties for common actions:

		string ConfirmCommand { get; set; }
	}


	public abstract class CompanyBasePageModel : PageModel, ICompanyLayoutData
	{
		protected ILogger Logger { get; }
		
		protected CompanyRepository CmpRepository { get; }
		
		
		protected IStringLocalizer<CompanyResource> CmpLocalizer { get; }

		protected IStringLocalizer<DialogResources> DlgLocalizer { get; }


		private protected CompanyBasePageModel(ILogger logger,
			CompanyRepository cmpRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer)
			: this(logger, cmpRepository, cmpLocalizer, null)
		{
		}

		private protected CompanyBasePageModel(ILogger logger,
			CompanyRepository cmpRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer,
			IStringLocalizer<DialogResources> dlgLocalizer)
		{
			this.Logger = logger;
			this.CmpRepository = cmpRepository;
			this.CmpLocalizer = cmpLocalizer;
			this.DlgLocalizer = dlgLocalizer;
		}


		#region ICompanyLayoutData

		[BindProperty(SupportsGet = true)]
		public int CompanyId { get; set; }

		public string CompanyName { get; set; }

		public abstract EnCompanySection Section { get; }

		[BindProperty]
		public string ConfirmCommand { get; set; }

		#endregion
	}
}
