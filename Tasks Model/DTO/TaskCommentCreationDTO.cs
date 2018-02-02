using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.DTO
{
    public class TaskCommentCreationDTO
    {
        public int? Id { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int AuthorId { get; set; }
        public string AuthorUsername { get; set; }
        public string AuthorName { get; set; }
        public string AuthorSurname { get; set; }
        public int? TaskId { get; set; }

        public TaskCommentCreationDTO() { }

        public TaskCommentCreationDTO(int? id, string content, DateTime date, int authorId, string authorUsername, int? taskId)
        {
            Id = id;
            Content = content;
            Date = date;
            AuthorId = authorId;
            AuthorUsername = authorUsername;
            TaskId = taskId;
        }
    }
}
