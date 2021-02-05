using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MedStat.WebAdmin.Classes
{
	public static class NavHelper
	{
		public enum EnMainMenuItem
		{
			Home,
			Company,
			Account,

			Devices, // menu group
				DeviceList,
				DeviceModels
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
		public static bool IsDevicesMainMenuSelected(this ViewDataDictionary viewData)
		{
			return
				viewData.IsMainMenuSelected(EnMainMenuItem.Devices);
		}


		public static bool IsMainMenuSelected(this ViewDataDictionary viewData, EnMainMenuItem menuItem)
		{
			return
				viewData[SelectedMainMenuItem] is EnMainMenuItem 
				&& ((EnMainMenuItem) viewData[SelectedMainMenuItem]) == menuItem;
		}
	}
}
