using System;
using Prism.Events;

namespace Tasks_Prism.Events
{
    public class ExceptionOccuredEvent : PubSubEvent<Exception> { }
}
