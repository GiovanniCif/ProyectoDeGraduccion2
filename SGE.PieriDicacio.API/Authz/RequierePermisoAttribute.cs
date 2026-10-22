using Microsoft.AspNetCore.Authorization;

namespace SGE.PieriDicacio.API.Authz
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequierePermisoAttribute : AuthorizeAttribute
    {
        public RequierePermisoAttribute(string permiso)
        {
            Policy = $"{PermissionRequirement.Prefix}{permiso}";
        }
    }
}
