using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SGE.PieriDicacio.API.Authz;
using SGE.PieriDicacio.API.Controllers;

var failures = new List<string>();
var checks = 0;

void Check(bool condition, string description)
{
    checks++;
    if (!condition)
        failures.Add(description);
}

async Task<bool> IsAuthorizedAsync(IEnumerable<string> permissions, string requiredPermission)
{
    var claims = permissions.Select(permission => new Claim(PermissionRequirement.ClaimType, permission));
    var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    var requirement = new PermissionRequirement(requiredPermission);
    var context = new AuthorizationHandlerContext([requirement], user, null);

    await new PermissionHandler().HandleAsync(context);
    return context.HasSucceeded;
}

var policyProvider = new PermissionPolicyProvider(Options.Create(new AuthorizationOptions()));
var reportsPolicy = await policyProvider.GetPolicyAsync($"{PermissionRequirement.Prefix}Ver Reportes");
Check(reportsPolicy is not null, "El proveedor debe crear una política de permisos.");
Check(reportsPolicy!.Requirements.OfType<PermissionRequirement>().Single().Permission == "Ver Reportes",
    "La política debe conservar el nombre exacto del permiso.");
Check(reportsPolicy.Requirements.Any(requirement => requirement.GetType().Name == "DenyAnonymousAuthorizationRequirement"),
    "Las políticas de permisos deben exigir un usuario autenticado.");

Check(await IsAuthorizedAsync(["Ver Clientes"], "Ver Clientes"),
    "Un permiso directo debe autorizar la operación.");
Check(await IsAuthorizedAsync(["Gestionar Clientes"], "Ver Clientes"),
    "Gestionar Clientes debe incluir la capacidad de consultar clientes.");
Check(!await IsAuthorizedAsync(["Ver Clientes"], "Gestionar Clientes"),
    "Un permiso de consulta no debe autorizar modificaciones.");
Check(!await IsAuthorizedAsync(["Ver Inventario"], "Ver Reportes"),
    "Un permiso de otro módulo no debe autorizar reportes.");

var rolePermissions = new Dictionary<string, string[]>
{
    ["Administración"] =
    [
        "Ver Clientes", "Gestionar Clientes", "Ver Pedidos", "Crear Pedidos",
        "Cambiar Estado Pedido", "Ver Pagos", "Registrar Pagos", "Ver Reportes",
        "Gestionar Catálogos", "Ver Empleados"
    ],
    ["Producción"] =
    [
        "Ver Pedidos", "Cambiar Estado Pedido", "Ver Producción", "Gestionar Producción",
        "Gestionar Lista de Materiales", "Ver Inventario"
    ],
    ["Bodega"] = ["Ver Inventario", "Gestionar Inventario", "Ver Reportes"],
    ["RRHH"] = ["Ver Empleados", "Gestionar Empleados", "Registrar Asistencia", "Ver Reportes"]
};

var roleExpectations = new[]
{
    (Role: "Administración", Permission: "Ver Clientes", Allowed: true),
    (Role: "Administración", Permission: "Ver Pagos", Allowed: true),
    (Role: "Administración", Permission: "Ver Reportes", Allowed: true),
    (Role: "Administración", Permission: "Ver Inventario", Allowed: false),
    (Role: "Producción", Permission: "Ver Pedidos", Allowed: true),
    (Role: "Producción", Permission: "Ver Producción", Allowed: true),
    (Role: "Producción", Permission: "Ver Reportes", Allowed: false),
    (Role: "Bodega", Permission: "Ver Inventario", Allowed: true),
    (Role: "Bodega", Permission: "Ver Reportes", Allowed: true),
    (Role: "Bodega", Permission: "Gestionar Usuarios", Allowed: false),
    (Role: "RRHH", Permission: "Ver Empleados", Allowed: true),
    (Role: "RRHH", Permission: "Registrar Asistencia", Allowed: true),
    (Role: "RRHH", Permission: "Ver Clientes", Allowed: false)
};

foreach (var expectation in roleExpectations)
{
    var actual = await IsAuthorizedAsync(rolePermissions[expectation.Role], expectation.Permission);
    Check(actual == expectation.Allowed,
        $"El rol {expectation.Role} produjo un resultado incorrecto para {expectation.Permission}.");
}

