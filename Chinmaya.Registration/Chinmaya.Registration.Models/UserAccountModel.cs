using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Chinmaya.Models
{
	class UserAccountModel
	{
		public class UserModel
		{
			public string Id { get; set; }
			public string Email { get; set; }
			public bool EmailConfirmed { get; set; }
			public string Password { get; set; }
			public int RoleId { get; set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public System.DateTime DOB { get; set; }
			public Nullable<int> AgeGroupId { get; set; }
			public int GenderId { get; set; }
			public string Address { get; set; }
			public string City { get; set; }
			public int StateId { get; set; }
			public int CountryId { get; set; }
			public string ZipCode { get; set; }
			public string HomePhone { get; set; }
			public string CellPhone { get; set; }
			public bool IsIndividual { get; set; }
			public bool Status { get; set; }
			public string CreatedBy { get; set; }
			public System.DateTime CreatedDate { get; set; }
			public string UpdatedBy { get; set; }
			public Nullable<System.DateTime> UpdatedDate { get; set; }
			public Dictionary<int, string> UserSecurityQuestions = new Dictionary<int, string>();
		}


		public class FamilyMemberModel
		{
			[DisplayName("First Name")]
			[Required]
			[StringLength(100, ErrorMessage = "First Name should not be more than 100 characters")]
			public string FirstName { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "Last Name should not be more than 100 characters")]
			[DisplayName("Last Name")]
			public string LastName { get; set; }

			[Required]
			[DisplayName("Date of Birth")]
			public System.DateTime DOB { get; set; }

			[Required]
			[DisplayName("Relationship")]
			public int RelationshipData { get; set; }

			[DisplayName("Grade")]
			public int Grade { get; set; }

			[DisplayName("Gender")]
			public int GenderData { get; set; }

			[StringLength(20, ErrorMessage = "Cell Phone should not be more than 20 characters")]
			[DisplayName("Cell Phone")]
			public string CellPhone { get; set; }

			[DisplayName("Email Address")]
			[EmailAddress(ErrorMessage = "Invalid Email Address")]
			[StringLength(256, ErrorMessage = "Email should not be more than 256 characters")]
			public string Email { get; set; }

			public string UpdatedBy { get; set; }
		}
	}
}
