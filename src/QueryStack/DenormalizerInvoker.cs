using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.QueryStack
{
    public class DenormalizerInvoker
    {
        private readonly DevPortalDbContext _dbContext;
        private readonly Type _denormalizerType;
        private readonly IServiceProvider _serviceProvider;

        public DenormalizerInvoker( 
            Type denormalizerType, 
            IServiceProvider serviceProvider,
            DevPortalDbContext dbContext)
        {
            _denormalizerType = denormalizerType;
            _serviceProvider = serviceProvider;
            _dbContext = dbContext;
        }

        public async Task<bool> Invoke(DomainEvent evt)
        {
            object denormalizer = CreateDenormalizerInstance();
            MethodInfo[] handleMethods = GetMethodsToInvoke(evt, denormalizer);

            foreach (var handleMethod in handleMethods)
            {
                var result = handleMethod.Invoke(denormalizer, new[] { evt });
                if (result is Task task) await task;
            }
            return handleMethods.Length > 0;
        }

        protected virtual object CreateDenormalizerInstance()
        {
            return ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, _denormalizerType);
        }

        protected virtual MethodInfo[] GetMethodsToInvoke(DomainEvent evt, object denormalizer)
        {
            bool EventIsAssignableToParameter(MethodInfo method) =>
                method.GetParameters().FirstOrDefault()?.ParameterType.IsAssignableFrom(evt.GetType()) == true;

            return denormalizer.GetType().GetMethods()
                .Where(m => m.Name == "Handle")
                .Where(EventIsAssignableToParameter)
                .ToArray();
        }
    }
}
