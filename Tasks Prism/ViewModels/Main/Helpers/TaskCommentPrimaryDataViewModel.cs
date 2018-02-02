using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tasks_Prism.ViewModels.Main.Helpers
{
    public class TaskCommentPrimaryDataViewModel : BindableBase
    {
        public int Id { get; set; }

        private string content = "";
        public string Content
        {
            get { return content; }
            set { SetProperty(ref content, value); }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set { SetProperty(ref date, value); }
        }

        private string author = "";
        public string Author
        {
            get { return author; }
            set { SetProperty(ref author, value); }
        }
    }
}
