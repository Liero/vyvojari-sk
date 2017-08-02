using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Infrastructure
{
    public interface IHandleMessages<in T>
    {
        void Handle(T message);
    }
}
