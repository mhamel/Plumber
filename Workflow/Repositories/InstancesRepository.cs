﻿using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Workflow.Helpers;
using Workflow.Models;
using Workflow.Relators;
using Workflow.Repositories.Interfaces;

namespace Workflow.Repositories
{
    public class InstancesRepository : IInstancesRepository
    {
        private readonly UmbracoDatabase _database;

        public InstancesRepository()
            : this(ApplicationContext.Current.DatabaseContext.Database)
        {
        }

        private InstancesRepository(UmbracoDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poco"></param>
        public void InsertInstance(WorkflowInstancePoco poco)
        {
            _database.Insert(poco);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poco"></param>
        public void UpdateInstance(WorkflowInstancePoco poco)
        {
            _database.Update(poco);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountPendingInstances()
        {
            return _database.Fetch<int>(SqlHelpers.CountPendingInstances).First();
        }

        /// <summary>
        /// Get all workflow instances
        /// </summary>
        /// <returns>A list of objects of type <see cref="WorkflowInstancePoco"/></returns>
        public List<WorkflowInstancePoco> GetAllInstances()
        {
            return _database.Fetch<WorkflowInstancePoco, WorkflowTaskInstancePoco, UserGroupPoco, WorkflowInstancePoco>(new UserToGroupForInstanceRelator().MapIt, SqlHelpers.AllInstances);
        }

        /// <summary>
        /// Get all workflow instances created after the given date
        /// </summary>
        /// <param name="oldest">The creation date of the oldest instances to return</param>
        /// <returns>A list of objects of type <see cref="WorkflowInstancePoco"/></returns>
        public List<WorkflowInstancePoco> GetAllInstancesForDateRange(DateTime oldest)
        {
            return _database.Fetch<WorkflowInstancePoco>(SqlHelpers.AllInstancesForDateRange, oldest);
        }

        /// <summary>
        /// Get a single instance by guid
        /// </summary>
        /// <param name="guid">The instance guid</param>
        /// <returns>A list of objects of type <see cref="WorkflowInstancePoco"/></returns>
        public WorkflowInstancePoco GetInstanceByGuid(Guid guid)
        {
            return _database.Fetch<WorkflowInstancePoco>(SqlHelpers.InstanceByGuid, guid).First();
        }

        /// <summary>
        /// Get all instances matching the given status[es] for the given node id
        /// </summary>
        /// <param name="nodeId">The node id</param>
        /// <param name="status">Optional list of WorkflowStatus integers. If not provided, method returns all instances for the node.</param>
        /// <returns>A list of objects of type <see cref="WorkflowInstancePoco"/></returns>
        public IEnumerable<WorkflowInstancePoco> GetInstancesForNodeByStatus(int nodeId, IEnumerable<int> status = null)
        {
            if (status == null || !status.Any())
                return _database.Fetch<WorkflowInstancePoco>(SqlHelpers.InstanceByNodeStr, nodeId);

            string statusStr = string.Concat("Status = ", string.Join(" OR Status = ", status));
            if (!string.IsNullOrEmpty(statusStr))
            {
                statusStr = " AND " + statusStr;
            }

            return _database.Fetch<WorkflowInstancePoco>(string.Concat(SqlHelpers.InstanceByNodeStr, statusStr), nodeId);
        }
    }
}
