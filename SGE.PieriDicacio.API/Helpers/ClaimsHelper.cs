using System.Security.Claims;

namespace SGE.PieriDicacio.API.Helpers
{
    public static class ClaimsHelper
    {
        public static int GetUserId(this IEnumerable<Claim> claims)
        {
            var id = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(id, out var userId) ? userId : 0;
        }

        public static string GetUsername(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        }

        public static string GetRole(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        }

        public static int GetUserIdFromHttpContext(this HttpContext context)
        {
            return context.User.Claims.GetUserId();
        }
    }
}
