using Prism.Events;

namespace Tasks_Prism.Events.TaskCreation
{
    public class TaskStatusSelectedEvent : PubSubEvent<string> /* Status name */
    {
    }
}
