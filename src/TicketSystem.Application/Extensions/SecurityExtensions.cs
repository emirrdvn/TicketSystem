using TicketSystem.Domain.Entities;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Extensions;

/// <summary>
/// Security extension methods for validating resource ownership and access permissions
/// </summary>
public static class SecurityExtensions
{
    /// <summary>
    /// Validates if a user can access a ticket based on their role and relationship to the ticket
    /// </summary>
    /// <param name="ticket">The ticket to check access for</param>
    /// <param name="userId">The user attempting to access the ticket</param>
    /// <param name="userRole">The role of the user (Admin, Technician, Customer)</param>
    /// <returns>True if user has access, false otherwise</returns>
    public static bool CanUserAccessTicket(this Ticket ticket, Guid userId, string userRole)
    {
        if (ticket == null)
            throw new ArgumentNullException(nameof(ticket));

        // Admins and Technicians can access all tickets
        if (userRole == "Admin" || userRole == "Technician")
            return true;

        // Customer who created the ticket can access it
        if (ticket.CustomerId == userId)
            return true;

        // No access for other roles
        return false;
    }

    /// <summary>
    /// Validates if a user can modify a ticket's status
    /// </summary>
    /// <param name="ticket">The ticket to check</param>
    /// <param name="userId">The user attempting to modify</param>
    /// <param name="userRole">The role of the user</param>
    /// <returns>True if user can modify, false otherwise</returns>
    public static bool CanUserModifyTicket(this Ticket ticket, Guid userId, string userRole)
    {
        if (ticket == null)
            throw new ArgumentNullException(nameof(ticket));

        // Admins and Technicians can modify all tickets
        if (userRole == "Admin" || userRole == "Technician")
            return true;

        // Ticket owner can modify their ticket
        if (ticket.CustomerId == userId)
            return true;

        return false;
    }

    /// <summary>
    /// Validates if a user can view another user's profile
    /// </summary>
    /// <param name="targetUserId">The user ID of the profile being viewed</param>
    /// <param name="currentUserId">The user ID attempting to view</param>
    /// <param name="currentUserRole">The role of the current user</param>
    /// <returns>True if user can view profile, false otherwise</returns>
    public static bool CanViewUserProfile(Guid targetUserId, Guid currentUserId, string currentUserRole)
    {
        // Admins can view all profiles
        if (currentUserRole == "Admin")
            return true;

        // Users can view their own profile
        if (targetUserId == currentUserId)
            return true;

        return false;
    }

    /// <summary>
    /// Validates if a user can modify another user's profile
    /// </summary>
    /// <param name="targetUserId">The user ID of the profile being modified</param>
    /// <param name="currentUserId">The user ID attempting to modify</param>
    /// <param name="currentUserRole">The role of the current user</param>
    /// <returns>True if user can modify profile, false otherwise</returns>
    public static bool CanModifyUserProfile(Guid targetUserId, Guid currentUserId, string currentUserRole)
    {
        // Admins can modify all profiles
        if (currentUserRole == "Admin")
            return true;

        // Users can modify their own profile
        if (targetUserId == currentUserId)
            return true;

        return false;
    }

    /// <summary>
    /// Validates if a status change is allowed based on user role and current ticket state
    /// </summary>
    /// <param name="ticket">The ticket being updated</param>
    /// <param name="newStatus">The new status to set</param>
    /// <param name="userRole">The role of the user making the change</param>
    /// <returns>True if status change is allowed, false otherwise</returns>
    public static bool IsStatusChangeAllowed(this Ticket ticket, TicketStatus newStatus, string userRole)
    {
        if (ticket == null)
            throw new ArgumentNullException(nameof(ticket));

        // Admins can make any status change
        if (userRole == "Admin")
            return true;

        // Prevent customers from changing status to InProgress (only technicians/admins)
        if (userRole == "Customer" && newStatus == TicketStatus.InProgress)
            return false;

        // Prevent reopening closed tickets (except by admins)
        if (ticket.Status == TicketStatus.Closed && userRole != "Admin")
            return false;

        return true;
    }
}
