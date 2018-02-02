using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace Tasks_Prism.Events
{
    public class RemoveSelectedTaskEvent : PubSubEvent<int> /* Selected task Id */
    {
    }
}
