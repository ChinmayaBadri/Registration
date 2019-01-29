﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ChinmayaEntities : DbContext
    {
        public ChinmayaEntities()
            : base("name=ChinmayaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CheckPayment> CheckPayments { get; set; }
        public virtual DbSet<CreditCardPayment> CreditCardPayments { get; set; }
        public virtual DbSet<Directory> Directories { get; set; }
        public virtual DbSet<Donation> Donations { get; set; }
        public virtual DbSet<EventHoliday> EventHolidays { get; set; }
        public virtual DbSet<EventRegistration> EventRegistrations { get; set; }
        public virtual DbSet<EventSession> EventSessions { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<UserActivation> UserActivations { get; set; }
        public virtual DbSet<UserSecurityQuestion> UserSecurityQuestions { get; set; }
        public virtual DbSet<AccountType> AccountTypes { get; set; }
        public virtual DbSet<AgeGroup> AgeGroups { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<FamilyGender> FamilyGenders { get; set; }
        public virtual DbSet<Frequency> Frequencies { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<InvoiceStatu> InvoiceStatus { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Relationship> Relationships { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<Site> Sites { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<Weekday> Weekdays { get; set; }
        public virtual DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        public virtual DbSet<FamilyMember> FamilyMembers { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<SharedAccount> SharedAccounts { get; set; }
        public virtual DbSet<User> Users { get; set; }
    
        public virtual ObjectResult<GetUserFamilyMembers_Result> GetUserFamilyMembers(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetUserFamilyMembers_Result>("GetUserFamilyMembers", userIdParameter);
        }
    
        public virtual ObjectResult<GetUserFamilyMember_Result> GetUserFamilyMember(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetUserFamilyMember_Result>("GetUserFamilyMember", userIdParameter);
        }
    
        public virtual ObjectResult<GetFamilyMemberForUser_Result> GetFamilyMemberForUser(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetFamilyMemberForUser_Result>("GetFamilyMemberForUser", userIdParameter);
        }
    }
}
