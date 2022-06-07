using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;

namespace Sps.Pos.Mvc.Core
{
	public static class PrincipalExtention
	{
		public static string GetUserId(this IPrincipal user) =>
			((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.NameIdentifier)?.Value;

		public static string GetFullName(this IPrincipal user) =>
			((ClaimsIdentity)user.Identity).FindFirst("FullName")?.Value;

		public static DateTime? GetLastLogin(this IPrincipal user, string dateFormat, string dateTimeFormat)
		{
			var lastLogin = ((ClaimsIdentity)user.Identity).FindFirst("LastLogin")?.Value;
			var dt = lastLogin.ToDateTime(new string[] { dateFormat, dateTimeFormat });
			return dt;
		}

		public static string GetUserName(this IPrincipal user) =>
			((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.Name)?.Value;

		public static string GetMobileNumber(this IPrincipal user) =>
			 ((ClaimsIdentity)user.Identity).FindFirst("MobileNumber")?.Value;

		public static string GetEmail(this IPrincipal user) =>
			((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.Email)?.Value;

		public static List<string> GetRoleIds(this IPrincipal user) =>
			((ClaimsIdentity)user.Identity).FindAll("RoleId").Select(x => x.Value)?.ToList();

		public static List<string> GetRoleNames(this IPrincipal user)
		{
			return ((ClaimsIdentity)user.Identity).FindAll(ClaimTypes.Role).Select(x => x.Value)?.ToList();
		}

		public static List<string> GetPermissions(this IPrincipal user)
		{
			return ((ClaimsIdentity)user.Identity).FindAll("Permission").Select(x => x.Value)?.ToList();
		}

		public static List<string> GetPermissionIds(this IPrincipal user)
		{
			return ((ClaimsIdentity)user.Identity).FindAll("PermissionId").Select(x => x.Value)?.ToList();
		}

		public static string GetCommaSeparatedPermission(this IPrincipal user)
		{
			return ((ClaimsIdentity)user.Identity).FindFirst("CommaSeparatedPermission")?.Value;
		}

		public static bool HasRoleId(this IPrincipal user, string roleId) =>
			!string.IsNullOrWhiteSpace(user.GetRoleIds().FirstOrDefault(x => x.Equals(roleId, StringComparison.OrdinalIgnoreCase)));

		public static bool HasRoleName(this IPrincipal user, string roleName) =>
			!string.IsNullOrWhiteSpace(user.GetRoleNames().FirstOrDefault(x => x.Equals(roleName, StringComparison.OrdinalIgnoreCase)));

		public static bool HasPermission(this IPrincipal user, int permissionId)
		{
			return user?.GetPermissionIds().FirstOrDefault(x => x == permissionId.ToString()) != null;
		}

		public static string GetAuthenticationToken(this IPrincipal user) =>
			((ClaimsIdentity)user.Identity).FindFirst("AuthenticationToken")?.Value;

		public static DateTime? ToDateTime(this string dateTimeStr, params string[] dateFmt)
		{
			const DateTimeStyles style = DateTimeStyles.AllowWhiteSpaces;
			if (dateFmt == null)
			{
				var dateInfo = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
				dateFmt = dateInfo.GetAllDateTimePatterns();
			}

			var result = DateTime.TryParseExact(dateTimeStr, dateFmt, CultureInfo.InvariantCulture,
						 style, out var dt) ? dt : null as DateTime?;

			return result;
		}

	}
}
