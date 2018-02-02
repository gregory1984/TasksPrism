using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Tasks_Model.Interfaces;
using Tasks_Prism.Helpers;

namespace Tasks_Prism.ViewModels.Tasks.Helpers
{
    public class TaskParticipantViewModel : BindableBase
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value); }
        }

        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { SetProperty(ref isEnabled, value); }
        }

        private readonly IUserService userService;

        public TaskParticipantViewModel(int id, string username, string name, string surname, bool isChecked, IUserService userService)
        {
            this.userService = userService;

            Id = id;
            Username = username;
            Name = name;
            Surname = surname;
            IsChecked = isChecked;
        }
    }
}
