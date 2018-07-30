using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwaggerIntroduction.Models;
using SwaggerIntroduction.Models.ApiModels;
using SwaggerIntroduction.Models.DataModels;
using SwaggerIntroduction.Repository;
using SwaggerIntroduction.Security;

namespace SwaggerIntroduction.Controllers
{
    [Route("api/users")]
    public class Users : BaseController<Users>
    {
        private readonly PasswordHashingHelper _passwordHashingHelper;

        public Users(IUserRepository repo, ILogger<Users> logger, IMapper mapper, IOptions<AppSettingsConfigurationModel> settings, IHandleTokens tokenHandler) : base(repo, logger, mapper, settings, tokenHandler)
        {
            _passwordHashingHelper = new PasswordHashingHelper(AppSettings.Value.PasswordAdditive);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUserProfile()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserClaims = HttpContext.User;
            var email = TokenHandler.GetEmailFromClaims(currentUserClaims);

            if (string.IsNullOrEmpty(email))
            {
                return StatusCode(500);
            }

            var returnObject = new GetUserDetailsResponse();
            var userMaster = Repo.GetUserMaster(email);
            returnObject = Mapper.Map(userMaster, returnObject);
            var userDetails = Repo.GetUserDetails(userMaster.UserId);
            returnObject = Mapper.Map(userDetails, returnObject);
            return Ok(returnObject);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserRequestModel createUserRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMaster = Mapper.Map<UserMaster>(createUserRequestModel);
            (userMaster.Salt, userMaster.UserPassword) =
                _passwordHashingHelper.GetHashedPassword(createUserRequestModel.UserPassword);

            if (userMaster.Salt == null || userMaster.UserPassword == null)
            {
                return StatusCode(500);
            }

            await Repo.AddDataToDataSet(userMaster);
            var result = Repo.SaveData();

            if (result != 1)
            {
                Logger.LogWarning("Failed to save data in usermaster table");
                return BadRequest("Could not input user data");
            }

            createUserRequestModel = Mapper.Map(userMaster, createUserRequestModel);
            var userDetails = Mapper.Map<UserDetails>(createUserRequestModel);
            await Repo.AddDataToDataSet(userDetails);
            result = Repo.SaveData();
            if (result == 1)
            {
                createUserRequestModel = Mapper.Map(userDetails, createUserRequestModel);
                return Created("api/users", Mapper.Map<CreateUserResponseModel>(createUserRequestModel));
            }

            Logger.LogWarning("Failed to save data in user details table");
            return BadRequest("could not add a valid value. \n");
        }

        [HttpPut("updatePassword")]
        [Authorize]
        public IActionResult UpdateUserPassword()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}