using System.Windows;
using MahApps.Metro.Controls;
using Tasks_Prism.ViewModels.About;

namespace Tasks_Prism.Views.About
{
    public partial class AboutWindow : MetroWindow
    {
        public AboutWindow()
        {
            InitializeComponent();

            var viewmodel = DataContext as AboutWindowViewModel;
            viewmodel.CloseAboutWindow += () => Close();
        }
    }
}
