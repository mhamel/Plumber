﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Workflow.Events.Args;
using Workflow.Models;
using Workflow.Repositories;
using Workflow.Repositories.Interfaces;
using Workflow.Services.Interfaces;

namespace Workflow.Services
{
    public class GroupService : IGroupService
    {
        private readonly ILogger _log;
        private readonly IPocoRepository _repo;

        public static event EventHandler<GroupEventArgs> Created;
        public static event EventHandler<GroupEventArgs> Updated;
        public static event EventHandler<GroupDeletedEventArgs> Deleted;

        public GroupService()
            : this(
                ApplicationContext.Current.ProfilingLogger.Logger,
                new PocoRepository()
            )
        {
        }

        private GroupService(ILogger log, IPocoRepository repo)
        {
            _log = log;
            _repo = repo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<UserGroupPoco> CreateUserGroupAsync(string name)
        {
            string alias = name.Replace(" ", "-");
            bool exists = _repo.GroupAliasExists(alias);

            if (exists)
                return Task.FromResult((UserGroupPoco)null);

            UserGroupPoco group = _repo.InsertUserGroup(name, alias, false);

            // emit event
            Created?.Invoke(this, new GroupEventArgs(group));

            return Task.FromResult(group);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Task<UserGroupPoco> GetPopulatedUserGroupAsync(int groupId)
        {
            UserGroupPoco group = _repo.GetPopulatedUserGroup(groupId);

            return Task.FromResult(group);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<UserGroupPoco> GetUserGroupAsync(int id)
        {
            UserGroupPoco result = _repo.GetPopulatedUserGroup(id);
            return Task.FromResult(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UserGroupPermissionsPoco GetDefaultUserGroupPermissions(string name)
        {
            UserGroupPermissionsPoco result = _repo.GetDefaultUserGroupPermissions(name);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<UserGroupPoco>> GetUserGroupsAsync()
        {
            IEnumerable<UserGroupPoco> result = _repo.GetUserGroups();
            return Task.FromResult(result.Where(r => !r.Deleted));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poco"></param>
        /// <returns></returns>
        public Task<UserGroupPoco> UpdateUserGroupAsync(UserGroupPoco poco)
        {
            bool nameExists = _repo.GroupNameExists(poco.Name);
            UserGroupPoco existingPoco = _repo.GetUserGroupById(poco.GroupId);

            if (poco.Name != existingPoco.Name && nameExists)
                return Task.FromResult((UserGroupPoco)null);

            _repo.DeleteUsersFromGroup(existingPoco.GroupId);

            foreach (var user in poco.Users)
                _repo.AddUserToGroup(user);

            _repo.UpdateUserGroup(poco);

            Updated?.Invoke(this, new GroupEventArgs(poco));

            return Task.FromResult(poco);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Task DeleteUserGroupAsync(int groupId)
        {
            Deleted?.Invoke(this, new GroupDeletedEventArgs(groupId));
            return Task.Run(() => _repo.DeleteUserGroup(groupId));
        }
    }
}
