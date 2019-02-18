namespace NServiceBus.AspNetCore.SampleApp.BusMessages
{
    public class TestCommand : ICommand
    {
        public string TestValue { get; set; }

        public bool Crash { get; set; }
    }
}
