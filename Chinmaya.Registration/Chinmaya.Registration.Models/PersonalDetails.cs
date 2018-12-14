using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Chinmaya.Registration.Models
{
	public class PersonalDetails
	{
		[Required]
		[StringLength(100, ErrorMessage = "First Name should not be more than 100 characters")]
		[DisplayName("First Name")]
		public string FirstName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "Last Name should not be more than 100 characters")]
		[DisplayName("Last Name")]
		public string LastName { get; set; }

        [Required]
        [DisplayName("Date of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        [DataType(DataType.Date)]
        public Nullable<DateTime> DOB { get; set; } = null;

		[Required]
		[DisplayName("Gender")]
		public int GenderData { get; set; }

		[DisplayName("Age Group")]
		public Nullable<int> AgeGroupData { get; set; }

	}
	
}
