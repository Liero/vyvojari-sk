using Rebus.Pipeline.Receive;
using Rebus.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Extensions
{
    public class MyHandlerInvoker<TMessage> : HandlerInvoker<TMessage>
    {
        public MyHandlerInvoker(Func<Task> action, object handler, ITransactionContext transactionContext)
            : base(action, handler, transactionContext)
        {

        }
    }
}
