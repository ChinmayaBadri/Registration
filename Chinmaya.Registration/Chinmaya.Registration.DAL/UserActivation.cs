//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chinmaya.Registration.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserActivation
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ActivationCode { get; set; }
        public Nullable<bool> IsActivated { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ActivatedDate { get; set; }
    }
}
