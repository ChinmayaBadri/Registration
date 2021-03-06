﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Chinmaya.Registration.Models
{
	public class ContactDetails
	{
		public string Email { get; set; }

		[Required]
		[StringLength(500, ErrorMessage = "Address should not be more than 500 characters")]
		[DisplayName("Address")]
		public string Address { get; set; }
		[DisplayName("City")]
		[Required]
		[StringLength(50, ErrorMessage = "City should not be more than 50 characters")]
		public string City { get; set; }
		[DisplayName("State")]
		[Required]
		public int State { get; set; }
		[DisplayName("Zipcode")]
		[RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Zip")]
		[Required]
		//[StringLength(10, ErrorMessage = "ZipCode should not be more than 10 characters")]
		public string ZipCode { get; set; }
		[DisplayName("Home Phone")]
		[Required]
		[RegularExpression(@"^(\d{10})$", ErrorMessage = "Invalid Home Phone")]
		//[StringLength(20, ErrorMessage = "Home Phone should not be more than 20 characters")]
		public string HomePhone { get; set; }
		[DisplayName("Country")]
		[Required]
		public int Country { get; set; }
		//[StringLength(20, ErrorMessage = "Cell Phone should not be more than 20 characters")]
		[RegularExpression(@"^(\d{10})$", ErrorMessage = "Invalid Cell Phone")]
		[DisplayName("Cell Phone")]
		public string CellPhone { get; set; }
	}
}
