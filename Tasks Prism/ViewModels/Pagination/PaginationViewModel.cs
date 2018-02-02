using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Tasks_Model.Interfaces;
using Tasks_Prism.Events.Pagination;

namespace Tasks_Prism.ViewModels.Pagination
{
    public class PaginationViewModel : BindableBase
    {
        #region Properties
        private int pageNo = 1;
        public int PageNo
        {
            get { return pageNo; }
            set
            {
                SetProperty(ref pageNo, value);

                FirstPage.RaiseCanExecuteChanged();
                PreviousPage.RaiseCanExecuteChanged();
                NextPage.RaiseCanExecuteChanged();
                LastPage.RaiseCanExecuteChanged();
            }
        }

        private int pageCount = 1;
        public int PageCount
        {
            get { return pageCount; }
            set
            {
                SetProperty(ref pageCount, value);

                NextPage.RaiseCanExecuteChanged();
                LastPage.RaiseCanExecuteChanged();
            }
        }

        public int PageSize
        {
            get => preferencesService.Preferences.TasksPerPage;
        }
        #endregion

        private readonly IEventAggregator eventAggregator;
        private readonly IPreferencesService preferencesService;

        public PaginationViewModel(IEventAggregator eventAggregator, IPreferencesService preferencesService)
        {
            this.eventAggregator = eventAggregator;
            this.preferencesService = preferencesService;
        }

        private DelegateCommand firstPage;
        public DelegateCommand FirstPage
        {
            get => firstPage ?? (firstPage = new DelegateCommand(() =>
            {
                PageNo = 1;
                eventAggregator.GetEvent<JumpToPageEvent>().Publish(PageNo);
            },

            () => PageNo > 1));
        }

        private DelegateCommand previousPage;
        public DelegateCommand PreviousPage
        {
            get => previousPage ?? (previousPage = new DelegateCommand(() =>
            {
                PageNo -= 1;
                eventAggregator.GetEvent<JumpToPageEvent>().Publish(PageNo);
            },

            () => PageNo > 1));
        }

        private DelegateCommand nextPage;
        public DelegateCommand NextPage
        {
            get => nextPage ?? (nextPage = new DelegateCommand(() =>
            {
                PageNo += 1;
                eventAggregator.GetEvent<JumpToPageEvent>().Publish(PageNo);
            },

            () => PageNo < PageCount));
        }

        private DelegateCommand lastPage;
        public DelegateCommand LastPage
        {
            get => lastPage ?? (lastPage = new DelegateCommand(() =>
            {
                PageNo = PageCount;
                eventAggregator.GetEvent<JumpToPageEvent>().Publish(PageNo);
            },

            () => PageNo < PageCount));
        }
    }
}
