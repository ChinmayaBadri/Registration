using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

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
		[DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
		public System.TimeSpan StartTime { get; set; }
		[DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
		public System.TimeSpan EndTime { get; set; }
		public Nullable<int> AgeFrom { get; set; }
		public Nullable<int> AgeTo { get; set; }
		public Nullable<decimal> Amount { get; set; }
		public bool select { get; set; }
		public bool termsandConditions { get; set; }
	}
}
