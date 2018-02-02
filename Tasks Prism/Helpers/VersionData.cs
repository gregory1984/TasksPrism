using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Prism.Helpers
{
    public class VersionData
    {
        private string versionNumber;
        public string VersionNumber
        {
            get => versionNumber;
        }

        private string compilationMarker;
        public string CompilationMarker
        {
            get => compilationMarker;
        }

        private IList<string> technologies;
        public string Technologies
        {
            get => string.Join(", ", technologies);
        }

        private IList<string> authorsData;
        public string AuthorsData
        {
            get => string.Join("\n", authorsData);
        }

        public VersionData()
        {
            versionNumber = "5.01";
            compilationMarker = DateTime.Now.ToString();

            technologies = new List<string>
            {
                "C#", "WPF", "Prism", "MySQL", "NHibernate"
            };

            authorsData = new List<string>
            {
                "Grzegorz Matławski", "Dolnośląskie/Wrocław", "grzegorz.matlawski@gmail.com"
            };
        }
    }
}
