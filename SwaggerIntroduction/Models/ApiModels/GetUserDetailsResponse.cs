using System;

namespace SwaggerIntroduction.Models.ApiModels
{
    public class GetUserDetailsResponse : IApiResponse
    {
        public string UserEmail { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime CreationDate { get; set; }
    }
}