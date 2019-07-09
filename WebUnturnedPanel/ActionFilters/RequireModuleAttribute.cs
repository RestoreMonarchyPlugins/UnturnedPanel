using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace RestoreMonarchy.WebUnturnedPanel.ActionFilters
{
    public class RequireModuleAttribute : ActionFilterAttribute
    {
        private readonly string moduleName;
        private readonly IConfiguration configuration;

        public RequireModuleAttribute(string moduleName, IConfiguration configuration)
        {
            this.moduleName = moduleName;
            this.configuration = configuration;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!configuration.GetSection("Modules").GetValue<bool>(moduleName))
            {
                context.Result = new NotFoundResult();
            }
        }
    }
}
