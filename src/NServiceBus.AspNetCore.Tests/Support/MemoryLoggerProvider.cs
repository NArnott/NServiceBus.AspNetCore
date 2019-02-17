using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NServiceBus.AspNetCore.Tests.Support
{
    public class MemoryLoggerProvider : ILoggerProvider
    {
        private StringBuilder _sb;

        public MemoryLoggerProvider(StringBuilder sb)
        {
            _sb = sb;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(_sb, categoryName);
        }

        public void Dispose()
        {
            //do nothing
        }

        class Logger : ILogger
        {
            private readonly StringBuilder _sb;
            private readonly string _categoryName;
            private readonly Stack<string> _scopes = new Stack<string>();

            public Logger(StringBuilder sb, string categoryName)
            {
                _sb = sb;
                _categoryName = categoryName;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                _scopes.Push(JsonConvert.SerializeObject(state));
                return new ScopeDisposable(this);
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                string scopes = String.Join(" =>  ", _scopes);
                if (scopes.Length > 0)
                    scopes += "\n";

                _sb.AppendLine($"{_categoryName} {scopes}{logLevel}: {formatter(state, exception)}");
            }

            class ScopeDisposable : IDisposable
            {
                private Logger _logger;

                public ScopeDisposable(Logger logger)
                {
                    _logger = logger;
                }

                public void Dispose()
                {
                    _logger?._scopes.Pop();

                    _logger = null;
                }
            }
        }
    }
}

