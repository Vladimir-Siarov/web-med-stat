using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedStat.WebAdmin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MedStat.WebAdmin.Classes
{
	public static class DataTableHelper
	{
		public struct RequestKeys
		{
			public const string SearchTerm = "search[value]";

			public const string SortByColumnIndex = "order[0][column]";
			public const string SortByDirection = "order[0][dir]";

			public const string Take = "length";
			public const string Skip = "start";
		}

		private const string DataTablePageFlag = "DATA_TABLE_PAGE";


		public static DataTableModel ParseDataTableRequest(HttpRequest request)
		{
			if(request == null)
				throw new ArgumentNullException(nameof(request));

			DataTableModel model = new DataTableModel();
			var query = request.Query;

			// search properties
			if (query.ContainsKey(RequestKeys.SearchTerm))
			{
				model.SearchTerm = query[RequestKeys.SearchTerm];
			}

			// sorting properties
			if (query.ContainsKey(RequestKeys.SortByColumnIndex))
			{
				model.SortByColumnIndex = int.Parse(query[RequestKeys.SortByColumnIndex][0]);
			}
			if (query.ContainsKey(RequestKeys.SortByDirection))
			{
				model.IsSortByAsc = query[RequestKeys.SortByDirection][0]
					.Equals("asc", StringComparison.InvariantCultureIgnoreCase);
			}

			// paging properties
			if (query.ContainsKey(RequestKeys.Take))
			{
				model.Take = int.Parse(query[RequestKeys.Take][0]);
			}
			if (query.ContainsKey(RequestKeys.Skip))
			{
				model.Skip = int.Parse(query[RequestKeys.Skip][0]);
			}

			return model;
		}


		// Extensions

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
