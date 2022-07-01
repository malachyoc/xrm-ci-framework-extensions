using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm.Framework.CI.Extensions.DataOperations;

namespace Xrm.Framework.CI.Extensions.Entities
{
    public static class RolePrivilegeExtensions
    {
        public const int BASIC_MASK = 0x00000001; 
        public const int LOCAL_MASK = 0x00000002; 
        public const int DEEP_MASK = 0x00000004; 
        public const int GLOBAL_MASK = 0x00000008;

        /// <summary>
        /// Return Data Structure contains privilege details
        /// </summary>
        /// <param name="baseEntity"></param>
        public static RolePrivilege ToRolePrivlege(this JsonEntity baseEntity)
        {
            if (baseEntity == null)
                return null;

            RolePrivilege rp = new RolePrivilege();
            rp.Depth = GetPrivilegeDepth((int)baseEntity["privilegedepthmask"]);
            rp.PrivilegeName = (string)baseEntity["privilegename"];

            return rp;
        }

        private static PrivilegeDepth GetPrivilegeDepth(int privilegeDeptMask)
        {
            switch (privilegeDeptMask)
            {
                case BASIC_MASK:
                    return PrivilegeDepth.Basic;

                case LOCAL_MASK:
                    return PrivilegeDepth.Local;

                case DEEP_MASK:
                    return PrivilegeDepth.Deep;

                case GLOBAL_MASK:
                    return PrivilegeDepth.Global;

                default:
                    throw new ArgumentException($"{privilegeDeptMask} is not a vaild privilege dept");
            }
        }
    }
}
