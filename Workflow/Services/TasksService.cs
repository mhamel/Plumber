﻿using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Workflow.Models;
using Workflow.Repositories;
using Workflow.Repositories.Interfaces;
using Workflow.Services.Interfaces;

namespace Workflow.Services
{
    public class TasksService : ITasksService
    {
        private readonly IConfigService _configService;
        private readonly ITasksRepository _tasksRepo;

        public TasksService()
            : this(
                new TasksRepository(),
                new ConfigService()
            )
        {
        }

        private TasksService(ITasksRepository tasksRepo, IConfigService configService)
        {
            _tasksRepo = tasksRepo;
            _configService = configService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountPendingTasks()
        {
            return _tasksRepo.CountPendingTasks();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public int CountGroupTasks(int groupId)
        {
            return _tasksRepo.CountGroupTasks(groupId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="count"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<WorkflowTask> GetPendingTasks(IEnumerable<int> status, int count, int page)
        {
            IEnumerable<WorkflowTaskInstancePoco> taskInstances = _tasksRepo.GetPendingTasks(status);
            List<WorkflowTask> tasks = ConvertToWorkflowTaskList(taskInstances.Skip((page - 1) * count).Take(count).ToList());

            return tasks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="count"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<WorkflowTask> GetAllGroupTasks(int groupId, int count, int page)
        {
            IEnumerable<WorkflowTaskInstancePoco> taskInstances = _tasksRepo.GetAllGroupTasks(groupId);
            List<WorkflowTask> tasks = ConvertToWorkflowTaskList(taskInstances.Skip((page - 1) * count).Take(count).ToList());

            return tasks;
        }

        /// <summary>
        /// Converts a list of workflowTaskInstances into a list of UI-ready workflowTasks
        /// </summary>
        /// <param name="taskInstances"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public List<WorkflowTask> ConvertToWorkflowTaskList(List<WorkflowTaskInstancePoco> taskInstances, WorkflowInstancePoco instance = null)
        {
            List<WorkflowTask> workflowItems = new List<WorkflowTask>();

            if (!taskInstances.Any()) return workflowItems;

            bool useInstanceFromTask = instance == null;

            foreach (WorkflowTaskInstancePoco taskInstance in taskInstances)
            {
                instance = useInstanceFromTask ? taskInstance.WorkflowInstance : instance;

                string instanceNodeName = instance.Node?.Name ?? "NODE NO LONGER EXISTS";
                string typeDescription = instance.TypeDescription;

                var item = new WorkflowTask
                {
                    Status = taskInstance.StatusName,
                    CssStatus = taskInstance.StatusName.ToLower().Split(' ')[0],
                    Type = typeDescription,
                    NodeId = instance.NodeId,
                    InstanceGuid = instance.Guid,
                    ApprovalGroupId = taskInstance.UserGroup.GroupId,
                    NodeName = instanceNodeName,
                    RequestedBy = instance.AuthorUser.Name,
                    RequestedById = instance.AuthorUser.Id,
                    RequestedOn = taskInstance.CreatedDate.ToString(),
                    ApprovalGroup = taskInstance.UserGroup.Name,
                    Comments = useInstanceFromTask ? instance.AuthorComment : taskInstance.Comment,
                    ActiveTask = taskInstance.StatusName,
                    Permissions = _configService.GetRecursivePermissionsForNode(instance.Node),
                    CurrentStep = taskInstance.ApprovalStep
                };

                workflowItems.Add(item);
            }

            return workflowItems.OrderByDescending(x => x.CurrentStep).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<WorkflowTaskInstancePoco> GetAllPendingTasks(IEnumerable<int> status)
        {
            List<WorkflowTaskInstancePoco> taskInstances = _tasksRepo.GetAllPendingTasks(status);
            return taskInstances;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldest"></param>
        /// <returns></returns>
        public List<WorkflowTaskInstancePoco> GetAllTasksForDateRange(DateTime oldest)
        {
            List<WorkflowTaskInstancePoco> taskInstances = _tasksRepo.GetAllTasksForDateRange(oldest);
            return taskInstances;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<WorkflowTaskInstancePoco> GetTasksByNodeId(int id)
        {
            List<WorkflowTaskInstancePoco> taskInstances = _tasksRepo.GetTasksByNodeId(id);
            return taskInstances;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<WorkflowTaskInstancePoco> GetTaskSubmissionsForUser(int id, IEnumerable<int> status)
        {
            return _tasksRepo.GetTaskSubmissionsForUser(id, status);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public List<WorkflowTaskInstancePoco> GetTasksWithGroupByInstanceGuid(Guid guid)
        {
            return _tasksRepo.GetTasksAndGroupByInstanceId(guid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poco"></param>
        public void InsertTask(WorkflowTaskInstancePoco poco)
        {
            _tasksRepo.InsertTask(poco);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poco"></param>
        /// <returns></returns>
        public void UpdateTask(WorkflowTaskInstancePoco poco)
        {
            _tasksRepo.UpdateTask(poco);
        }
    }
}
