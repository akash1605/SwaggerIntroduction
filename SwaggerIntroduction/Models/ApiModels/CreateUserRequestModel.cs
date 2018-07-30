using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SwaggerIntroduction.Models.ApiModels
{
    public class CreateUserRequestModel : IApiRequest
    {
        [HiddenInput]
        public int UserId { get; set; }

        [HiddenInput]
        public DateTime CreationDate { get; set; }

        [Required, EmailAddress]
        [MaxLength(225)]
        public string UserEmail { get; set; }

        [Required]
        [MaxLength(20)]
        [MinLength(8)]
        public string UserPassword { get; set; }

        [Required]
        [MaxLength(225)]
        public string FirstName { get; set; }

        [MaxLength(225)]
        public string LastName { get; set; }
    }

    public class CreateUserResponseModel : IApiResponse
    {
        public string UserEmail { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Token { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
