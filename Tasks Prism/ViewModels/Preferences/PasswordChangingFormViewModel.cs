using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
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

namespace Tasks_Prism.ViewModels.Preferences
{
    public class PasswordChangingFormViewModel : ViewModelBase
    {
        #region Delegates
        public delegate void PasswordBoxEmptyDelegate();
        public event PasswordBoxEmptyDelegate PasswordBoxEmpty;

        public delegate void CurrentPasswordIncorrectDelegate();
        public event CurrentPasswordIncorrectDelegate CurrentPasswordIncorrect;

        public delegate void NewPasswordsNotSameDelegate();
        public event NewPasswordsNotSameDelegate NewPasswordsNotSame;

        public delegate void PasswordChangedDelegate();
        public event PasswordChangedDelegate PasswordChanged;

        public delegate void MinLengthViolationDelegate();
        public event MinLengthViolationDelegate MinLengthViolation;
        #endregion


        public PasswordChangingFormViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService)
            : base(eventAggregator, unityContainer, userService)
        {
        }

        private DelegateCommand<IList<PasswordBox>> changePassword;
        public DelegateCommand<IList<PasswordBox>> ChangePassword
        {
            get => changePassword ?? (changePassword = new DelegateCommand<IList<PasswordBox>>(p =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    switch (userService.ChangePassword(p[0].Password, p[1].Password, p[2].Password))
                    {
                        case PasswordChangingStatus.Changed: { PasswordChanged?.Invoke(); break; }
                        case PasswordChangingStatus.CurrentPasswordIncorrect: { CurrentPasswordIncorrect?.Invoke(); break; }
                        case PasswordChangingStatus.MinLengthViolation: { MinLengthViolation?.Invoke(); break; }
                        case PasswordChangingStatus.NewPasswordsNotSame: { NewPasswordsNotSame?.Invoke(); break; }
                        case PasswordChangingStatus.ElementEmpty: { PasswordBoxEmpty?.Invoke(); break; }
                    }
                });
            }));
        }

        private DelegateCommand close;
        public DelegateCommand Close
        {
            get => close ?? (close = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<ClosePreferencesWindowEvent>().Publish();
            }));
        }
    }
}
