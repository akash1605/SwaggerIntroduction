using Microsoft.AspNetCore.Mvc.Filters;

namespace SwaggerIntroduction.Security
{
    public class Validation: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
