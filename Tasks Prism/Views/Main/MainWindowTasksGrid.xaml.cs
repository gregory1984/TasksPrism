using System.Windows.Controls;
using System.Windows;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Tasks_Prism.Views.Administration;
using Tasks_Prism.Views.Preferences;
using Tasks_Prism.Views.Login;
using Tasks_Prism.Views.About;
using Tasks_Prism.Views.Tasks;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Main;

namespace Tasks_Prism.Views.Main
{
    public partial class MainWindowTasksGrid : UserControl
    {
        public MainWindowTasksGrid()
        {
            InitializeComponent();

            var viewmodel = DataContext as MainWindowTasksGridViewModel;

            viewmodel.RemoveSelectedTask += (taskId) =>
            {
                var result = MessageBox.Show($"Czy na pewno usunąć zlecenie nr: {taskId}?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            };

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
