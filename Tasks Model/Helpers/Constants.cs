using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.Helpers
{
    public static class Constants
    {
        public static string AssemblyName { get; } = "Tasks Prism.exe";

        public static string UserRoleName { get; } = "Użytkownik";
        public static string AdminRoleName { get; } = "Administrator";

        public static string AdminUsername { get; } = "administrator";
        public static string AdminPassword { get; } = AdminUsername;

        public static int MinPasswordLength = 8;
        public static string Migration_UsersCSV = "Migration/users.csv";

        public static string UserStatus_Blocked { get; } = "Zablokowany";
        public static string UserStatus_Active { get; } = "Aktywny";

        public static string TaskStatus_Canceled { get; } = "Anulowane";
        public static string TaskStatus_Opened { get; } = "Otwarte";
        public static string TaskStatus_Closed { get; } = "Zamknięte";

        public static string TaskPriority_Low { get; } = "Niski";
        public static string TaskPriority_Normal { get; } = "Normalny";
        public static string TaskPriority_High { get; } = "Wysoki";

        public static string TaskGenre_Update { get; } = "Aktualizacja";
        public static string TaskGenre_Installation { get; } = "Instalacja";
        public static string TaskGenre_Toner { get; } = "Tonery";
        public static string TaskGenre_Task { get; } = "Zlecenie";
    }
}
