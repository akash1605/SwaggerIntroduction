using System.ComponentModel.DataAnnotations;

namespace SwaggerIntroduction.Models.ApiModels
{
    public class AddAddressModel : IApiRequest
    {
        [Required]
        [MaxLength(200)]
        public string AddressLineOne { get; set; }

        [MaxLength(200)]
        public string AddressLineTwo { get; set; }

        [MaxLength(200)]
        public string City { get; set; }

        [MaxLength(200)]
        public string State { get; set; }

        [Required]
        [MaxLength(200)]
        public string Country { get; set; }

        [MaxLength(100)]
        public string Postcode { get; set; }

        [Required]
        public int PhoneNumber { get; set; }

        public bool MarkAsDefault { get; set; }
    }
}
