using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MedStat.WebAdmin.Classes.SharedResources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

namespace MedStat.WebAdmin.Classes.TagHelpers.SiteNav
{
	public class SiteNavLinkTagHelper : TagHelper
	{
		private readonly IUrlHelperFactory _urlHelperFactory;
		private readonly IStringLocalizer<NavResource> _navLocalizer;

		#region Svg icons

		private const string SvgIcon_Home =
			@"<svg width='100%' height='100%' viewBox='0 0 16 16' class='bi bi-house-fill' fill='currentColor' xmlns='http://www.w3.org/2000/svg'>
<path fill-rule='evenodd' d='M8 3.293l6 6V13.5a1.5 1.5 0 0 1-1.5 1.5h-9A1.5 1.5 0 0 1 2 13.5V9.293l6-6zm5-.793V6l-2-2V2.5a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5z'></path>
<path fill-rule='evenodd' d='M7.293 1.5a1 1 0 0 1 1.414 0l6.647 6.646a.5.5 0 0 1-.708.708L8 2.207 1.354 8.854a.5.5 0 1 1-.708-.708L7.293 1.5z'></path>
</svg>";

		private const string SvgIcon_Company =
			@"<svg width='100%' height='100%' viewBox='0 0 16 16' class='bi bi-house-fill' fill='currentColor' xmlns='http://www.w3.org/2000/svg'>
<path fill-rule='evenodd' d='M14.763.075A.5.5 0 0 1 15 .5v15a.5.5 0 0 1-.5.5h-3a.5.5 0 0 1-.5-.5V14h-1v1.5a.5.5 0 0 1-.5.5h-9a.5.5 0 0 1-.5-.5V10a.5.5 0 0 1 .342-.474L6 7.64V4.5a.5.5 0 0 1 .276-.447l8-4a.5.5 0 0 1 .487.022zM6 8.694L1 10.36V15h5V8.694zM7 15h2v-1.5a.5.5 0 0 1 .5-.5h2a.5.5 0 0 1 .5.5V15h2V1.309l-7 3.5V15z'/>
<path d='M2 11h1v1H2v-1zm2 0h1v1H4v-1zm-2 2h1v1H2v-1zm2 0h1v1H4v-1zm4-4h1v1H8V9zm2 0h1v1h-1V9zm-2 2h1v1H8v-1zm2 0h1v1h-1v-1zm2-2h1v1h-1V9zm0 2h1v1h-1v-1zM8 7h1v1H8V7zm2 0h1v1h-1V7zm2 0h1v1h-1V7zM8 5h1v1H8V5zm2 0h1v1h-1V5zm2 0h1v1h-1V5zm0-2h1v1h-1V3z'/>
</svg>";

		private const string SvgIcon_SmartWatch =
			@"<svg width='100%' height='100%' viewBox='0 0 16 16' class='bi bi-house-fill' fill='currentColor' xmlns='http://www.w3.org/2000/svg'>
<path d='M9 5a.5.5 0 0 0-1 0v3H6a.5.5 0 0 0 0 1h2.5a.5.5 0 0 0 .5-.5V5z'/>
<path d='M4 1.667v.383A2.5 2.5 0 0 0 2 4.5v7a2.5 2.5 0 0 0 2 2.45v.383C4 15.253 4.746 16 5.667 16h4.666c.92 0 1.667-.746 1.667-1.667v-.383a2.5 2.5 0 0 0 2-2.45V8h.5a.5.5 0 0 0 .5-.5v-2a.5.5 0 0 0-.5-.5H14v-.5a2.5 2.5 0 0 0-2-2.45v-.383C12 .747 11.254 0 10.333 0H5.667C4.747 0 4 .746 4 1.667zM4.5 3h7A1.5 1.5 0 0 1 13 4.5v7a1.5 1.5 0 0 1-1.5 1.5h-7A1.5 1.5 0 0 1 3 11.5v-7A1.5 1.5 0 0 1 4.5 3z'/>
</svg>";

		#endregion


		/// <summary>
		/// Defines main menu item for current element.
		/// </summary>
		public NavHelper.EnMainMenuItem? MenuItem { get; set; } = null;

		/// <summary>
		/// The page name to generate the url for.
		/// </summary>
		public string PageName { get; set; }

		/// <summary>
		/// Specifies text of current menu link.
		/// </summary>
		public string Text { get; set; }


		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }


		public SiteNavLinkTagHelper(IUrlHelperFactory urlHelperFactory,
			IStringLocalizer<NavResource> navLocalizer)
		{
			this._urlHelperFactory = urlHelperFactory;
			this._navLocalizer = navLocalizer;
		}


		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.Attributes.RemoveAll("href");
			output.Attributes.RemoveAll("class");
			output.Attributes.RemoveAll("aria-current");

			if (!string.IsNullOrEmpty(this.PageName))
			{
				output.TagName = "a";

				var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

				output.Attributes.SetAttribute("href", urlHelper.Page(this.PageName));
			}
			else
			{
				output.TagName = "span";
			}

			output.Attributes.SetAttribute("class", "nav-link");

			// if active menu link
			if (this.MenuItem.HasValue && ViewContext.ViewData.IsMainMenuSelected(this.MenuItem.Value))
			{
				output.Attributes.SetAttribute("aria-current", "page");
			}


			output.Content.Clear();

			// append icon
			{
				string svgIcon = this.MenuItem.HasValue ? GetSvgIcon(this.MenuItem.Value) : null;

				// append SVG icon
				if (!string.IsNullOrEmpty(svgIcon))
				{
					output.Content.AppendFormat("<span class='nav-link-icon' title='{0}'>", this.Text);
					output.Content.AppendHtml(svgIcon);
					output.Content.AppendHtml("</span>");	
				}
				else if(!string.IsNullOrEmpty(this.Text)) // append Text icon
				{
					output.Content.AppendFormat(
						"<span class='nav-link-textIcon' title='{0}' aria-hidden='true'>{1}</span>", 
						this.Text, this.Text.Substring(0, 1));
				}
			}

			// append link text
			{
				output.Content.AppendFormat("<span class='nav-link-text'>{0}</span>", this.Text);
			}
		}


		private static string GetSvgIcon(NavHelper.EnMainMenuItem menuItem)
		{
			switch (menuItem)
			{
				case NavHelper.EnMainMenuItem.Home:
					return SvgIcon_Home;

				case NavHelper.EnMainMenuItem.Company:
					return SvgIcon_Company;

				case NavHelper.EnMainMenuItem.Devices:
					return SvgIcon_SmartWatch;

				default:
					return string.Empty;
			}
		}
	}
}
