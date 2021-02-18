using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using MedStat.WebAdmin.Classes.SharedResources;

namespace MedStat.WebAdmin.Classes.TagHelpers
{
	public class BreadcrumbTagHelper : TagHelper
	{
		private const string BreadcrumbCssClass = "ms-breadcrumb";

		private const string HomeSvgIcon =
			@"<svg width='1em' height='1em' viewBox='0 0 16 16' class='bi bi-house-fill' fill='currentColor' xmlns='http://www.w3.org/2000/svg'>
  <path fill-rule='evenodd' d='M8 3.293l6 6V13.5a1.5 1.5 0 0 1-1.5 1.5h-9A1.5 1.5 0 0 1 2 13.5V9.293l6-6zm5-.793V6l-2-2V2.5a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5z'/>
  <path fill-rule='evenodd' d='M7.293 1.5a1 1 0 0 1 1.414 0l6.647 6.646a.5.5 0 0 1-.708.708L8 2.207 1.354 8.854a.5.5 0 1 1-.708-.708L7.293 1.5z'/>
</svg>";

		private readonly IUrlHelperFactory _urlHelperFactory;
		private readonly IStringLocalizer<NavResource> _navLocalizer;


		/// <summary>
		/// Define main menu item for current page.
		/// </summary>
		public NavHelper.EnMainMenuItem? MenuItem { get; set; } = null;

		/// <summary>
		/// Define current page Title.
		/// </summary>
		public string PageTitle { get; set; }

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }


		public BreadcrumbTagHelper(IUrlHelperFactory urlHelperFactory,
			IStringLocalizer<NavResource> navLocalizer)
		{
			this._urlHelperFactory = urlHelperFactory;
			this._navLocalizer = navLocalizer;
		}


		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "ol";
			output.Attributes.SetAttribute("class", BreadcrumbCssClass);

			//var content = await output.GetChildContentAsync();
			var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

			// Append Home node
			{
				output.Content.AppendFormat("<li class='home'><a href='{0}' title='{1}'>",
					urlHelper.Page("/Index"),
					_navLocalizer["Home"]);
				output.Content.AppendHtml($"{HomeSvgIcon}</a></li>");
			}

			// Append parent page menu Item node
			if (this.MenuItem.HasValue)
			{
				switch (MenuItem)
				{
					case NavHelper.EnMainMenuItem.Company:
						output.Content.AppendFormat("<li><a href='{0}'>{1}</a></li>",
							urlHelper.Page("/Companies/Index"),
							_navLocalizer["Companies"]);
						break;

					case NavHelper.EnMainMenuItem.Devices:
					case NavHelper.EnMainMenuItem.DeviceList:
					case NavHelper.EnMainMenuItem.DeviceModels:
						output.Content.AppendFormat("<li><a href='{0}'>{1}</a></li>",
							urlHelper.Page("/Devices/Index"),
							_navLocalizer["Devices"]);
						break;

					case NavHelper.EnMainMenuItem.Account:
						output.Content.AppendFormat("<li><a href='{0}'>{1}</a></li>",
							urlHelper.Page("/Account/Manage/Index", new { area = "Identity" }),
							_navLocalizer["Account"]);
						break;

					case NavHelper.EnMainMenuItem.Tracking:
						output.Content.AppendFormat("<li><span>{0}</span></li>",
							_navLocalizer["Tracking"]);
						break;

					default:
						throw new NotSupportedException(MenuItem.ToString());
				}
			}

			// Append current Page node
			output.Content.AppendFormat("<li aria-current='page'>{0}</li>", this.PageTitle);
		}
	}
}
