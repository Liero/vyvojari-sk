using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Authorization.Handlers
{
    public static class HandlerRegistrationServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthorizationHandlers(
            this IServiceCollection services,
            Assembly assembly,
            ServiceLifetime serviceLifetime)
        {
            var handlerFromAssembly = assembly.GetTypes()
                .Where(t => !t.IsAbstract && t.IsClass)
                .Where(t => typeof(IAuthorizationHandler).IsAssignableFrom(t));

            foreach(var handler in handlerFromAssembly)
            {
                services.Add(new ServiceDescriptor(
                    typeof(IAuthorizationHandler), 
                    handler, 
                    serviceLifetime));

            }
            return services;
        }
    }
}
