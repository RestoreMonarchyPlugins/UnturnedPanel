using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestoreMonarchy.WebUnturnedPanel.ActionFilters
{
    public class RequirePermissionAttribute : ActionFilterAttribute
    {
        private readonly IConfiguration configuration;
        public RequirePermissionAttribute(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
    }
}
