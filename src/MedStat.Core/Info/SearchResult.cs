using System;
using System.Collections.Generic;
using System.Text;

namespace MedStat.Core.Info
{
	public class SearchResult<T>
	{
		public int TotalRecords { get; set; }

		public IEnumerable<T> Data { get; set; }
	}
}
