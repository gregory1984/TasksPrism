using System;
using System.Windows.Controls;
using System.Threading;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Commands;
using Tasks_Prism.Events;
using Tasks_Prism.ViewModels.Base;
using Tasks_Prism.Helpers;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;

namespace Tasks_Prism.ViewModels.About
{
    public class AboutWindowViewModel : ViewModelBase
    {
        #region Properties
        private string versionNumber = "";
        public string VersionNumber
        {
            get => versionNumber;
            set => SetProperty(ref versionNumber, value);
        }

        private string compilationMarker = "";
        public string CompilationMarker
        {
            get => compilationMarker;
            set => SetProperty(ref compilationMarker, value);
        }

        private string technologies = "";
        public string Technologies
        {
            get => technologies;
            set => SetProperty(ref technologies, value);
        }

        private string author = "";
        public string Author
        {
            get => author;
            set => SetProperty(ref author, value);
        }
        #endregion

        #region Delegates
        public delegate void CloseAboutWindowDelegate();
        public event CloseAboutWindowDelegate CloseAboutWindow;
        #endregion

        public AboutWindowViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService)
            : base(eventAggregator, unityContainer, userService)
        {

        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                var versionData = unityContainer.Resolve<VersionData>(UnityNames.VersionData);
                VersionNumber = versionData.VersionNumber;
                CompilationMarker = versionData.CompilationMarker;
                Technologies = versionData.Technologies;
                Author = versionData.AuthorsData;
            }));
        }

        private DelegateCommand close;
        public DelegateCommand Close
        {
            get => close ?? (close = new DelegateCommand(() => CloseAboutWindow?.Invoke()));
        }
    }
}
