﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwaggerIntroduction.Models;
using SwaggerIntroduction.Models.ApiModels;
using SwaggerIntroduction.Repository;
using SwaggerIntroduction.Security;

namespace SwaggerIntroduction.Controllers
{
    [Route("api/tokens")]
    public class Tokens : BaseController<Tokens>
    {
        private readonly PasswordHashingHelper _passwordHashingHelper;

        public Tokens(IUserRepository repo, ILogger<Tokens> logger, IMapper mapper, IOptions<AppSettingsConfigurationModel> settings, IHandleTokens tokenHandler) : base(repo, logger, mapper, settings, tokenHandler)
        {
            _passwordHashingHelper = new PasswordHashingHelper(AppSettings.Value.PasswordAdditive);
        }

        /// <summary>
        /// Returns a token after validating a user.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponseModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateToken([FromBody] TokenRequestModel requestModelModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var details = Repo.GetUserMaster(requestModelModel.UserEmail);
            if (details == null)
            {
                return BadRequest("User validation failed");
            }

            var hashedPassword = _passwordHashingHelper.HashValues(requestModelModel.Password, _passwordHashingHelper.GetSaltFromString(details.Salt));

            if (!string.Equals(hashedPassword, details.UserPassword))
            {
                return BadRequest("User validation failed");
            }

            var token = TokenHandler.Create(requestModelModel.UserEmail, AppSettings.Value.SigningKey);

            if (string.IsNullOrEmpty(token))
            {
                return StatusCode(500);
            }

            var tokenModel = new TokenResponseModel()
            {
                UserEmail = requestModelModel.UserEmail,
                BearerToken = token
            };

            return Ok(tokenModel);
        }
    }
}