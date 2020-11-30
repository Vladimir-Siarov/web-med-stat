using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedStat.Core.BE.Company;
using MedStat.WebAdmin.Classes;

namespace MedStat.WebAdmin.Pages.Companies
{
	public class CompanyCreateModel : PageModel
	{
		private readonly ILogger<CompanyCreateModel> _logger;

		[BindProperty]
		public Company Company { get; set; }


		public CompanyCreateModel(ILogger<CompanyCreateModel> logger)
		{
			_logger = logger;
		}

		public void OnGet()
		{

		}
	}
}
