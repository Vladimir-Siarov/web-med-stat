using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedStat.WebAdmin.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;

		public IndexModel(ILogger<IndexModel> logger)
		{
			_logger = logger;
		}

		public IActionResult OnGet()
		{
			if (User.Identity.IsAuthenticated)
			{
				//if (User.IsInRole("XXX"))
				{
					return 
						RedirectToPage("/Companies/Index");
				}
			}

			return Page();
		}
	}
}
