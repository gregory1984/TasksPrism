using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Tasks_Prism.ViewModels.ComboBox
{
    public class ComboBoxItem
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public ComboBoxItem(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ComboBoxViewModel : BindableBase
    {
        private IList<ComboBoxItem> items;
        public IList<ComboBoxItem> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        private ComboBoxItem selectedItem;
        public ComboBoxItem SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
    }
}
