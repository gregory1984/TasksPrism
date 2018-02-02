using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.DTO
{
    public class TaskPrimaryDataDTO
    {
        public int? Id { get; set; }
        public string Topic { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime? EndPeriod { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Genre { get; set; }
        public TaskCommentPrimaryDataDTO LastComment { get; set; }

        public TaskPrimaryDataDTO() { }

        public TaskPrimaryDataDTO(int? id, string topic, string content, DateTime startPeriod, DateTime? endPeriod,
            int? lastCommentId, string lastCommentContent, DateTime lastCommentDate, string lastCommentAuthor,
            string status, string priority, string genre, string author)
        {
            Id = id;
            Topic = topic;
            Content = content;
            StartPeriod = startPeriod;
            EndPeriod = endPeriod;
            Status = status;
            Priority = priority;
            Genre = genre;
            Author = author;

            LastComment = lastCommentId.HasValue ? new TaskCommentPrimaryDataDTO(lastCommentId, lastCommentContent, lastCommentDate, lastCommentAuthor) : null;
        }
    }
}
