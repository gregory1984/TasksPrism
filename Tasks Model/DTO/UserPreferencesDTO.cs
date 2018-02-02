using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks_Model.Entities;

namespace Tasks_Model.DTO
{
    public class UserPreferencesDTO
    {
        public int UserId { get; set; }
        public int AutoRefreshingFrequency { get; set; }
        public int TasksPerPage { get; set; }
        public bool DisplayOnlyTasksWithMyParticipation { get; set; }
        public bool EnableTasksListAutoRefreshing { get; set; }
        public bool DisplayTasksRefreshingProgressBar { get; set; }
        public bool HideCanceledTasks { get; set; }

        public UserPreferencesDTO() { }
        public UserPreferencesDTO(int userId, int autoRefreshingFrequency, int tasksPerPage, bool displayOnlyTasksWithMyParticipation, bool enableTasksListAutoRefreshing, bool displayTasksRefreshingProgressBar, bool hideCanceledTasks)
        {
            UserId = userId;
            AutoRefreshingFrequency = autoRefreshingFrequency;
            TasksPerPage = tasksPerPage;
            DisplayOnlyTasksWithMyParticipation = displayOnlyTasksWithMyParticipation;
            EnableTasksListAutoRefreshing = enableTasksListAutoRefreshing;
            DisplayTasksRefreshingProgressBar = displayTasksRefreshingProgressBar;
            HideCanceledTasks = hideCanceledTasks;
        }

        public UserPreferencesDTO(UserPreference preferences)
        {
            UserId = preferences.User.Id;
            AutoRefreshingFrequency = preferences.AutoRefreshingFrequency;
            TasksPerPage = preferences.TasksPerPage;
            DisplayOnlyTasksWithMyParticipation = preferences.DisplayOnlyTasksWithMyParticipation;
            EnableTasksListAutoRefreshing = preferences.EnableTasksListAutoRefreshing;
            DisplayTasksRefreshingProgressBar = preferences.DisplayTasksRefreshingProgressBar;
            HideCanceledTasks = preferences.HideCanceledTasks;
        }
    }
}
