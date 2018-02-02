using System.Windows.Controls;
using Tasks_Prism.ViewModels.Preferences;
using Tasks_Prism.Helpers;

namespace Tasks_Prism.Views.Preferences
{
    /// <summary>
    /// Interaction logic for PasswordChangingForm
    /// </summary>
    public partial class PasswordChangingForm : UserControl
    {
        public PasswordChangingForm()
        {
            InitializeComponent();

            var viewmodel = DataContext as PasswordChangingFormViewModel;

            viewmodel.PasswordBoxEmpty += () => MessageBoxes.Warning("Hasła nie mogą pozostać puste.");
            viewmodel.CurrentPasswordIncorrect += () => MessageBoxes.Warning("Wartość w polu \"Obecne hasło\" nie jest hasłem używanym przez obecnie zalogowanego użytkownika.");
            viewmodel.NewPasswordsNotSame += () => MessageBoxes.Warning("Wartości w polach \"Nowe hasło\" oraz \"Powtórz nowe hasło\" muszą być identyczne.");
            viewmodel.MinLengthViolation += () => MessageBoxes.Warning("Nowe hasło musi mieć przynajmniej osiem znaków.");
            viewmodel.PasswordChanged += () => MessageBoxes.Information("Hasło zostało pomyślnie zmienione");

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
