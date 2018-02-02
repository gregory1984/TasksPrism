using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace Tasks_Prism.Helpers
{
    public static class MessageBoxes
    {
        public static void Information(string message, string caption = "")
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void Warning(string message, string caption = "")
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void Critical(string message, string caption = "")
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void CriticalQuestion(string error, string message = "Wystąpił błąd kryczyny. Czy chcesz zobaczyć szczegóły?", string caption = "Błąd krytyczny")
        {
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Error);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        var path = "Logs/logs.txt";
                        File.WriteAllText(path, error);
                        var notepad = new Process();
                        notepad.StartInfo = new ProcessStartInfo("notepad.exe", path);
                        notepad.Start();
                        break;
                    }
            }
        }

        public static void Question(string message, Action yesAction, Action noAction = null, string caption = "")
        {
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        yesAction?.Invoke();
                        break;
                    }
                case MessageBoxResult.No:
                    {
                        noAction?.Invoke();
                        break;
                    }
            }
        }
    }
}
