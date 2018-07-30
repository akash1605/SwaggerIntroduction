using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwaggerIntroduction.Models;
using SwaggerIntroduction.Repository;
using SwaggerIntroduction.Security;

namespace SwaggerIntroduction.Controllers
{
    public class BaseController<T> : Controller
    {
        protected readonly IUserRepository Repo;

        protected readonly ILogger<T> Logger;

        protected readonly IMapper Mapper;

        protected readonly IOptions<AppSettingsConfigurationModel> AppSettings;

        protected readonly IHandleTokens TokenHandler;

        protected BaseController(IUserRepository repo, ILogger<T> logger, IMapper mapper,
            IOptions<AppSettingsConfigurationModel> settings, IHandleTokens tokenHandler)
        {
            Repo = repo;
            Logger = logger;
            Mapper = mapper;
            AppSettings = settings;
            TokenHandler = tokenHandler;
        }
    }
}
