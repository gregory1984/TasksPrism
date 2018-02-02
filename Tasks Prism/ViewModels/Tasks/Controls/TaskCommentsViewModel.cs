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
    public class TaskCommentsViewModel : ViewModelBase
    {
        #region Privileges
        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { SetProperty(ref isEnabled, value); }
        }
        #endregion

        #region Properties
        private IList<TaskCommentPrimaryDataDTO> comments;
        public IList<TaskCommentPrimaryDataDTO> Comments
        {
            get { return comments; }
            set { SetProperty(ref comments, value); }
        }

        private TaskCommentPrimaryDataDTO selectedComment;
        public TaskCommentPrimaryDataDTO SelectedComment
        {
            get { return selectedComment; }
            set { SetProperty(ref selectedComment, value); }
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

        public TaskCommentsViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService)
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
                    var comments = new TaskCommentsCollectedPayload
                    {
                        Comments = new List<TaskCommentPrimaryDataDTO>(this.Comments)
                    };

                    eventAggregator.GetEvent<TaskCommentsCollectedEvent>().Publish(comments);
                });

                eventAggregator.ExecuteSafety(() =>
                {
                    Comments = new ObservableCollection<TaskCommentPrimaryDataDTO>();

                    var taskData = unityContainer.Resolve<TaskCreationDTO>(UnityNames.ModifiedTaskData);
                    if (taskData.Id.HasValue)
                    {
                        foreach (var c in taskData.Comments)
                        {
                            Comments.Add(new TaskCommentPrimaryDataDTO(c.Id, c.Content, c.Date, c.AuthorId, c.AuthorUsername));
                        }
                    }

                    SetPrivileges(taskData.Id);
                });
            }));
        }

        private DelegateCommand addComment;
        public DelegateCommand AddComment
        {
            get => addComment = (addComment = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    var comment = new TaskCommentPrimaryDataDTO
                    {
                        AuthorId = userService.AuthenticatedUser.Id.Value,
                        Author = userService.AuthenticatedUser.Username,
                        Content = this.Content,
                        Date = DateTime.Now
                    };

                    comments.Insert(0, comment);
                    Content = "";
                });
            }));
        }

        private void SetPrivileges(int? taskId)
        {
            IList<Permission> granted = userService.AuthenticatedUserPermissions.ConvertFromDTO();

            if (granted.Contains(Permission.TasksBrowsing))
            {
                IsEnabled = false;
                if (IsTaskAddingMode(taskId, granted) || IsTaskModifyingMode(taskId, granted))
                {
                    IsEnabled = true;
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
