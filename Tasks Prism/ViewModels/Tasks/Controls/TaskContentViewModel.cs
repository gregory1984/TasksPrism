using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Threading;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Commands;
using Tasks_Prism.Events;
using Tasks_Prism.Events.TaskCreation;
using Tasks_Prism.Events.Payloads;
using Tasks_Prism.ViewModels.Base;
using Tasks_Prism.Helpers;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;

namespace Tasks_Prism.ViewModels.Tasks.Controls
{
    public class TaskContentViewModel : ViewModelBase
    {
        #region Privileges
        private bool isReadOnly;
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set { SetProperty(ref isReadOnly, value); }
        }
        #endregion

        #region Properties
        private string topic = "";
        public string Topic
        {
            get { return topic; }
            set
            {
                SetProperty(ref topic, value);
                TopicLength = Topic.Length.ToString();
            }
        }

        private string topicLength = "0";
        public string TopicLength
        {
            get { return topicLength; }
            set { SetProperty(ref topicLength, value); }
        }

        private string content = "";
        public string Content
        {
            get { return content; }
            set
            {
                SetProperty(ref content, value);
                ContentLength = Content.Length.ToString();
            }
        }

        private string contentLength = "0";
        public string ContentLength
        {
            get { return contentLength; }
            set { SetProperty(ref contentLength, value); }
        }
        #endregion

        #region Event tokens
        private SubscriptionToken taskDataRequestToken;
        #endregion

        public TaskContentViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService)
            : base(eventAggregator, unityContainer, userService)
        {

        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                taskDataRequestToken = eventAggregator.GetEvent<TaskDataRequestEvent>().Subscribe(() =>
                {
                    var content = new TaskContentCollectedPayload
                    {
                        Topic = this.Topic,
                        Content = this.Content
                    };

                    eventAggregator.GetEvent<TaskContentCollectedEvent>().Publish(content);
                });

                eventAggregator.ExecuteSafety(() =>
                {
                    var taskData = unityContainer.Resolve<TaskCreationDTO>(UnityNames.ModifiedTaskData);

                    if (taskData.Id.HasValue)
                    {
                        Topic = taskData.Topic;
                        Content = taskData.Content;
                    }

                    SetPrivileges(taskData.Id);
                });
            }));
        }

        private void SetPrivileges(int? taskId)
        {
            IList<Permission> granted = userService.AuthenticatedUserPermissions.ConvertFromDTO();

            if (granted.Contains(Permission.TasksBrowsing))
            {
                IsReadOnly = true;
                if (IsTaskAddingMode(taskId, granted) || IsTaskModifyingMode(taskId, granted))
                {
                    IsReadOnly = false;
                }
            }
        }

        private bool IsTaskAddingMode(int? taskId, IList<Permission> granted)
            => !taskId.HasValue && granted.Contains(Permission.TasksAdding);

        private bool IsTaskModifyingMode(int? taskId, IList<Permission> granted)
            => taskId.HasValue && granted.Contains(Permission.TasksModifying);

        public override void UnsubscribePrismEvents()
        {
            base.UnsubscribePrismEvents();
            eventAggregator.GetEvent<TaskDataRequestEvent>().Unsubscribe(taskDataRequestToken);
        }
    }
}
