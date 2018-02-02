using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Tasks_Prism.Events.Payloads;

namespace Tasks_Prism.Events.TaskSearching
{
    public class UseTaskFiltersEvent : PubSubEvent<UseTaskFiltersPayload>
    {
    }
}
