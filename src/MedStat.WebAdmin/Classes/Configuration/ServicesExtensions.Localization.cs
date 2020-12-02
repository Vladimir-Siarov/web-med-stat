using System.Globalization;
using MedStat.Core.DAL;
using MedStat.Core.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MedStat.WebAdmin.Classes.Configuration
{
	public static partial class ServicesExtensions
	{
		/// <summary>
		/// Adds MVC data annotations localization for "MedStat.Core" BEs to the application.
		/// </summary>
		/// <param name="builder">The <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcBuilder" />.</param>
		/// <returns>The <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcBuilder" />.</returns>
		public static IMvcBuilder AddCoreDataAnnotationsLocalization(this IMvcBuilder builder) 
		{
			return
				builder.AddDataAnnotationsLocalization(options =>
				{
					options.DataAnnotationLocalizerProvider = (type, factory) =>
					{
						// check is "type" from the "MedStat.Core"
						var stringLocalizer = Core.Resources.Localizer.GetDataAnnotationLocalizer(type, factory);
						if (stringLocalizer == null)
						{
							// create default Localizer for Web project
							stringLocalizer = factory.Create(type);
						}

						return stringLocalizer;
					};
				});
		}

		/// <summary>
		/// Configure request localization for "MedStat.Admin" web app.
		/// </summary>
		/// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the service to.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public static IServiceCollection ConfigureRequestLocalization(this IServiceCollection services)
		{
			return
				services.Configure<RequestLocalizationOptions>(options =>
				{
					var supportedCultures = new[]
					{
						new CultureInfo("en-US"),
						new CultureInfo("en-GB"),
						new CultureInfo("en"),
						new CultureInfo("ru-RU")
					};

					options.DefaultRequestCulture = new RequestCulture("ru-RU");
					options.SupportedCultures = supportedCultures;
					options.SupportedUICultures = supportedCultures;
				});
		}
	}
}
