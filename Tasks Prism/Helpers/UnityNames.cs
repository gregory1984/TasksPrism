using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Prism.Helpers
{
    public static class UnityNames
    {
        public static string VersionData { get; } = nameof(VersionData);
        public static string CredentialsSerializer { get; } = nameof(CredentialsSerializer);
        public static string ActuallyModifiedTaskId { get; } = nameof(ActuallyModifiedTaskId);
        public static string ModifiedTaskData { get; } = nameof(ModifiedTaskData);
    }
}
