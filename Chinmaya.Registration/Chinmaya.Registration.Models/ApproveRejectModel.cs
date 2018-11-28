using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Models
{
    public class ApproveRejectModel
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Message { get; set; }
        public bool IsApproved { get; set; }
    }
}
