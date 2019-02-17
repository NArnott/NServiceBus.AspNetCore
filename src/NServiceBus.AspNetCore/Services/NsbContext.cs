using NServiceBus.Extensibility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NServiceBus.AspNetCore.Services
{
    class NsbContext : INsbContext
    {
        private readonly IPipelineContext _realPiplineContext;

        public NsbContext(IMessageSessionProvider messageSessionProvider, IIncomingNsbMessageContextAccessor pipelineContextAccessor)
        {
            _realPiplineContext = pipelineContextAccessor.Context
                ?? (IPipelineContext)new MessageSessionWrapper(messageSessionProvider.GetMessageSession())
                ?? throw new InvalidOperationException("Could not locate a Nsb Context.");
        }

        IMessageProcessingContext MessageContext => (_realPiplineContext as IMessageProcessingContext) ?? throw new InvalidOperationException("Current NSB Context is not for an incoming message. IMessageProcessingContext specific items are not allowed.");


        ContextBag IExtendable.Extensions => _realPiplineContext.Extensions;

        #region IPipelineContext

        Task IPipelineContext.Publish(object message, PublishOptions options)
        {
            return _realPiplineContext.Publish(message, options);
        }

        Task IPipelineContext.Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions)
        {
            return _realPiplineContext.Publish(messageConstructor, publishOptions);
        }

        Task IPipelineContext.Send(object message, SendOptions options)
        {
            return _realPiplineContext.Send(message, options);
        }

        Task IPipelineContext.Send<T>(Action<T> messageConstructor, SendOptions options)
        {
            return _realPiplineContext.Send(messageConstructor, options);
        }

        #endregion

        #region IMessageProcessingContext

        Task IMessageProcessingContext.Reply(object message, ReplyOptions options)
        {
            return MessageContext.Reply(message, options);
        }

        Task IMessageProcessingContext.Reply<T>(Action<T> messageConstructor, ReplyOptions options)
        {
            return MessageContext.Reply(messageConstructor, options);
        }

        string IMessageProcessingContext.MessageId => MessageContext.MessageId;

        string IMessageProcessingContext.ReplyToAddress => MessageContext.ReplyToAddress;

        IReadOnlyDictionary<string, string> IMessageProcessingContext.MessageHeaders => MessageContext.MessageHeaders;

        Task IMessageProcessingContext.ForwardCurrentMessageTo(string destination)
        {
            return MessageContext.ForwardCurrentMessageTo(destination);
        }

        #endregion

        #region IMessageSessionWrapper

        class MessageSessionWrapper : IPipelineContext
        {
            private readonly IMessageSession _messageSession;

            public MessageSessionWrapper(IMessageSession messageSession)
            {
                _messageSession = messageSession;
            }

            ContextBag IExtendable.Extensions => throw new NotSupportedException();

            Task IPipelineContext.Publish(object message, PublishOptions options)
            {
                return _messageSession.Publish(message, options);
            }

            Task IPipelineContext.Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions)
            {
                return _messageSession.Publish(messageConstructor, publishOptions);
            }

            Task IPipelineContext.Send(object message, SendOptions options)
            {
                return _messageSession.Send(message, options);
            }

            Task IPipelineContext.Send<T>(Action<T> messageConstructor, SendOptions options)
            {
                return _messageSession.Send(messageConstructor, options);
            }
        }

        #endregion
    }
}
