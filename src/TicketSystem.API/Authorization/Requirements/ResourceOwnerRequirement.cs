using Microsoft.AspNetCore.Authorization;

namespace TicketSystem.API.Authorization.Requirements;

/// <summary>
/// Authorization requirement for resource ownership validation.
/// Ensures that users can only access or modify their own resources.
/// Admins can bypass this requirement.
/// </summary>
public class ResourceOwnerRequirement : IAuthorizationRequirement
{
    public ResourceOwnerRequirement()
    {
    }
}
