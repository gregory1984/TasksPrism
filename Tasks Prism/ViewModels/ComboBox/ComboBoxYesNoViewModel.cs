using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Tasks_Prism.ViewModels.ComboBox
{
    public enum YesNoSelection
    {
        Yes,
        No
    }

    public class ComboBoxYesNoItem
    {
        public YesNoSelection Selection { get; set; }

        public ComboBoxYesNoItem(YesNoSelection selection)
        {
            Selection = selection;
        }

        public override string ToString()
        {
            return Selection == YesNoSelection.Yes ? "Tak" : "Nie";
        }
    }

    public class ComboBoxYesNoViewModel : BindableBase
    {
        private IList<ComboBoxYesNoItem> items;
        public IList<ComboBoxYesNoItem> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        private ComboBoxYesNoItem selectedItem;
        public ComboBoxYesNoItem SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        public ComboBoxYesNoViewModel()
        {
            Items = new ObservableCollection<ComboBoxYesNoItem>
            {
                new ComboBoxYesNoItem(YesNoSelection.Yes),
                new ComboBoxYesNoItem(YesNoSelection.No)
            };
        }

        public void SetSelection(bool isYes)
        {
            SelectedItem = isYes ? Items.First() : Items.Last();
        }

        public bool SelectedToBool()
        {
            return SelectedItem.Selection == YesNoSelection.Yes ? true : false;
        }
    }
}
