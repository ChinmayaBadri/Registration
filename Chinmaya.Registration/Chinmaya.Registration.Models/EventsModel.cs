using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Models
{
	public class EventsModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public Nullable<int> WeekdayId { get; set; }
		public int FrequencyId { get; set; }
		public Nullable<int> AgeFrom { get; set; }
		public Nullable<int> AgeTo { get; set; }
		public Nullable<decimal> Amount { get; set; }
		public bool Status { get; set; }
		public string CreatedBy { get; set; }
		public System.DateTime CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public Nullable<System.DateTime> UpdatedDate { get; set; }
	}
}
