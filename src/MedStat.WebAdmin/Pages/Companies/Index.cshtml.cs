using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedStat.WebAdmin.Classes;

namespace MedStat.WebAdmin.Pages.Companies
{
	public class CompanyListModel : PageModel
	{
		private readonly ILogger<CompanyListModel> _logger;


		public CompanyListModel(ILogger<CompanyListModel> logger)
		{
			_logger = logger;
		}

		public void OnGet()
		{

		}
	}
}
