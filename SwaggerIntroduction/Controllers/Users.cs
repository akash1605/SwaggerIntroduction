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

            var isOperationSuccessful = Getemail(out var email);
            if (!isOperationSuccessful)
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

        [HttpPut("editpassword")]
        [Authorize]
        public IActionResult UpdateUserPassword([FromBody] UpdatePasswordRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isOperationSuccessful = Getemail(out var email);
            if (!isOperationSuccessful)
            {
                return StatusCode(500);
            }

            var result = Repo.GetUserMaster(email);
            if (result == null)
            {
                return StatusCode(500);
            }


            var userOldPassword = _passwordHashingHelper.HashValues(model.OldPassword, _passwordHashingHelper.GetSaltFromString(result.Salt));
            if (!string.Equals(result.UserPassword, userOldPassword))
            {
                return BadRequest("Update failed");
            }

            var (salt, userNewPassword) = _passwordHashingHelper.GetHashedPassword(model.NewPassword);
            result.UserPassword = userNewPassword;
            result.Salt = salt;

            Repo.UpdateMasterInformation(result);
            var saveResult = Repo.SaveData();

            if (saveResult != 1)
            {
                return BadRequest();
            }

            return Ok("Password Updated!");
        }

        [HttpPost("address")]
        [Authorize]
        public IActionResult AddAddress([FromBody] AddAddressModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isOperationSuccessful = Getemail(out var email);
            if (!isOperationSuccessful)
            {
                return StatusCode(500);
            }

            var result = Repo.GetUserMaster(email);
            if (result == null)
            {
                return StatusCode(500);
            }

            var addressDetails = Mapper.Map<UserAddress>(model);
            addressDetails.UserId = result.UserId;

            Repo.AddDataToDataSet(addressDetails);
            if (model.MarkAsDefault != null && model.MarkAsDefault == true)
            {
                Repo.UnMarkUserAddressNotDefault(result.UserId);
            }

            var saveResult = Repo.SaveData();

            if (saveResult != 1 || saveResult != 2)
            {
                return StatusCode(500);
            }

            return Created("api/users/address", model);
        }

        [HttpPut("address/{id}")]
        [Authorize]
        public IActionResult UpdateSingleAddress([FromBody] AddAddressModel model, [FromRoute] int id)
        {
            var isOperationSuccessful = Getemail(out var email);
            if (!isOperationSuccessful)
            {
                return StatusCode(500);
            }

            var result = Repo.GetUserMaster(email);
            if (result == null)
            {
                return StatusCode(500);
            }

            if (model.MarkAsDefault != null && model.MarkAsDefault == true)
            {
                Repo.UnMarkUserAddressNotDefault(result.UserId);
            }

            var addressDetails = Mapper.Map<UserAddress>(model);
            Repo.UpdateParticularAddress(id, addressDetails);
            var saveResult = Repo.SaveData();

            if (saveResult == 1 || saveResult == 2 || saveResult == 0)
            {
                return Ok();
            }

            return StatusCode(500);
        }

        private bool Getemail(out string email)
        {
            var currentUserClaims = HttpContext.User;
            email = TokenHandler.GetEmailFromClaims(currentUserClaims);
            return !string.IsNullOrEmpty(email);
        }
    }
}