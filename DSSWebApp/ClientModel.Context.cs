﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DSSWebApp
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class customRemoteDbConnectionString : DbContext
    {
        public customRemoteDbConnectionString()
            : base("name=customRemoteDbConnectionString")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<clienti> clienti { get; set; }
        public virtual DbSet<ordini> ordini { get; set; }
        public virtual DbSet<serie> serie { get; set; }
    }
}
