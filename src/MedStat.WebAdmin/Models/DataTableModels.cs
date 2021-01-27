
namespace MedStat.WebAdmin.Models
{
	public class DataTableModel
	{
		public string SearchTerm { get; set; }

		
		public int SortByColumnIndex { get; set; }

		public bool IsSortByAsc { get; set; }


		public int Take { get; set; }

		public int Skip { get; set; }
	}

	public class DataTableSearchPanelModel
	{
		/// <summary>
		/// Define dom ID of html "table" element.
		/// </summary>
		public string TableId { get; }

		public string PageLengthControlId => $"selectGridPageLength_for_{this.TableId}";

		public string SearchControlId => $"inputGridSearch_for_{this.TableId}";


		public DataTableSearchPanelModel(string tableId)
		{
			this.TableId = tableId;
		}
	}
}
