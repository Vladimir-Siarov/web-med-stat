using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MedStat.WebAdmin.Classes.TagHelpers.SiteNav
{
	public class SiteNavItemTagHelper : TagHelper
	{
		/// <summary>
		/// Defines main menu item for current element.
		/// </summary>
		public NavHelper.EnMainMenuItem? MenuItem { get; set; } = null;

		/// <summary>
		/// Defines that current menu is a menu group item and has a sub-menu.
		/// </summary>
		public bool IsMenuGroup { get; set; }


		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }


		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "li";

			output.Attributes.RemoveAll("class");

			List<string> cssClasses = new List<string>{ "nav-item" };
			{
				if(this.IsMenuGroup)
					cssClasses.Add("nav-groupItem");

				if(this.MenuItem.HasValue && ViewContext.ViewData.IsMainMenuSelected(this.MenuItem.Value))
					cssClasses.Add("active");
			}

			output.Attributes.SetAttribute("class", string.Join(" ", cssClasses));
		}
	}
}
