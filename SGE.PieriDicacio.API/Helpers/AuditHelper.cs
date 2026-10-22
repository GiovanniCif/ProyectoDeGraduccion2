namespace SGE.PieriDicacio.API.Helpers
{
    public static class AuditHelper
    {
        public static string? ObtenerIp(HttpContext? httpContext)
        {
            if (httpContext == null)
                return null;

            var ip = httpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(ip))
                return null;

            return ip == "::1" ? "127.0.0.1" : ip;
        }
    }
}
