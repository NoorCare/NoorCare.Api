﻿using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using WebAPI.Entity;

namespace WebAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //AspNetUsers -> User
            modelBuilder.Entity<ApplicationUser>().ToTable("User");
            //AspNetRoles -> Role
            modelBuilder.Entity<IdentityRole>().ToTable("Role");
            //AspNetUserRoles -> UserRole
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole");
            //AspNetUserClaims -> UserClaim
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
            //AspNetUserLogins -> UserLogin
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<ClientDetail>().ToTable("ClientDetail");
            modelBuilder.Entity<Facility>().ToTable("Facility");
            modelBuilder.Entity<Disease>().ToTable("DiseaseType");
            modelBuilder.Entity<EmergencyContact>().ToTable("EmergencyContact");
            modelBuilder.Entity<MedicalInformation>().ToTable("MedicalInformation");
            modelBuilder.Entity<CountryCode>().ToTable("CountryCode");
            modelBuilder.Entity<City>().ToTable("citys");
            modelBuilder.Entity<State>().ToTable("tblState");
        }
    }
}