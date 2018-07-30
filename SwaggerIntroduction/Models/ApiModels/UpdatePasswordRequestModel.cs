using System.ComponentModel.DataAnnotations;

namespace SwaggerIntroduction.Models.ApiModels
{
    public class UpdatePasswordRequestModel : IApiRequest
    {
        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        public string NewPassword { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }
}
