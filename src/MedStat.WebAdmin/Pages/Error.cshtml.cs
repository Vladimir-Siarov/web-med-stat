using System;
using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Localization;

namespace MedStat.WebAdmin.Pages
{
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	[IgnoreAntiforgeryToken]
	public class ErrorModel : PageModel
	{
		private readonly ILogger<ErrorModel> _logger;
		private readonly IStringLocalizer<ErrorModel> _localizer;


		public string RequestId { get; set; }

		public int? ResponseStatusCode { get; set; }

		public string ErrorMessage { get; set; }

		public string LogErrorMessage { get; set; }


		public ErrorModel(ILogger<ErrorModel> logger, IStringLocalizer<ErrorModel> localizer)
		{
			_logger = logger;
			_localizer = localizer;
		}


		public void OnGet(string httpCode)
		{
			this.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
			this.ResponseStatusCode = HttpContext.Response?.StatusCode;

			if (!string.IsNullOrEmpty(httpCode) && int.TryParse(httpCode, out var code))
			{
				this.ResponseStatusCode = code;
			}

			var error = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
			if (error != null)
			{
				try
				{
					// Exception was already logged by app.
					// Lets log "RequestId" to link user's screenshot with logged exception.
					_logger.LogInformation("Error page was displayed: {RequestId}, {StatusCode}, {UserUid}", 
						this.RequestId, this.ResponseStatusCode, this.User?.Identity?.Name);
				}
				catch (Exception logError)
				{
					Console.WriteLine(logError);
					this.LogErrorMessage = logError.Message;
				}
			}

			switch (this.ResponseStatusCode)
			{
				case 400:
					this.ErrorMessage = _localizer["Bad Request."];
					break;

				case 404:
					this.ErrorMessage = _localizer["Page is not found."];
					break;

				default:
					this.ErrorMessage = error?.Message;
					break;
			}
		}
	}
}
