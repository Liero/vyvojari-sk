﻿// modified from: https://github.com/rebus-org/Rebus/blob/master/Rebus/Pipeline/Receive/ActivateHandlersStep.cs

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Extensions;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;
using Rebus.Transport;
// ReSharper disable UnusedMethodReturnValue.Local

namespace DevPortal.Web.AppCode.Extensions
{
    /// <summary>
    /// Incoming message step that gets relevant handlers for the message
    /// </summary>
    [StepDocumentation(@"Looks at the incoming message and decides how to handle it. A HandlerInvokers object is saved to the context to be invoked later.")]
    public class MyActivateHandlersStep : IIncomingStep
    {
        readonly ConcurrentDictionary<Type, MethodInfo> _dispatchMethods = new ConcurrentDictionary<Type, MethodInfo>();
        readonly IHandlerActivator _handlerActivator;

        /// <summary>
        /// Constructs the step with the <see cref="IHandlerActivator"/> to use to get the handler instances
        /// </summary>
        public MyActivateHandlersStep(IHandlerActivator handlerActivator)
        {
            _handlerActivator = handlerActivator;
        }

        /// <summary>
        /// Looks up handlers for the incoming message and saves the handlers (without invoking them) to the context as a <see cref="HandlerInvokers"/>
        /// </summary>
        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            var transactionContext = context.Load<ITransactionContext>();
            var message = context.Load<Message>();
            var body = message.Body;
            var messageType = body.GetType();
            var methodToInvoke = _dispatchMethods
                .GetOrAdd(messageType, type => GetDispatchMethod(messageType));

            var handlerInvokers = await ((Task<HandlerInvokers>)methodToInvoke.Invoke(this, new[] { body, transactionContext, message })).ConfigureAwait(false);

            context.Save(handlerInvokers);

            await next().ConfigureAwait(false);
        }

        async Task<HandlerInvokers> GetHandlerInvokers<TMessage>(TMessage message, ITransactionContext transactionContext, Message logicalMessage)
        {
            var handlers = await _handlerActivator.GetHandlers(message, transactionContext).ConfigureAwait(false);

            var listOfHandlerInvokers = handlers
                .Select(handler => new MyHandlerInvoker<TMessage>(() => handler.Handle(message), handler, transactionContext))
                .Cast<HandlerInvoker>()
                .ToList();

            return new HandlerInvokers(logicalMessage, listOfHandlerInvokers);
        }

        MethodInfo GetDispatchMethod(Type messageType)
        {
            return GetType()
                .GetMethod(nameof(GetHandlerInvokers), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(messageType);
        }
    }
}