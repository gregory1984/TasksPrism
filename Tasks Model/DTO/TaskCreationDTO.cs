using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.DTO
{
    public class TaskCreationDTO
    {
        public int? Id { get; set; }
        public string Topic { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime? EndPeriod { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public int? GenreId { get; set; }
        public IList<int> ParticipantsIds { get; set; }
        public IList<TaskCommentCreationDTO> Comments { get; set; }
    }
}
