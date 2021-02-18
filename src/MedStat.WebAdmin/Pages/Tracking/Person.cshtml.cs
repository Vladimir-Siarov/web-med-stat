using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedStat.WebAdmin.Pages.Tracking
{
	public class TrackingPersonPageModel : PageModel
	{
		public int PersonId { get; set; }


		public TrackingPersonPageModel()
		{
		}


		public IActionResult OnGet()
		{
			return Page();
		}
	}
}
