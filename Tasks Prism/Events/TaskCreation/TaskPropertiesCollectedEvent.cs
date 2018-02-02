using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Tasks_Prism.Events.Payloads;

namespace Tasks_Prism.Events.TaskCreation
{
    public class TaskPropertiesCollectedEvent : PubSubEvent<TaskPropertiesCollectedPayload>
    {
    }
}
