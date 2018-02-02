using Prism.Events;
using Tasks_Prism.Events.Payloads;

namespace Tasks_Prism.Events.TaskCreation
{
    public class TaskContentCollectedEvent : PubSubEvent<TaskContentCollectedPayload>
    {
    }
}
