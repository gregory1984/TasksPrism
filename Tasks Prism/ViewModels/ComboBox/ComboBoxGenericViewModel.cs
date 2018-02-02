using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tasks_Prism.ViewModels.ComboBox
{
    public class ComboBoxGenericViewModel<T> : BindableBase
    {
        private IList<T> items;
        public IList<T> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        private T selectedItem;
        public T SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
    }
}
