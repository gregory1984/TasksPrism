using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.DTO
{
    public class TaskCommentPrimaryDataDTO
    {
        public int? Id { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int? AuthorId { get; set; }
        public string Author { get; set; }

        public TaskCommentPrimaryDataDTO() { }

        //  For last comment inside main task list.
        public TaskCommentPrimaryDataDTO(int? id, string content, DateTime date, string author)
        {
            Id = id;
            Content = content;
            Date = date;
            AuthorId = null;
            Author = author;
        }

        //  For displaying all comments of the selected task.
        public TaskCommentPrimaryDataDTO(int? id, string content, DateTime date, int authorId, string author)
        {
            Id = id;
            Content = content;
            Date = date;
            AuthorId = authorId;
            Author = author;
        }

        public override string ToString()
        {
            return Content;
        }
    }
}
