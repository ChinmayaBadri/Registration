using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Models
{
	public class EventRegistrationModel
	{
		public int Id { get; set; }
		public string EventId { get; set; }
		public string FamilyMemberId { get; set; }
		public string OwnerId { get; set; }
		public Nullable<int> CheckPaymentId { get; set; }
		public Nullable<int> InvoiceId { get; set; }
		public Nullable<bool> IsRegister { get; set; }
		public Nullable<bool> IsConfirm { get; set; }
		public Nullable<bool> IsPaid { get; set; }
		public string CreatedBy { get; set; }
		public System.DateTime CreatedDate { get; set; }
	}
}
