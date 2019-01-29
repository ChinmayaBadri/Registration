using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Models
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
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<bool> IsApproveMailSent { get; set; }
		public Nullable<bool> IsLocked { get; set; }
		public Nullable<int> NumberOfAttempts { get; set; }
		public Dictionary<int, string> UserSecurityQuestions = new Dictionary<int, string>();
        public string RoleName { get; set; }
    }

	public class UserInfoModel
	{
		public string Id { get; set; }
		public string FullName { get; set; }
		public string AccountType { get; set; }
		public string DOB { get; set; }
		public string HomePhone { get; set; }
		public string CellPhone { get; set; }
	}
	
}
