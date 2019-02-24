using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus.AspNetCore.Tests.Support;
using NServiceBus.AspNetCore.Tests.TestNsbItems;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace NServiceBus.AspNetCore.Tests
{
    static class HelperMethods
    {
        public static IApplicationBuilder ToApplicationBuilder(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            var mock = new Mock<IApplicationBuilder>();

            mock.SetupGet(x => x.ApplicationServices).Returns(serviceProvider);

            return mock.Object;
        }

        public static NsbBuilder AddTestNsbEndpoint(
            this IServiceCollection services,
            Action configCalled = null,
            [CallerMemberName] string callerName = null
            )
        {
            var builder = services.AddNServiceBus().AddNsbEndpoint(callerName, x => x.ApplyTestConfigs(callerName, configCalled));

            return builder;
        }

        public static void ApplyTestConfigs(this EndpointConfiguration endpointConfiguration, string endpointName, Action configCalled)
        {
            configCalled?.Invoke();

            var transport = endpointConfiguration.UseTransport<LearningTransport>();

            transport.Routing().RouteToEndpoint(typeof(TestCommand), endpointName);
        }

        public static StringBuilder AddStringBuilderTestLogging(this IServiceCollection services)
        {
            var sb = new StringBuilder();

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddProvider(new MemoryLoggerProvider(sb));
            });

            return sb;
        }
    }
}
