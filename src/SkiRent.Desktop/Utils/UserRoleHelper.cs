using SkiRent.Shared.Contracts.Common;

namespace SkiRent.Desktop.Utils
{
    public static class UserRoleHelper
    {
        private static readonly Dictionary<RoleTypes, string> LocalizedUserRoles = new()
        {
            { RoleTypes.Admin, "Admin" },
            { RoleTypes.Customer, "Vásárló" }
        };

        private static readonly Dictionary<string, RoleTypes> UserRoleFromString = new()
        {
            { "Admin", RoleTypes.Admin },
            { "Vásárló", RoleTypes.Customer }
        };

        public static string GetLocalizedString(RoleTypes role)
        {
            return LocalizedUserRoles.TryGetValue(role, out var localizedString)
                ? localizedString
                : role.ToString();
        }

        public static RoleTypes GetUserRoleFromLocalizedString(string localizedUserRole)
        {
            return UserRoleFromString.TryGetValue(localizedUserRole, out var status)
                ? status
                : throw new ArgumentException("Invalid localized status string", nameof(localizedUserRole));
        }

        public static IEnumerable<string> GetAllLocalizedUserRoles()
        {
            return LocalizedUserRoles.Values;
        }
    }
}
