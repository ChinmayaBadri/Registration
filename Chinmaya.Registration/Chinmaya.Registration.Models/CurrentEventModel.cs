using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Models
{
	public class CurrentEventModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string FamilyMemberName { get; set; }
		public string Description { get; set; }
		public string Weekday { get; set; }
		public string Frequency { get; set; }
		public System.TimeSpan StartTime { get; set; }
		public System.TimeSpan EndTime { get; set; }
		public Nullable<decimal> Amount { get; set; }
		public bool select { get; set; }
		
	}
}
