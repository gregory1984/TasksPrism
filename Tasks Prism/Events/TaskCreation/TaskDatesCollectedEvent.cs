using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks_Prism.Events.Payloads;
using Prism.Events;

namespace Tasks_Prism.Events.TaskCreation
{
    public class TaskDatesCollectedEvent : PubSubEvent<TaskDatesCollectedPayload>
    {
    }
}
