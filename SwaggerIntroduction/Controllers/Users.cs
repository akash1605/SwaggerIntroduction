using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Returns user profile for a signed in user.
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(GetUserDetailsResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public IActionResult GetUserProfile()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isOperationSuccessful = Getemail(out var email);
            if (!isOperationSuccessful)
            {
                return NotFound();
            }

            var returnObject = new GetUserDetailsResponse();
            var userMaster = Repo.GetUserMaster(email);
            returnObject = Mapper.Map(userMaster, returnObject);
            var userDetails = Repo.GetUserDetails(userMaster.UserId);
            returnObject = Mapper.Map(userDetails, returnObject);
            return Ok(returnObject);
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CreateUserResponseModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserRequestModel createUserRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var masterResult = Repo.GetUserMaster(createUserRequestModel.UserEmail);
            if (masterResult != null)
            {
                return Ok("Exists");
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
                var token = TokenHandler.Create(createUserRequestModel.UserEmail, AppSettings.Value.SigningKey);
                if (string.IsNullOrEmpty(token))
                {
                    return StatusCode(500);
                }

                createUserRequestModel = Mapper.Map(userDetails, createUserRequestModel);
                var returnobject = Mapper.Map<CreateUserResponseModel>(createUserRequestModel);
                returnobject.Token = token;
                return Created("api/users", returnobject);
            }

            Logger.LogWarning("Failed to save data in user details table");
            return BadRequest("could not add a valid value. \n");
        }

        /// <summary>
        /// Helps the user to set new password.
        /// </summary>
        [HttpPut("editpassword")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult UpdateUserPassword([FromBody] UpdatePasswordRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = GetUserMasterDetails();
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

            if (saveResult == 3)
            {
                return BadRequest();
            }

            return Ok("Password Updated!");
        }

        /// <summary>
        /// Returns all the addresses for a user.
        /// </summary>
        [HttpGet("address")]
        [Authorize]
        [ProducesResponseType(typeof(List<AddAddressModel>), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        public IActionResult GetUserAddresses()
        {
            var result = GetUserMasterDetails();
            if (result == null)
            {
                return StatusCode(500);
            }

            var addressResultList = Repo.GetUserAddresses(result.UserId);
            var returnObjectList = addressResultList.Select(address => Mapper.Map<AddAddressModel>(address)).ToList();
            return Ok(returnObjectList);
        }

        /// <summary>
        /// Returns the user address whose id is passed in.
        /// </summary>
        [HttpGet("address/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(AddAddressModel), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetUserAddress([FromRoute] int id)
        {
            var result = GetUserMasterDetails();
            if (result == null)
            {
                return StatusCode(500);
            }

            var addressResultList = Repo.GetUserAddress(id).Result;

            if (addressResultList == null)
            {
                return NotFound();
            }

            if (result.UserId != addressResultList.UserId)
            {
                return NotFound();
            }

            var returnObject = Mapper.Map<AddAddressModel>(addressResultList);

            return Ok(returnObject);
        }

        /// <summary>
        /// Adds a new address for a signed in user.
        /// </summary>
        [HttpPost("address")]
        [Authorize]
        [ProducesResponseType(typeof(AddAddressModel), 201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        public IActionResult AddAddress([FromBody] AddAddressModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = GetUserMasterDetails();
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

            if (saveResult == 3)
            {
                return StatusCode(500);
            }

            return Created("api/users/address", model);
        }

        /// <summary>
        /// Edits the given address for a user.
        /// </summary>
        [HttpPut("address/{id}")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        public IActionResult UpdateSingleAddress([FromBody] AddAddressModel model, [FromRoute] int id)
        {
            var result = GetUserMasterDetails();
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

        /// <summary>
        /// Deletes the given address for a user.
        /// </summary>
        [HttpDelete("address/{id}")]
        [Authorize]
        [ProducesResponseType( 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public IActionResult DeleteUserAddress([FromRoute] int id)
        {
            var result = GetUserMasterDetails();
            if (result == null)
            {
                return StatusCode(500);
            }

            var addressResult = Repo.GetUserAddress(id).Result;
            if (addressResult == null)
            {
                return NotFound("No such address error");
            }

            if (result.UserId != addressResult.UserId)
            {
                return NotFound("No such address error");
            }

            Repo.DeleteAddress(addressResult);
            var saveResult = Repo.SaveData();

            return saveResult == 3 ? StatusCode(500) : Ok();
        }

        private UserMaster GetUserMasterDetails()
        {
            var isOperationSuccessful = Getemail(out var email);
            return !isOperationSuccessful ? null : Repo.GetUserMaster(email);
        }

        private bool Getemail(out string email)
        {
            var currentUserClaims = HttpContext.User;
            email = TokenHandler.GetEmailFromClaims(currentUserClaims);
            return !string.IsNullOrEmpty(email);
        }
    }
}