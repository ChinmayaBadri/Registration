using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Chinmaya.Registration.Models
{
	public class CheckPaymentModel
	{
		public int Id { get; set; }

		[Required]
		[StringLength(200, ErrorMessage = "Account Holder Name should not be more than 200 characters")]
		[DisplayName("Account Holder Name")]
		public string AccountHolderName { get; set; }

		[Required]
		[DisplayName("Account Type")]
		public int AccountTypeId { get; set; }

		[Required]
		[StringLength(200, ErrorMessage = "Bank name should not be more than 200 characters")]
		[DisplayName("Bank name")]
		public string BankName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "Account Number should not be more than 100 characters")]
		[DisplayName("Account Number")]
		public string AccountNumber { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "Routing Number should not be more than 100 characters")]
		[DisplayName("Routing Number")]
		public string RoutingNumber { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "Account Number should not be more than 100 characters")]
		[DisplayName("Confirm Account Number")]
		[Compare("AccountNumber")]
		public string ConfirmAccountNumber { get; set; }

		public Nullable<decimal> Amount { get; set; }
		public int StatusId { get; set; }
		public string CreatedBy { get; set; }
		public System.DateTime CreatedDate { get; set; }
	}
}
