using System;
using System.Threading.Tasks;
using MedStat.Core.BE.Company;
using MedStat.Core.Repositories;
using MedStat.WebAdmin.Classes.SharedResources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

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


	public class CompanyBasePageModel : PageModel
	{
		protected ILogger Logger { get; }
		
		protected CompanyRepository CmpRepository { get; }
		
		protected IStringLocalizer<CompanyResource> CmpLocalizer { get; }

		protected IStringLocalizer<DialogResources> DlgLocalizer { get; }

		
		public CompanyBasePageModel(ILogger logger,
			CompanyRepository cmpRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer)
		{
			this.Logger = logger;
			this.CmpRepository = cmpRepository;
			this.CmpLocalizer = cmpLocalizer;
			this.DlgLocalizer = null;
		}

		public CompanyBasePageModel(ILogger logger,
			CompanyRepository cmpRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer,
			IStringLocalizer<DialogResources> dlgLocalizer)
		:this(logger, cmpRepository, cmpLocalizer)
		{
			this.DlgLocalizer = dlgLocalizer;
		}
	}
}
