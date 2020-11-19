using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MedStat.WebAdmin.Classes
{
	public static class NavHelper
	{
		public enum EnMainMenuItem
		{
			Home,
			Company
		}


		private const string SelectedMainMenuItem = "SelectedMainMenuItem";

		
		public static void SetSelectedMainMenu(this ViewDataDictionary viewData, EnMainMenuItem menuItem)
		{
			viewData[SelectedMainMenuItem] = menuItem;
		}


		public static bool IsCompanyMainMenuSelected(this ViewDataDictionary viewData)
		{
			return 
				viewData.IsMainMenuSelected(EnMainMenuItem.Company);
		}
		public static bool IsHomeMainMenuSelected(this ViewDataDictionary viewData)
		{
			return
				viewData.IsMainMenuSelected(EnMainMenuItem.Home);
		}


		private static bool IsMainMenuSelected(this ViewDataDictionary viewData, EnMainMenuItem menuItem)
		{
			return
				viewData[SelectedMainMenuItem] is EnMainMenuItem 
				&& ((EnMainMenuItem) viewData[SelectedMainMenuItem]) == menuItem;
		}
	}
}
