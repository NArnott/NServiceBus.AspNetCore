namespace NServiceBus.AspNetCore.SampleApp.BusMessages
{
    public class TestValueChangedEvent : IEvent
    {
        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }
}
