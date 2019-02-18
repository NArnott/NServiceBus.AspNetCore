using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NServiceBus.AspNetCore.SampleApp.BusMessages;
using NServiceBus.AspNetCore.SampleApp.Stores;
using NServiceBus.AspNetCore.Services;
using System.Threading.Tasks;

namespace NServiceBus.AspNetCore.SampleApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpPost(nameof(TestNsbCommand))]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> TestNsbCommand(
            [FromServices] INsbContext nsb,
            string testValue, 
            bool crash = false)
        {
            var testCmd = new TestCommand()
            {
                TestValue = testValue,
                Crash = crash,
            };

            await nsb.Send(testCmd);

            return Accepted();
        }

        [HttpGet("LastValue")]
        public string GetLastValue([FromServices] MemoryStore store)
        {
            return store.GetValue();
        }
    }
}
