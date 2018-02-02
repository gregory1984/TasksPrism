using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Prism.Events.Payloads
{
    public class TaskParticipantsCollectedPayload
    {
        public IList<int> TaskParticipantsIds { get; set; }
    }
}
