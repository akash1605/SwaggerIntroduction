using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwaggerIntroduction.Models.ApiModels;
using SwaggerIntroduction.Models.DataModels;
using SwaggerIntroduction.Repository;

namespace SwaggerIntroduction.Controllers
{
    [Route("api/tokens")]
    public class Tokens : BaseController<Tokens>
    {
        private readonly SignInManager<TokenRequestModel> _signInManager;

        public Tokens(IUserRepository repo, ILogger<Tokens> logger, IMapper mapper, SignInManager<TokenRequestModel> signInManager) : base(repo, logger, mapper)
        {
            _signInManager = signInManager;
        }

        [HttpPost]
        public IActionResult CreateToken([FromBody] TokenRequestModel requestModelModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validation.Validate(FromBodyAttribute.GetCustomAttributes());
            var token = new TokenResponseModel();
            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserMaster model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(model.UserEmail,
                model.UserPassword, false, false);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}