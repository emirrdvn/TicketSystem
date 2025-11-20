using Microsoft.AspNetCore.Authorization;

namespace TicketSystem.API.Authorization.Requirements;

/// <summary>
/// Authorization requirement for ticket access validation.
/// Ensures that only ticket owners, assigned technicians, or admins can access ticket data.
/// </summary>
public class TicketAccessRequirement : IAuthorizationRequirement
{
    public TicketAccessRequirement()
    {
    }
}
