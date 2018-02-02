using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Events;
using Prism.Commands;
using Tasks_Prism.Events;
using Tasks_Model.Interfaces;

namespace Tasks_Prism.ViewModels.Base
{
    public class ViewModelBase : BindableBase
    {
        protected readonly IEventAggregator eventAggregator;
        protected readonly IUnityContainer unityContainer;
        protected readonly IUserService userService;

        protected SubscriptionToken exceptionOccuredToken;

        public ViewModelBase(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService)
        {
            this.eventAggregator = eventAggregator;
            this.unityContainer = unityContainer;
            this.userService = userService;
        }

        protected void SubscribeExceptionHandling()
            => exceptionOccuredToken = eventAggregator.GetEvent<ExceptionOccuredEvent>()
                .Subscribe(ex => ExceptionOccured?.Invoke(ex));

        public virtual void UnsubscribePrismEvents()
            => eventAggregator.GetEvent<ExceptionOccuredEvent>().Unsubscribe(exceptionOccuredToken);

        public delegate void ExceptionOccuredDelegate(Exception exception);
        public event ExceptionOccuredDelegate ExceptionOccured;
    }
}
