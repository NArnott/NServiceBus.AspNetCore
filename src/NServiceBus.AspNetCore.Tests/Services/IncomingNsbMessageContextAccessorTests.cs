using Moq;
using NServiceBus.AspNetCore.Services;
using System.Threading.Tasks;
using Xunit;

namespace NServiceBus.AspNetCore.Tests.Services
{
    public class IncomingNsbMessageContextAccessorTests
    {
        IncomingNsbMessageContextAccessor Sut { get; } = new IncomingNsbMessageContextAccessor();

        [Fact] 
        public async Task ContextMaintainedAcrossAwaits()
        {
            //arrange
            var context = Mock.Of<IMessageProcessingContext>();
            Sut.Context = context;

            //act
            await Task.Delay(250).ConfigureAwait(false);

            //assert
            Assert.Equal(context, Sut.Context);
        }

        [Fact]
        public async Task ContextMaintainedAcrossThreads()
        {
            //arrange
            var context = Mock.Of<IMessageProcessingContext>();
            Sut.Context = context;

            //act/assert
            await Task.Run(() =>
            {
                Assert.Equal(context, Sut.Context);
            });
        }

    }
}
