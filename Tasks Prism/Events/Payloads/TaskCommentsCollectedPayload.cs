using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks_Model.DTO;

namespace Tasks_Prism.Events.Payloads
{
    public class TaskCommentsCollectedPayload
    {
        public IList<TaskCommentPrimaryDataDTO> Comments { get; set; }
    }
}
