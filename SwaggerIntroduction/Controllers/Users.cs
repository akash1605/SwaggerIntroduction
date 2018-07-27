using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwaggerIntroduction.Models.ApiModels;
using SwaggerIntroduction.Models.DataModels;
using SwaggerIntroduction.Repository;

namespace SwaggerIntroduction.Controllers
{
    [Route("api/users")]
    public class Users : BaseController<Users>
    {
        public Users(IUserRepository repo, ILogger<Users> logger, IMapper mapper) : base(repo, logger, mapper)
        {
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUserProfile()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(Repo.GetUserMaster(1));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserRequestModel createUserRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMaster = Mapper.Map<UserMaster>(createUserRequestModel);

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