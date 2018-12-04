using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Models
{
	public class MyAccountModel
	{
		public List<UserFamilyMember> userFamilyMember { get; set; }
        public FamilyMemberModel familyMemberModel { get; set; } = new FamilyMemberModel();
		public UpdatePasswordModel updatePasswordModel { get; set; } = new UpdatePasswordModel();
		public UpdatePhone updatePhone { get; set; } = new UpdatePhone();
		public UpdateEmail updateMailModel { get; set; } = new UpdateEmail();
		public ContactDetails updateAddressModel { get; set; } = new ContactDetails();
		public bool IsIndividual { get; set; }
	}
}
