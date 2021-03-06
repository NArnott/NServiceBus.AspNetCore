﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NServiceBus.AspNetCore.Services;
using System;

namespace NServiceBus.AspNetCore
{
    /// <summary>
    /// Adds NSB extension methods to <see cref="IServiceCollection"/>.
    /// </summary>
    public static class NsbServiceCollectionExtensions
    {
        /// <summary>
        /// Adds an NSB Endpoint to the service collection. Can be called multiple times to add multiple endpoints to the microservice. Will use Configuration["NServiceBusLicense"] for contents of License.xml if exists.
        /// </summary>
        /// <param name="services">The service collection to add NSB services to.</param>
        public static NsbBuilder AddNServiceBus(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IMessageSessionProvider, MessageSessionProvider>();
            services.TryAddSingleton(x => x.GetRequiredService<IMessageSessionProvider>().GetMessageSession()); //default IMessageSession provider
            services.TryAddSingleton<IIncomingNsbMessageContextAccessor, IncomingNsbMessageContextAccessor>();
            services.TryAddScoped<INsbContext, NsbContext>();

            return new NsbBuilder(services);
        }
    }
}
