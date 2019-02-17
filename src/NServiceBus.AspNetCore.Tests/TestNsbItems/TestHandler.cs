using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.AspNetCore.Tests.TestNsbItems
{
    public class TestHandler : IHandleMessages<TestCommand>
    {
        public Task Handle(TestCommand message, IMessageHandlerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
