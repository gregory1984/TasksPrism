using System.Windows;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Administration;

namespace Tasks_Prism.Views.Administration
{
    public partial class AdministrationWindow : MetroWindow
    {
        public AdministrationWindow()
        {
            InitializeComponent();

            var viewmodel = DataContext as AdministrationWindowViewModel;
            viewmodel.CloseAdministrationWindow += () => Close();

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
