using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SwaggerIntroduction.Models.DataModels
{
    public class UserAddress : IDataObject
    {
        [Key]
        public int UserAddressId { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public string AddressLineOne { get; set; }

        public string AddressLineTwo { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Postcode { get; set; }

        public int PhoneNumber { get; set; }

        public bool IsDefaultAddress { get; set; }
    }

    public class UserDetails : IDataObject
    {
        [Key]
        public int UserDetailsId { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime CreationDate { get; set; }
    }

    public class UserMaster : IdentityUser, IDataObject
    {
        [Key]
        public int UserId { get; set; }

        public string UserEmail { get; set; }

        public string UserPassword { get; set; }
    }
}