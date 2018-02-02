using System.Windows.Controls;
using Tasks_Prism.ViewModels.Preferences;

namespace Tasks_Prism.Views.Preferences
{
    /// <summary>
    /// Interaction logic for PreferencesForm
    /// </summary>
    public partial class PreferencesForm : UserControl
    {
        public PreferencesForm()
        {
            InitializeComponent();

            var viewmodel = DataContext as PreferencesFormViewModel;

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
