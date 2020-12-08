
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
}
