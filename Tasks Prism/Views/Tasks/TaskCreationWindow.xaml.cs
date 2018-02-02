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
using Tasks_Prism.ViewModels.Tasks;

namespace Tasks_Prism.Views.Tasks
{
    public partial class TaskCreationWindow : MetroWindow
    {
        public TaskCreationWindow()
        {
            InitializeComponent();

            var viewmodel = DataContext as TaskCreationWindowViewModel;

            viewmodel.TaskGenreNotSelected += () => MessageBoxes.Warning("Nie wybrano rodzaju zlecenia.");
            viewmodel.TaskPriorityNotSelected += () => MessageBoxes.Warning("Nie wybrano priorytetu zlecenia.");
            viewmodel.TaskStatusNotSelected += () => MessageBoxes.Warning("Nie wybrano status zlecenia.");
            viewmodel.TaskParticipantsNotSelected += () => MessageBoxes.Warning("Nie wybrano uczestników zlecenia.");
            viewmodel.TopicOrContentEmpty += () => MessageBoxes.Warning("Nie wypełniono treści oraz tematu zlecenia.");
            viewmodel.ClosedOrCanceledWithNoEndPeriod += () => MessageBoxes.Warning("Zlecenie zamknięte lub " +
                "anulowane musi posiadać wypełnioną datę oraz godzinę zakończenia.");
            viewmodel.EndPeriodEarlierThanStartPeriod += () => MessageBoxes.Warning("Data zakończenia zlecenia jest " +
                "wcześniejsza niż data otwarcia.");

            viewmodel.TaskModified += (taskId) =>
            {
                MessageBoxes.Information($"Zlecenie '{taskId}' zostało pomyślnie zmienone.");
                Close();
            };

            viewmodel.TaskAdded += () =>
            {
                MessageBoxes.Information("Zlecenie zostało pomyślnie dodane do systemu.");
                Close();
            };

            viewmodel.CloseTaskCreationWindow += () => Close();

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
