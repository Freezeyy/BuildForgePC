using BuildForgePcStore.Models;

namespace BuildForgePcStore.Helpers;

public static class SessionExtensions
{
    public static void SetUserSession(this ISession session, int userId, string name, string role)
    {
        session.SetInt32(SessionKeys.UserId, userId);
        session.SetString(SessionKeys.UserName, name);
        session.SetString(SessionKeys.UserRole, role);
    }

    public static void ClearUserSession(this ISession session)
    {
        session.Remove(SessionKeys.UserId);
        session.Remove(SessionKeys.UserName);
        session.Remove(SessionKeys.UserRole);
    }

    public static int? GetUserId(this ISession session) =>
        session.GetInt32(SessionKeys.UserId);

    public static string? GetUserName(this ISession session) =>
        session.GetString(SessionKeys.UserName);

    public static string? GetUserRole(this ISession session) =>
        session.GetString(SessionKeys.UserRole);

    public static bool IsLoggedIn(this ISession session) =>
        session.GetUserId().HasValue;

    public static bool IsAdmin(this ISession session) =>
        string.Equals(session.GetUserRole(), UserRoles.Admin, StringComparison.Ordinal);

    public static bool IsCustomer(this ISession session) =>
        string.Equals(session.GetUserRole(), UserRoles.Customer, StringComparison.Ordinal);
}
