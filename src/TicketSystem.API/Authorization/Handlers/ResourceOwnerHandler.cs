using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TicketSystem.API.Authorization.Requirements;

namespace TicketSystem.API.Authorization.Handlers;

/// <summary>
/// Handles authorization for resource ownership.
/// Allows access if user is:
/// 1. Admin
/// 2. The resource owner (userId matches resource userId)
/// </summary>
public class ResourceOwnerHandler : AuthorizationHandler<ResourceOwnerRequirement, Guid>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOwnerRequirement requirement,
        Guid resourceUserId)
    {
        // Get user claims
        var userIdClaim = context.User.FindFirst("userId")?.Value;
        var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (!Guid.TryParse(userIdClaim, out var currentUserId))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Admins can access any resource
        if (roleClaim == "Admin")
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Allow access if user owns the resource
        if (currentUserId == resourceUserId)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // If none of the conditions are met, fail the authorization
        context.Fail();
        return Task.CompletedTask;
    }
}
