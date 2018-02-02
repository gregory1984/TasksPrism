using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Transform;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;
using NHibernate.Linq;
using Tasks_Crypto;
using Tasks_Model.Helpers;
using Tasks_Model.Entities;
using Tasks_Model.Database;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;
using Tasks_Model.Searching;
using Pro = NHibernate.Criterion.Projections;
using Res = NHibernate.Criterion.Restrictions;
using SqlType = NHibernate.NHibernateUtil;

namespace Tasks_Model.Services
{
    public class TaskService : ITaskService
    {
        private IList<TaskPriorityDTO> taskPriorities;
        private IList<TaskStatusDTO> taskStatuses;
        private IList<TaskGenreDTO> taskGenres;

        public IList<TaskPriorityDTO> GetTaskPriorities()
        {
            if (taskPriorities == null)
            {
                using (var session = Hibernate.SessionFactory.OpenSession())
                {
                    TaskPriority p = null;

                    taskPriorities = session.QueryOver(() => p)
                        .SelectList(l => l
                            .Select(() => p.Id)
                            .Select(() => p.Name))
                        .TransformUsing(Transformers.AliasToBeanConstructor(typeof(TaskPriorityDTO).GetConstructors()[1]))
                        .List<TaskPriorityDTO>();
                }

            }
            return taskPriorities;
        }

        public IList<TaskStatusDTO> GetTaskStatuses()
        {
            if (taskStatuses == null)
            {
                using (var session = Hibernate.SessionFactory.OpenSession())
                {
                    TaskStatus p = null;

                    taskStatuses = session.QueryOver(() => p)
                        .SelectList(l => l
                            .Select(() => p.Id)
                            .Select(() => p.Name))
                        .TransformUsing(Transformers.AliasToBeanConstructor(typeof(TaskStatusDTO).GetConstructors()[1]))
                        .List<TaskStatusDTO>();
                }
            }
            return taskStatuses;
        }

        public IList<TaskGenreDTO> GetTaskGenres()
        {
            if (taskGenres == null)
            {
                using (var session = Hibernate.SessionFactory.OpenSession())
                {
                    TaskGenre p = null;

                    taskGenres = session.QueryOver(() => p)
                        .SelectList(l => l
                            .Select(() => p.Id)
                            .Select(() => p.Name))
                        .TransformUsing(Transformers.AliasToBeanConstructor(typeof(TaskGenreDTO).GetConstructors()[1]))
                        .List<TaskGenreDTO>();
                }
            }
            return taskGenres;
        }

        public string GetSelectedTaskParticipants(int taskId)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Task t = null;
                User u = null;

                var participants = session.QueryOver(() => t)
                    .JoinAlias(() => t.Users, () => u)
                    .Where(() => t.Id == taskId)
                    .SelectList(l => l
                        .Select(Pro.SqlFunction("CONCAT", SqlType.String,
                            Pro.SqlFunction("SUBSTRING", SqlType.String,
                            Pro.SqlFunction("UPPER", SqlType.String, Pro.Property("u.Name")), Pro.Constant(1), Pro.Constant(1))
                            ,
                            Pro.SqlFunction("SUBSTRING", SqlType.String,
                            Pro.SqlFunction("UPPER", SqlType.String, Pro.Property("u.Surname")), Pro.Constant(1), Pro.Constant(1))
                        )))
                    .List<string>();

