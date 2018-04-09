using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Permissions;

namespace Rocket.Core.Permissions
{
    public class PermissionProvider : IPermissionProvider
    {
        private IConfiguration config;
        public PermissionProvider(IConfiguration config)
        {
            this.config = config;
        }

        public bool HasPermission(IPermissionGroup @group, string permission)
        {
            throw new System.NotImplementedException();
        }

        public bool HasPermission(ICommandCaller caller, string permission)
        {
            throw new System.NotImplementedException();
        }

        public bool HasAllPermissions(IPermissionGroup @group, params string[] permissions)
        {
            throw new System.NotImplementedException();
        }

        public bool HasAllPermissions(ICommandCaller caller, params string[] permissions)
        {
            throw new System.NotImplementedException();
        }

        public bool HasAnyPermissions(IPermissionGroup @group, params string[] permissions)
        {
            throw new System.NotImplementedException();
        }

        public bool HasAnyPermissions(ICommandCaller caller, params string[] permissions)
        {
            throw new System.NotImplementedException();
        }

        public IPermissionGroup GetPrimaryGroup(ICommandCaller caller)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IPermissionGroup> GetGroups(ICommandCaller caller)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IPermissionGroup> GetGroups()
        {
            throw new NotImplementedException();
        }

        public void UpdateGroup(IPermissionGroup @group)
        {
            throw new NotImplementedException();
        }

        public void SetGroup(ICommandCaller caller, IPermissionGroup @group)
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            if(config.IsLoaded)
                throw new Exception("Permission provider is already loaded");

            config.Load(new EnvironmentContext
            {
                WorkingDirectory = Environment.CurrentDirectory,
                Name = "Rocket.Permissions"
            });
        }

        public void Reload()
        {
            config.Reload();
        }

        public void Save()
        {
            config.Save();
        }
    }
}