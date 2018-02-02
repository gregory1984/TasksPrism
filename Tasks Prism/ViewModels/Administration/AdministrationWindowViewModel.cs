using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Commands;
using Tasks_Prism.Events;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Base;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;

namespace Tasks_Prism.ViewModels.Administration
{
    public class AdministrationWindowViewModel : ViewModelBase
    {
        #region Properties
        private string windowTitle = "";
        public string WindowTitle
        {
            get => windowTitle;
            set => SetProperty(ref windowTitle, value);
        }
        #endregion

        #region Delegates
        public delegate void CloseAdministrationWindowDelegate();
        public event CloseAdministrationWindowDelegate CloseAdministrationWindow;
        #endregion

        #region Event tokens
        private SubscriptionToken closeAdministrationWindowToken;
        #endregion

        public AdministrationWindowViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService)
            : base(eventAggregator, unityContainer, userService)
        {

        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                closeAdministrationWindowToken = eventAggregator.GetEvent<CloseAdministrationWindowEvent>()
                    .Subscribe(() => CloseAdministrationWindow?.Invoke());

                eventAggregator.ExecuteSafety(() =>
                {
                    SetWindowTitle();
                });
            }));
        }

        private void SetWindowTitle()
            => WindowTitle = "Administracja";

        public override void UnsubscribePrismEvents()
        {
            eventAggregator.GetEvent<CloseAdministrationWindowEvent>().Unsubscribe(closeAdministrationWindowToken);
        }
    }
}
