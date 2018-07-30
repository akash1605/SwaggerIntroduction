using System.ComponentModel.DataAnnotations;

namespace SwaggerIntroduction.Models.ApiModels
{
    public class TokenRequestModel :  IApiRequest
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