using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Models
{
	public class SharedAccountModel
	{
		public int ID { get; set; }
		public string To_UserId { get; set; }
		public string From_UserId { get; set; }
		public Nullable<bool> IsApproved { get; set; }
		public Nullable<bool> IsDeclined { get; set; }
		public Nullable<bool> IsDeclinedPermanently { get; set; }
		public System.DateTime CreatedDate { get; set; }
		public Nullable<System.DateTime> UpdatedDate { get; set; }
	}
}
