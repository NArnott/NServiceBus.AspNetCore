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
            appBuilder.UseNServiceBusEndpoints();
        }

        [Fact]
        public async Task EndpointUsesILogger()
        {
            //arrange
            Services.AddTestNsbEndpoint();
            var log = Services.AddStringBuilderTestLogging();
            var appBuilder = Services.ToApplicationBuilder();


            //act
            appBuilder.UseNServiceBusEndpoints();


            //assert
            var nsb = appBuilder.ApplicationServices.GetRequiredService<IMessageSession>();
            await nsb.Send(new TestCommand());

            var finalLog = log.ToString();

            Assert.Contains("NServiceBus.AspNetCore.NsbEndpointConfigFactory", finalLog);
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
            Assert.Throws<InvalidOperationException>(() => appBuilder.UseNServiceBusEndpoints());
        }

        [Fact]
        public void MustAddAtLeastOneEndpoint()
        {
            //arrange
            var appBuilder = Services.ToApplicationBuilder();

            //act/assert
            Assert.Throws<InvalidOperationException>(() => appBuilder.UseNServiceBusEndpoints());
        }
    }
}
