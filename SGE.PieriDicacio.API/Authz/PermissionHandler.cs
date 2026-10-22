using Microsoft.AspNetCore.Authorization;

namespace SGE.PieriDicacio.API.Authz
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private static readonly IReadOnlyDictionary<string, string[]> ImpliedBy =
            new Dictionary<string, string[]>(StringComparer.Ordinal)
            {
                ["Ver Clientes"] = ["Gestionar Clientes"],
                ["Ver Pedidos"] = ["Crear Pedidos", "Cambiar Estado Pedido"],
                ["Ver Pagos"] = ["Registrar Pagos"],
                ["Ver Inventario"] = ["Gestionar Inventario"],
                ["Ver Producción"] = ["Gestionar Producción"],
                ["Ver Empleados"] = ["Gestionar Empleados", "Registrar Asistencia"]
            };

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var grantedPermissions = context.User.FindAll(PermissionRequirement.ClaimType)
                .Select(claim => claim.Value)
                .ToHashSet(StringComparer.Ordinal);

            var hasPermission = grantedPermissions.Contains(requirement.Permission)
                || (ImpliedBy.TryGetValue(requirement.Permission, out var strongerPermissions)
                    && strongerPermissions.Any(grantedPermissions.Contains));

            if (hasPermission)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
