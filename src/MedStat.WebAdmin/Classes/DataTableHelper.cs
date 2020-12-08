using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MedStat.WebAdmin.Classes
{
	public static class DataTableHelper
	{
		private const string DataTablePageFlag = "DATA_TABLE_PAGE";


		public static void SetDataTablePageFlag(this ViewDataDictionary viewData)
		{
			viewData[DataTablePageFlag] = true;
		}

		public static bool IsDataTablePage(this ViewDataDictionary viewData)
		{
			return 
				viewData.ContainsKey(DataTablePageFlag) && (bool)viewData[DataTablePageFlag];
		}

	}
}
