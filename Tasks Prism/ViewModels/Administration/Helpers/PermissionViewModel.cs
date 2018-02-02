using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Tasks_Prism.Events;

namespace Tasks_Prism.ViewModels.Administration.Helpers
{
    public class PermissionViewModel : BindableBase
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

        public string Name { get; set; }

        private readonly IEventAggregator eventAggregator;

        public PermissionViewModel(int id, bool isChecked, string name, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            Id = id;
            IsChecked = isChecked;
            Name = name;
            IsEnabled = true;
        }
    }
}
