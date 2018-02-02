using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks_Model.DTO;
using Tasks_Model.Searching;

namespace Tasks_Model.Interfaces
{
    public enum TaskCreationStatus
    {
        Added,
        Modified,
        GenreNotSelected,
        StatusNotSelected,
        PriorityNotSelected,
        PatricipantsNotSelected,
        ElementEmpty,
        ClosedOrCanceledWithNoEndPeriod,
        EndPeriodEarlierThanStartPeriod
    }

    public interface ITaskService
    {
        IList<TaskPriorityDTO> GetTaskPriorities();
        IList<TaskStatusDTO> GetTaskStatuses();
        IList<TaskGenreDTO> GetTaskGenres();
        string GetSelectedTaskParticipants(int taskId);
        TaskCreationDTO GetSelectedTask(int taskId);

        TaskCreationStatus AddModifyTask(TaskCreationDTO task);
        void RemoveTask(int taskId);

        IList<TaskPrimaryDataDTO> GetTasks(int pageNo, int pageSize, TaskSearchCriteria searchCriteria);
        int GetTasksPagesCount(int pageSize, TaskSearchCriteria searchCriteria);
    }
}
