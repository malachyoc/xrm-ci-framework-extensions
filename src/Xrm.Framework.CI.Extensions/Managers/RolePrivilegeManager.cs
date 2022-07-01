using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm.Framework.CI.Common.Logging;
using Xrm.Framework.CI.Extensions.DataOperations;
using Xrm.Framework.CI.Extensions.Entities;
using Xrm.Typed.Entities;
using static Xrm.Framework.CI.Extensions.DataOperations.DataImportManager;

namespace Xrm.Framework.CI.Extensions.Managers
{
    public class RolePrivilegeManager
    {
        #region Constructor and Member Variables
        private IOrganizationService _crmService;
        ILogger _logger;

        //Cache privilages so we dont need privilege id on requests
        private Dictionary<string, Privilege> _privileges = null;
        private Dictionary<string, Privilege> Privileges
        {
            get
            {
                if(_privileges == null)
                {
                    RetrievePrivilegeSetRequest retrievePrivilegeRequest = new RetrievePrivilegeSetRequest();
                    var response = (RetrievePrivilegeSetResponse)_crmService.Execute(retrievePrivilegeRequest);

                    _privileges = new Dictionary<string, Privilege>(response.EntityCollection.Entities.Count);

                    foreach(var entity in response.EntityCollection.Entities)
                    {
                        var typedPrivilege = entity.ToEntity<Privilege>();
                        _privileges.Add(typedPrivilege.Name, typedPrivilege);
                    }

                }
                return _privileges;
            }
        }

        public bool UnsubmittedRequests { 
            get; 
            set; 
        }

        //Staged Role Privileges
        private Dictionary<Guid, List<RolePrivilege>> _rolePrivileges = new Dictionary<Guid, List<RolePrivilege>>();
        public RolePrivilegeManager(IOrganizationService crmService, ILogger logger)
        {
            UnsubmittedRequests = false;
            _crmService = crmService;
            _logger = logger;
        }
        #endregion

        public void StagePrivilege (JsonEntity jsonEntity)
        {
            //Get the privilege Details
            var roleId = (Guid)jsonEntity["roleid"];
            RolePrivilege rolePrivilege = jsonEntity.ToRolePrivlege();
            Privilege privilege = Privileges[rolePrivilege.PrivilegeName];
            rolePrivilege.PrivilegeId = privilege.PrivilegeId.Value;

            if(!_rolePrivileges.ContainsKey(roleId))
            {
                _rolePrivileges[roleId] = new List<RolePrivilege>();
            }

            List<RolePrivilege> privileges = _rolePrivileges[roleId];
            privileges.Add(rolePrivilege);
            UnsubmittedRequests = true;
        }

        public void SubmitAddPrivilegesRequests(DataImportResult importResult)
        {
            foreach (var pair in _rolePrivileges)
            {
                AddPrivilegesRoleRequest request = new AddPrivilegesRoleRequest();
                request.Privileges = pair.Value.ToArray();
                request.RoleId = pair.Key;

                var response = _crmService.Execute(request);
                if (importResult != null)
                {
                    _logger.LogVerbose($"AddPrivilegesRoleRequest submitted for role '{request.RoleId}' with {request.Privileges.Length} privileges");
                    importResult.RecordsCreated++;
                }
            }

            UnsubmittedRequests = false;
        }
    }
}
