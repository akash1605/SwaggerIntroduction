using System.ComponentModel.DataAnnotations;

namespace SwaggerIntroduction.Models.ApiModels
{
    public class UpdatePasswordRequestModel : IApiRequest
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }

    public class UpdatePasswordResponseModel : IApiResponse
    {
        public string Email { get; set; }

        public string BearerToken { get; set; }
    }
}
