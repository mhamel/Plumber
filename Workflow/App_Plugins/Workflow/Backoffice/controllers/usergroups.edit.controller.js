﻿(function () {
    'use strict';

    // create controller 
    function EditController($scope, $routeParams, userGroupsResource, entityResource, notificationsService) {

        $scope.action = $routeParams.id !== '-1' ? 'Edit' : 'Create';

        // fetch all active users, then get the group
        // this kicks it all off...
        (function getAllUsers() {
            entityResource.getAll('User', 'IsApproved')
                .then(function (resp) {
                    $scope.allUsers = resp;
                    getGroup();
                });
        })();

        // only fetch group if id is valid - otherwise it's a create action
        function getGroup() {
            if ($routeParams.id !== '-1') {
                userGroupsResource.getGroup($routeParams.id)
                    .then(function (resp) {
                        if (resp.status === 200) {
                            $scope.group = resp.data;
                            getUsersNotInGroup();
                        }
                    });
            } else {
                $scope.group = {
                    GroupId: -1,
                    Name: '',
                    Description: '',
                    Alias: '',
                    GroupEmail: '',
                    Users: [],
                    UsersSummary: ''
                };
                getUsersNotInGroup();
            }
        };

        // filter all users to remove those in the group
        function getUsersNotInGroup() {
            $scope.notInGroup = [];
            angular.forEach($scope.allUsers, function (user) {
                if (!$scope.group.UsersSummary || $scope.group.UsersSummary.indexOf('|' + user.id + '|') === -1) {
                    $scope.notInGroup.push(user);
                }
            });
        };

        // add a user to the group, and remove from notInGroup
        $scope.add = function (id) {
            var index,
                user = $.grep($scope.notInGroup, function (u, i) {
                    if (u.id === id) {
                        index = i;
                        $scope.group.Users.push({ 'UserId': u.id, 'GroupId': $scope.group.GroupId, 'Name': u.name });
                        return true;
                    }
                    return false;
                })[0];

            $scope.notInGroup.splice(index, 1);
        };

        //
        $scope.remove = function (id) {
            var index,
                user = $.grep($scope.group.Users, function (u, i) {
                    if (u.UserId === id) {
                        index = i;
                        $scope.notInGroup.push({ 'id': u.UserId, 'name': u.Name });
                        return true;
                    }
                    return false;
                });

            $scope.group.Users.splice(index, 1);
        };

        //
        $scope.saveGroup = function () {
            userGroupsResource.saveGroup($scope.group)
                .then(function (resp) {
                    if (resp.status === 200) {
                        notificationsService.success("SUCCESS", resp.data);
                    }
                    else {
                        notificationsService.error("ERROR", resp.data);
                    }
                });
        };
    };

    // register controller 
    angular.module('umbraco').controller('Workflow.UserGroups.Edit.Controller', EditController);
}());

