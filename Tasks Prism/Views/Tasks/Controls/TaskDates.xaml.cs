using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Tasks_Prism.Views.Administration;
using Tasks_Prism.Views.Preferences;
using Tasks_Prism.Views.Login;
using Tasks_Prism.Views.About;
using Tasks_Prism.Views.Tasks;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Main;
using Tasks_Prism.ViewModels.Tasks.Controls;

namespace Tasks_Prism.Views.Tasks.Controls
{
    public partial class TaskDates : UserControl
    {
        public TaskDates()
        {
            InitializeComponent();

            var viewmodel = DataContext as TaskDatesViewModel;

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
