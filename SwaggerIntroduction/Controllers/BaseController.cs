using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwaggerIntroduction.Repository;

namespace SwaggerIntroduction.Controllers
{
    public class BaseController<T> : Controller
    {
        protected readonly IUserRepository Repo;

        protected readonly ILogger<T> Logger;

        protected readonly IMapper Mapper;

        protected BaseController(IUserRepository repo, ILogger<T> logger, IMapper mapper)
        {
            Repo = repo;
            Logger = logger;
            Mapper = mapper;
        }
    }
}
