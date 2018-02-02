using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Tasks_Prism.Events;

namespace Tasks_Prism.ViewModels.Administration.Helpers
{
    public class UserRoleViewModel : BindableBase
    {
        public int Id { get; set; }

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

        public bool IsSystemRole { get; set; }
        public string Name { get; set; }

        public UserRoleViewModel(int id, bool isChecked, bool isSystemRole, string name)
        {
            Id = id;
            IsChecked = isChecked;
            IsSystemRole = isSystemRole;
            Name = name;
            IsEnabled = false;
        }
    }
}