bool HasPermission(Type controllerType, string? methodName, string permission)
{
    var expectedPolicy = $"{PermissionRequirement.Prefix}{permission}";
    var classHasPolicy = controllerType.GetCustomAttributes<AuthorizeAttribute>(true)
        .Any(attribute => attribute.Policy == expectedPolicy);
    if (classHasPolicy || methodName is null)
        return classHasPolicy;

    return controllerType.GetMethod(methodName)!
        .GetCustomAttributes<AuthorizeAttribute>(true)
        .Any(attribute => attribute.Policy == expectedPolicy);
}

var endpointExpectations = new[]
{
    (Controller: typeof(ClientesController), Method: nameof(ClientesController.ObtenerTodos), Permission: "Ver Clientes"),
    (Controller: typeof(PedidosController), Method: nameof(PedidosController.ObtenerTodos), Permission: "Ver Pedidos"),
    (Controller: typeof(PagosController), Method: nameof(PagosController.ObtenerPagosPorPedido), Permission: "Ver Pagos"),
    (Controller: typeof(InventarioController), Method: nameof(InventarioController.ObtenerProductosTerminados), Permission: "Ver Inventario"),
    (Controller: typeof(ProduccionController), Method: nameof(ProduccionController.ObtenerOrdenesProduccion), Permission: "Ver Producción"),
    (Controller: typeof(EmpleadosController), Method: nameof(EmpleadosController.ObtenerEmpleados), Permission: "Ver Empleados"),
    (Controller: typeof(UsuariosController), Method: nameof(UsuariosController.ObtenerTodos), Permission: "Gestionar Usuarios"),
    (Controller: typeof(ReportesController), Method: (string?)null, Permission: "Ver Reportes"),
    (Controller: typeof(BitacoraController), Method: (string?)null, Permission: "Ver Bitácora"),
    (Controller: typeof(BOMController), Method: (string?)null, Permission: "Gestionar Lista de Materiales"),
    (Controller: typeof(PermisosController), Method: (string?)null, Permission: "Gestionar Roles y Permisos")
};

foreach (var endpoint in endpointExpectations)
{
    Check(HasPermission(endpoint.Controller, endpoint.Method, endpoint.Permission),
        $"Falta proteger {endpoint.Controller.Name}.{endpoint.Method ?? "<controlador>"} con {endpoint.Permission}.");
}

void CheckAllGetEndpoints(Type controllerType, string permission)
{
    var getMethods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
        .Where(method => method.GetCustomAttributes<HttpGetAttribute>(true).Any())
        .ToList();

    Check(getMethods.Count > 0, $"{controllerType.Name} debe contener endpoints GET para comprobar.");
    foreach (var method in getMethods)
    {
        Check(HasPermission(controllerType, method.Name, permission),
            $"Falta proteger {controllerType.Name}.{method.Name} con {permission}.");
    }
}

CheckAllGetEndpoints(typeof(ClientesController), "Ver Clientes");
CheckAllGetEndpoints(typeof(PedidosController), "Ver Pedidos");
CheckAllGetEndpoints(typeof(PagosController), "Ver Pagos");
CheckAllGetEndpoints(typeof(InventarioController), "Ver Inventario");
CheckAllGetEndpoints(typeof(ProduccionController), "Ver Producción");
CheckAllGetEndpoints(typeof(EmpleadosController), "Ver Empleados");
CheckAllGetEndpoints(typeof(UsuariosController), "Gestionar Usuarios");
CheckAllGetEndpoints(typeof(ReportesController), "Ver Reportes");
CheckAllGetEndpoints(typeof(BOMController), "Gestionar Lista de Materiales");

if (failures.Count > 0)
{
    Console.Error.WriteLine($"Fallaron {failures.Count} de {checks} comprobaciones:");
    foreach (var failure in failures)
        Console.Error.WriteLine($"- {failure}");
    return 1;
}

Console.WriteLine($"Correcto: {checks} comprobaciones de autorización superadas.");
return 0;
