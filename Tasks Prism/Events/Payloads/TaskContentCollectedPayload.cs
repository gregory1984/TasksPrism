using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Prism.Events.Payloads
{
    public class TaskContentCollectedPayload
    {
        public string Topic { get; set; }
        public string Content { get; set; }
    }
}
