using System;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Newtonsoft.Json.Serialization;

namespace KernelPanic.Serialization
{
    // Taken from https://www.newtonsoft.com/json/help/html/DeserializeWithDependencyInjection.htm.
    internal sealed class AutofacContractResolver : DefaultContractResolver
    {
        private readonly IContainer mContainer;

        public AutofacContractResolver(IContainer container)
        {
            mContainer = container;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            // use Autofac to create types that have been registered with it
            if (mContainer.IsRegistered(objectType))
            {
                JsonObjectContract contract = ResolveContact(objectType);
                contract.DefaultCreator = () => mContainer.Resolve(objectType);

                return contract;
            }

            return base.CreateObjectContract(objectType);
        }

        private JsonObjectContract ResolveContact(Type objectType)
        {
            // attempt to create the contact from the resolved type
            if (mContainer.ComponentRegistry.TryGetRegistration(new TypedService(objectType), out var registration))
            {
                Type viewType = (registration.Activator as ReflectionActivator)?.LimitType;
                if (viewType != null)
                {
                    return base.CreateObjectContract(viewType);
                }
            }

            // fall back to using the registered type
            return base.CreateObjectContract(objectType);
        }
    }
}