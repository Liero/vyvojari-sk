using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DevPortal.CommandStack.Infrastructure;
using Rebus.Bus;
using Rebus.Exceptions;
using Rebus.Logging;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;
// ReSharper disable ForCanBeConvertedToForeach

namespace DevPortal.Web.AppCode.EventSourcing
{
    /// <summary>
    /// Incoming step that gets a <see cref="List{T}"/> where T is <see cref="HandlerInvoker"/> from the context
    /// and invokes them in the order they're in.
    /// </summary>
    [StepDocumentation(@"Gets all the handler invokers from the current context and invokes them in order.

Please note that each invoker might choose to ignore the invocation internally.

If no invokers were found, a RebusApplicationException is thrown.")]
    public class MyDispatchIncomingMessageStep : IIncomingStep
    {
        readonly ILog _log;
        readonly IHandlerNotifications _eventListener;

        /// <summary>
        /// Creates the step
        /// </summary>
        public MyDispatchIncomingMessageStep(IHandlerNotifications eventListener, IRebusLoggerFactory rebusLoggerFactory)
        {
            if (rebusLoggerFactory == null) throw new ArgumentNullException(nameof(rebusLoggerFactory));
            _log = rebusLoggerFactory.GetLogger<DispatchIncomingMessageStep>();
            _eventListener = eventListener;
        }

        /// <summary>
        /// Keys of an <see cref="IncomingStepContext"/> items that indicates that message dispatch must be stopped
        /// </summary>
        public const string AbortDispatchContextKey = "abort-dispatch-to-handlers";

        /// <summary>
        /// Processes the message
        /// </summary>
        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            var invokers = context.Load<HandlerInvokers>();
            var handlersInvoked = 0;
            var message = invokers.Message;

            var messageId = message.GetMessageId();
            var messageType = message.GetMessageType();

            // if dispatch has already been aborted (e.g. in a transport message filter or something else that
            // was run before us....) bail out here:
            if (context.Load<bool>(AbortDispatchContextKey))
            {
                _log.Debug("Skipping dispatch of message {messageType} {messageId}", messageType, messageId);
                await next().ConfigureAwait(false);
                return;
            }

            var stopwatch = Stopwatch.StartNew();

            int index = 0;
            foreach (var invoker in invokers)
            {
                try
                {
                    await invoker.Invoke().ConfigureAwait(false);
                    handlersInvoked++;
                }
                catch(Exception ex)
                {
                    throw;
                }
                finally
                {
                    if (message.Body is DomainEvent evt)
                    {
                        _eventListener.SetHandled(invoker.Handler.GetType(), evt.Id);
                    }
                }
               
                // if dispatch was aborted at this point, bail out
                if (context.Load<bool>(AbortDispatchContextKey))
                {
                    _log.Debug("Skipping further dispatch of message {messageType} {messageId}", messageType, messageId);
                    break;
                }

                index++;
            }

            // throw error if we should have executed a handler but we didn't
            if (handlersInvoked == 0)
            {
                var text = $"Message with ID {messageId} and type {messageType} could not be dispatched to any handlers (and will not be retried under the default fail-fast settings)";

                throw new RebusApplicationException(text);
            }

            _log.Debug("Dispatching {messageType} {messageId} to {count} handlers took {elapsedMs:0} ms",
                messageType, messageId, handlersInvoked, stopwatch.ElapsedMilliseconds);

            await next().ConfigureAwait(false);
        }
    }
}