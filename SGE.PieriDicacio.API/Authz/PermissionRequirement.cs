using Microsoft.AspNetCore.Authorization;

namespace SGE.PieriDicacio.API.Authz
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public const string Prefix = "Permiso:";
        public const string ClaimType = "Permiso";

        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