                return string.Join(", ", participants.OrderBy(s => s));
            }
        }

        public TaskCreationDTO GetSelectedTask(int taskId)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Task t = null;
                TaskPriority p = null;
                TaskStatus s = null;
                TaskGenre g = null;

                TaskCreationDTO taskDTO = null;

                var task = session.QueryOver(() => t)
                    .JoinAlias(() => t.Status, () => s)
                    .JoinAlias(() => t.Priority, () => p)
                    .JoinAlias(() => t.Genre, () => g)
                    .Where(() => t.Id == taskId)
                    .SelectList(l => l
                        .Select(() => t.Id).WithAlias(() => taskDTO.Id)
                        .Select(() => t.Topic).WithAlias(() => taskDTO.Topic)
                        .Select(() => t.Content).WithAlias(() => taskDTO.Content)
                        .Select(() => t.Author).WithAlias(() => taskDTO.Author)
                        .Select(() => t.StartPeriod).WithAlias(() => taskDTO.StartPeriod)
                        .Select(() => t.EndPeriod).WithAlias(() => taskDTO.EndPeriod)
                        .Select(() => s.Id).WithAlias(() => taskDTO.StatusId)
                        .Select(() => p.Id).WithAlias(() => taskDTO.PriorityId)
                        .Select(() => g.Id).WithAlias(() => taskDTO.GenreId))
                    .TransformUsing(Transformers.AliasToBean<TaskCreationDTO>())
                    .SingleOrDefault<TaskCreationDTO>();

                TaskComment tc = null;
                User u = null;

                TaskCommentCreationDTO commentDTO = null;

                var comments = session.QueryOver(() => tc)
                    .JoinAlias(() => tc.Task, () => t)
                    .JoinAlias(() => tc.User, () => u)
                    .Where(() => t.Id == taskId)
                    .SelectList(l => l
                        .Select(() => tc.Id).WithAlias(() => commentDTO.Id)
                        .Select(() => tc.Content).WithAlias(() => commentDTO.Content)
                        .Select(() => tc.Date).WithAlias(() => commentDTO.Date)
                        .Select(() => u.Id).WithAlias(() => commentDTO.AuthorId)
                        .Select(() => u.Username).WithAlias(() => commentDTO.AuthorUsername)
                        .Select(() => u.Name).WithAlias(() => commentDTO.AuthorName)
                        .Select(() => u.Surname).WithAlias(() => commentDTO.AuthorSurname)
                        .Select(() => t.Id).WithAlias(() => commentDTO.TaskId))
                    .OrderBy(() => tc.Date).Desc
                    .TransformUsing(Transformers.AliasToBean<TaskCommentCreationDTO>())
                    .List<TaskCommentCreationDTO>();

                task.Comments = comments;

                var participants = session.QueryOver(() => t)
                    .JoinAlias(() => t.Users, () => u)
                    .Where(() => t.Id == taskId)
                    .SelectList(l => l
                        .Select(() => u.Id))
                    .List<int>();

                task.ParticipantsIds = participants;

                return task;
            }
        }

        public TaskCreationStatus AddModifyTask(TaskCreationDTO task)
        {
            if (task.EndPeriod.HasValue && task.EndPeriod.Value < task.StartPeriod)
                return TaskCreationStatus.EndPeriodEarlierThanStartPeriod;

            if (!task.GenreId.HasValue) return TaskCreationStatus.GenreNotSelected;
            if (!task.StatusId.HasValue) return TaskCreationStatus.StatusNotSelected;
            if (!task.PriorityId.HasValue) return TaskCreationStatus.PriorityNotSelected;

            if (!task.ParticipantsIds.Any())
                return TaskCreationStatus.PatricipantsNotSelected;

            var textFields = new List<string> { task.Topic, task.Content };
            if (textFields.Any(t => string.IsNullOrWhiteSpace(t)))
                return TaskCreationStatus.ElementEmpty;

            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                var closedTaskStatusId = session.QueryOver<TaskStatus>()
                    .Where(s => s.Name == Constants.TaskStatus_Closed)
                    .Select(s => s.Id)
                    .SingleOrDefault<int>();

                if (task.StatusId.HasValue && task.StatusId.Value == closedTaskStatusId && !task.EndPeriod.HasValue)
                    return TaskCreationStatus.ClosedOrCanceledWithNoEndPeriod;

                using (var transaction = session.BeginTransaction())
                {
                    Task taskEntity = task.Id.HasValue
                        ? session.QueryOver<Task>().Where(t => t.Id == task.Id.Value).SingleOrDefault()
                        : new Task();

                    taskEntity.Topic = task.Topic;
                    taskEntity.Content = task.Content;
                    taskEntity.Author = task.Author;
                    taskEntity.StartPeriod = task.StartPeriod;
                    taskEntity.EndPeriod = task.EndPeriod;
                    taskEntity.Status = session.QueryOver<TaskStatus>().Where(s => s.Id == task.StatusId.Value).SingleOrDefault();
                    taskEntity.Genre = session.QueryOver<TaskGenre>().Where(g => g.Id == task.GenreId.Value).SingleOrDefault();
                    taskEntity.Priority = session.QueryOver<TaskPriority>().Where(p => p.Id == task.PriorityId.Value).SingleOrDefault();
                    taskEntity.Users = session.QueryOver<User>().Where(Res.In("Id", task.ParticipantsIds.ToArray())).List();

                    if (task.Id.HasValue)
                    {
                        IList<TaskComment> currentCommentsEntitities = session.QueryOver<TaskComment>()
                            .JoinQueryOver(c => c.Task)
                            .Where(t => t.Id == task.Id.Value)
                            .List();

                        foreach (var comment in currentCommentsEntitities)
                        {
                            session.Delete(comment);
                            session.Flush();
                        }
                    }

                    var commentsEntities = new List<TaskComment>();
                    foreach (var c in task.Comments.OrderBy(c => c.Date))
                    {
                        commentsEntities.Add(new TaskComment
                        {
                            Content = c.Content,
                            Date = c.Date,
                            User = session.QueryOver<User>().Where(u => u.Id == c.AuthorId).SingleOrDefault(),
                            Task = taskEntity
                        });
                    }

                    taskEntity.Comments = commentsEntities;

                    session.SaveOrUpdate(taskEntity);
                    session.Flush();
                    transaction.Commit();

                    return task.Id.HasValue ? TaskCreationStatus.Modified : TaskCreationStatus.Added;
                }
            }
        }

        public void RemoveTask(int taskId)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Task t = null;

                var task = session.QueryOver(() => t)
                    .Where(() => t.Id == taskId)
                    .SingleOrDefault();

                session.Delete(task);
                session.Flush();
            }
        }

        public IList<TaskPrimaryDataDTO> GetTasks(int pageNo, int pageSize, TaskSearchCriteria criteria)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Task t = null;
                TaskStatus s = null;
                TaskPriority p = null;
                TaskGenre g = null;
                TaskComment c = null;
                TaskComment c_sub = null;
                User uc = null;

                var query = session.QueryOver(() => t)
                    .JoinAlias(() => t.Status, () => s)
                    .JoinAlias(() => t.Priority, () => p)
                    .JoinAlias(() => t.Genre, () => g)
                    .Left.JoinAlias(() => t.Comments, () => c)
                    .Left.JoinAlias(() => c.User, () => uc)
                    .Where(
                        Res.EqProperty(
                            Pro.Conditional(Res.IsNull("c.Id"), Pro.Constant(0), Pro.Property("c.Id")),
                            Pro.SubQuery(QueryOver.Of(() => c_sub)
                                .Where(() => c_sub.Task.Id == c.Task.Id)
                                .Select(Pro.Conditional(Res.IsNull("c_sub.Id"), Pro.Constant(0), Pro.Max("c_sub.Id"))))
                            ));

                if (criteria.TaskAuthorUsername.IsNotNull()) query.Where(() => t.Author == criteria.TaskAuthorUsername);
                if (criteria.ShowTasksOnly) query.Where(() => g.Id == 4);
                if (criteria.ShowInstallationsOnly) query.Where(() => g.Id == 2);
                if (criteria.ShowTonersOnly) query.Where(() => g.Id == 3);
                if (criteria.ShowUpdatesOnly) query.Where(() => g.Id == 1);
                if (criteria.ShowWithoutCanceledTasks) query.Where(Res.Not(Res.Eq(Pro.Property("s.Id"), 1)));
                if (criteria.TaskId.IsNotNull()) query.Where(() => t.Id == criteria.TaskId.Value);
                if (criteria.Topic.IsNotNull()) query.Where(Res.InsensitiveLike(Pro.Property("t.Topic"), criteria.Topic, MatchMode.Anywhere));
                if (criteria.Content.IsNotNull()) query.Where(Res.InsensitiveLike(Pro.Property("t.Content"), criteria.Content, MatchMode.Anywhere));
                if (criteria.StartDate.IsNotNull()) query.Where(Res.Eq(Pro.Cast(SqlType.Date, Pro.Property("t.StartPeriod")), criteria.StartDate.Value));
                if (criteria.StartHour.IsNotNull()) query.Where(Res.Eq(Pro.Cast(SqlType.Time, Pro.Property("t.StartPeriod")), criteria.StartHour.Value.ToString()));
                if (criteria.EndDate.IsNotNull()) query.Where(Res.Eq(Pro.Cast(SqlType.Date, Pro.Property("t.EndPeriod")), criteria.EndDate.Value));
                if (criteria.EndDate.IsNotNull()) query.Where(Res.Eq(Pro.Cast(SqlType.Time, Pro.Property("t.EndPeriod")), criteria.EndHour.Value.ToString()));
                if (criteria.TaskPriorityId.IsNotNull()) query.Where(() => p.Id == criteria.TaskPriorityId);
                if (criteria.TaskStatusId.IsNotNull()) query.Where(() => s.Id == criteria.TaskStatusId);

                if (criteria.TaskParticipantId.IsNotNull())
                {
                    Task subT = null;
                    User subU = null;

                    var subquery = QueryOver.Of(() => subT)
                        .JoinAlias(() => subT.Users, () => subU)
                        .Where(Res.And(
                            Res.EqProperty(Pro.Property("t.Id"), Pro.Property("subT.Id")),
                            Res.Eq(Pro.Property("subU.Id"), criteria.TaskParticipantId)))
                        .SelectList(l => l
                            .Select(() => subU.Id));

                    query.WithSubquery.WhereExists(subquery);
                }

                if (criteria.Comment.IsNotNull())
                {
                    TaskComment subC = null;
                    Task subT = null;

                    var subquery = QueryOver.Of(() => subC)
                        .JoinAlias(() => subC.Task, () => subT)
                        .Where(Res.And(
                            Res.EqProperty(Pro.Property("subT.Id"), Pro.Property("t.Id")),
                            Res.InsensitiveLike(Pro.Property("subC.Content"), criteria.Comment, MatchMode.Anywhere)))
                        .SelectList(l => l
                            .Select(() => subT.Id));

                    query.WithSubquery.WhereExists(subquery);
                }

                if (criteria.CommentAuthorId.IsNotNull())
                {
                    TaskComment subC = null;
                    Task subT = null;
                    User subU = null;

                    var subquery = QueryOver.Of(() => subC)
                        .JoinAlias(() => subC.Task, () => subT)
                        .JoinAlias(() => subC.User, () => subU)
                        .Where(Res.And(
                            Res.EqProperty(Pro.Property("subT.Id"), Pro.Property("t.Id")),
                            Res.Eq(Pro.Property("subU.Id"), criteria.CommentAuthorId)))
                        .SelectList(l => l
                            .Select(() => subT.Id));

                    query.WithSubquery.WhereExists(subquery);
                }

                query.SelectList(l => l
                    .Select(() => t.Id)
                    .Select(() => t.Topic)
                    .Select(() => t.Content)
                    .Select(() => t.StartPeriod)
                    .Select(() => t.EndPeriod)
                    .Select(() => c.Id)
                    .Select(() => c.Content)
                    .Select(() => c.Date)
                    .Select(() => uc.Username)
                    .Select(() => s.Name)
                    .Select(() => p.Name)
                    .Select(() => g.Name)
                    .Select(() => t.Author))
                .OrderBy(() => t.Id).Desc
                .TransformUsing(Transformers.AliasToBeanConstructor(typeof(TaskPrimaryDataDTO).GetConstructors()[1]))
                .Skip(pageSize * (pageNo - 1)).Take(pageSize);

                return query.List<TaskPrimaryDataDTO>();

                #region Above generates sql like...
                /*
                    SELECT this_.Id AS y0_,
                           this_.Topic AS y1_,
                           this_.StartPeriod AS y2_,
                           this_.EndPeriod AS y3_,
                           c4_.Id AS y4_,
                           c4_.Content AS y5_,
                           c4_.Date AS y6_,
                           u5_.Username AS y7_,
                           s1_.Name AS y8_,
                           p2_.Name AS y9_,
                           g3_.Name AS y10_
                    FROM `Task` this_
                        LEFT OUTER JOIN `TaskComment` c4_ ON this_.Id=c4_.Task_id
                        LEFT OUTER JOIN `User` u5_ ON c4_.User_id=u5_.Id
                        INNER JOIN `TaskGenre` g3_ ON this_.Genre_id=g3_.Id
                        INNER JOIN `TaskStatus` s1_ ON this_.Status_id=s1_.Id
                        INNER JOIN `TaskPriority` p2_ ON this_.Priority_id=p2_.Id
                    WHERE (CASE WHEN c4_.Id IS NULL THEN ?p0 ELSE c4_.Id END) =
                          (SELECT (CASE WHEN this_0_.Id IS NULL THEN ?p1 ELSE max(this_0_.Id) END) AS y0_ FROM `TaskComment` this_0_)
                    ORDER BY this_.Id DESC
                    LIMIT ?p2;

                    ?p0 = 0 [Type: Int32 (0)],
                    ?p1 = 0 [Type: Int32 (0)],
                    ?p2 = 50 [Type: Int32 (0)]
                */
                #endregion
            }
        }

        public int GetTasksPagesCount(int pageSize, TaskSearchCriteria criteria)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Task t = null;
                TaskStatus s = null;
                TaskPriority p = null;
                TaskGenre g = null;
                TaskComment c = null;
                User uc = null;

                var query = session.QueryOver(() => t)
                    .JoinAlias(() => t.Status, () => s)
                    .JoinAlias(() => t.Priority, () => p)
                    .JoinAlias(() => t.Genre, () => g)
                    .Left.JoinAlias(() => t.Comments, () => c)
                    .Left.JoinAlias(() => c.User, () => uc);

                if (criteria.TaskAuthorUsername.IsNotNull()) query.Where(() => t.Author == criteria.TaskAuthorUsername);
                if (criteria.ShowTasksOnly) query.Where(() => g.Id == 4);
                if (criteria.ShowInstallationsOnly) query.Where(() => g.Id == 2);
                if (criteria.ShowTonersOnly) query.Where(() => g.Id == 3);
                if (criteria.ShowUpdatesOnly) query.Where(() => g.Id == 1);
                if (criteria.ShowWithoutCanceledTasks) query.Where(Res.Not(Res.Eq(Pro.Property("s.Id"), 1)));
                if (criteria.TaskId.IsNotNull()) query.Where(() => t.Id == criteria.TaskId.Value);
                if (criteria.Topic.IsNotNull()) query.Where(Res.InsensitiveLike(Pro.Property("t.Topic"), criteria.Topic, MatchMode.Anywhere));
                if (criteria.Content.IsNotNull()) query.Where(Res.InsensitiveLike(Pro.Property("t.Content"), criteria.Content, MatchMode.Anywhere));
                if (criteria.StartDate.IsNotNull()) query.Where(Res.Eq(Pro.Cast(SqlType.Date, Pro.Property("t.StartPeriod")), criteria.StartDate.Value));
                if (criteria.StartHour.IsNotNull()) query.Where(Res.Eq(Pro.Cast(SqlType.Time, Pro.Property("t.StartPeriod")), criteria.StartHour.Value.ToString()));
                if (criteria.EndDate.IsNotNull()) query.Where(Res.Eq(Pro.Cast(SqlType.Date, Pro.Property("t.EndPeriod")), criteria.EndDate.Value));
                if (criteria.EndDate.IsNotNull()) query.Where(Res.Eq(Pro.Cast(SqlType.Time, Pro.Property("t.EndPeriod")), criteria.EndHour.Value.ToString()));
                if (criteria.TaskPriorityId.IsNotNull()) query.Where(() => p.Id == criteria.TaskPriorityId);
                if (criteria.TaskStatusId.IsNotNull()) query.Where(() => s.Id == criteria.TaskStatusId);

                if (criteria.TaskParticipantId.IsNotNull())
                {
                    Task subT = null;
                    User subU = null;

                    var subquery = QueryOver.Of(() => subT)
                        .JoinAlias(() => subT.Users, () => subU)
                        .Where(Res.And(
                            Res.EqProperty(Pro.Property("t.Id"), Pro.Property("subT.Id")),
                            Res.Eq(Pro.Property("subU.Id"), criteria.TaskParticipantId)))
                        .SelectList(l => l
                            .Select(() => subU.Id));

                    query.WithSubquery.WhereExists(subquery);
                }


                if (criteria.Comment.IsNotNull())
                {
                    TaskComment subC = null;
                    Task subT = null;

                    var subquery = QueryOver.Of(() => subC)
                        .JoinAlias(() => subC.Task, () => subT)
                        .Where(Res.And(
                            Res.EqProperty(Pro.Property("subT.Id"), Pro.Property("t.Id")),
                            Res.InsensitiveLike(Pro.Property("subC.Content"), criteria.Comment, MatchMode.Anywhere)))
                        .SelectList(l => l
                            .Select(() => subT.Id));

                    query.WithSubquery.WhereExists(subquery);
                }

                if (criteria.CommentAuthorId.IsNotNull())
                {
                    TaskComment subC = null;
                    Task subT = null;
                    User subU = null;

                    var subquery = QueryOver.Of(() => subC)
                        .JoinAlias(() => subC.Task, () => subT)
                        .JoinAlias(() => subC.User, () => subU)
                        .Where(Res.And(
                            Res.EqProperty(Pro.Property("subT.Id"), Pro.Property("t.Id")),
                            Res.Eq(Pro.Property("subU.Id"), criteria.CommentAuthorId)))
                        .SelectList(l => l
                            .Select(() => subT.Id));

                    query.WithSubquery.WhereExists(subquery);
                }

                var result = Math.Ceiling((1.0 * query.RowCount()) / pageSize);
                return result == 0 ? 1 : Convert.ToInt32(result);
            }
        }
    }
}
