//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chinmaya.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class EventRegistration
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
