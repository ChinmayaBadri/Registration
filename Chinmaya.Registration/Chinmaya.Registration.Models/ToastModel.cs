using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Models
{
    public class ToastModel
    {
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; } = "";
        public string Guid { get; set; }
        public string Type { get; set; }
        public bool isNew { get; set; }
    }
}
