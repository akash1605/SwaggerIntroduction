using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SwaggerIntroduction.Models.ApiModels
{
    public class TokenRequestModel : IdentityUser, IApiRequest
    {
        [Required, EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class TokenResponseModel : IApiResponse
    {
        public string UserEmail { get; set; }

        public string BearerToken { get; set; }
    }
}