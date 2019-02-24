using Microsoft.Extensions.DependencyInjection;
using NServiceBus.AspNetCore.Tests.TestNsbItems;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NServiceBus.AspNetCore.Tests
{
    public class NsbApplicationBuilderExtensionsTests
    {
        IServiceCollection Services { get; } = new ServiceCollection();

        [Fact]
        public void CanUseEndpointWithNoExtraServices()
        {
            //arrange
            Services.AddTestNsbEndpoint();

            var appBuilder = Services.ToApplicationBuilder();

            //act
            appBuilder.UseNServiceBus();
        }

        [Fact]
        public async Task EndpointUsesILogger()
        {
            //arrange
            Services.AddTestNsbEndpoint();
            var log = Services.AddStringBuilderTestLogging();
            var appBuilder = Services.ToApplicationBuilder();


            //act
            appBuilder.UseNServiceBus();


            //assert
            var nsb = appBuilder.ApplicationServices.GetRequiredService<IMessageSession>();
            await nsb.Send(new TestCommand());

            var finalLog = log.ToString();

            Assert.Contains("NServiceBus.AspNetCore.EndpointConfigurationFactory", finalLog);
            Assert.Contains("NServiceBus.AspNetCore.Tests.TestNsbItems.TestCommand", finalLog);
        }

        [Fact]
        public void CannotAddSameEndpointTwice()
        {
            //arrange
            Services.AddTestNsbEndpoint();
            Services.AddTestNsbEndpoint();

            var appBuilder = Services.ToApplicationBuilder();

            //act/assert
            Assert.Throws<InvalidOperationException>(() => appBuilder.UseNServiceBus());
        }

        [Fact]
        public void MustAddAtLeastOneEndpoint()
        {
            //arrange
            var appBuilder = Services.ToApplicationBuilder();

            //act/assert
            Assert.Throws<InvalidOperationException>(() => appBuilder.UseNServiceBus());
        }

        [Fact]
        public void CanSetPreAndPostConfigurations()
        {
            //arrange
            bool preConfigCalled = false;
            bool configCalled = false;
            bool postConfigCalled = false;

            Services
                .AddTestNsbEndpoint(() =>
                {
                    Assert.True(preConfigCalled);
                    Assert.False(configCalled);
                    Assert.False(postConfigCalled);

                    configCalled = true;
                })
                .AddGlobalEndpointPreConfigurator(x =>
                {
                    Assert.False(preConfigCalled);
                    Assert.False(configCalled);
                    Assert.False(postConfigCalled);

                    preConfigCalled = true;
                })
                .AddGlobalEndpointPostConfigurator(x =>
                {
                    Assert.True(preConfigCalled);
                    Assert.True(configCalled);
                    Assert.False(postConfigCalled);

                    postConfigCalled = true;
                });

            var appBuilder = Services.ToApplicationBuilder();


            //act
            appBuilder.UseNServiceBus();

            //assert
            Assert.True(preConfigCalled);
            Assert.True(configCalled);
            Assert.True(postConfigCalled);
        }
    }
}
