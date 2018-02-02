using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks_Model.DTO;

namespace Tasks_Model.Interfaces
{
    public interface IPreferencesService
    {
        UserPreferencesDTO Preferences { get; }

        void SavePreferences(UserPreferencesDTO preferences);
        void LoadPreferences(int userId);
        void Reset(int userId);
    }
}
