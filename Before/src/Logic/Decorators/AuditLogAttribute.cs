using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Decorators
{
    /// <summary>
    /// Is intended to be used on command handlers only!
    /// Is used only to mark classes which want the AuditLoggingDecorator logic to be executed in the end.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class AuditLogAttribute : Attribute
    {

    }
}
