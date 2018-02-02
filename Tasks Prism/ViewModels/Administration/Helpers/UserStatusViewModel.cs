using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Prism.ViewModels.Administration.Helpers
{
    public class UserStatusViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public UserStatusViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
