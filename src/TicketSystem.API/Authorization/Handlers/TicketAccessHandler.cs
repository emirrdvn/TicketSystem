using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketSystem.API.Authorization.Requirements;
using TicketSystem.Infrastructure.Data;

namespace TicketSystem.API.Authorization.Handlers;

/// <summary>
/// Handles authorization for ticket access.
/// Allows access if user is:
/// 1. Admin
/// 2. Ticket owner (customer who created the ticket)
/// 3. Assigned technician
/// </summary>
public class TicketAccessHandler : AuthorizationHandler<TicketAccessRequirement, int>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TicketAccessHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TicketAccessRequirement requirement,
        int ticketId)
    {
        // Get user claims
        var userIdClaim = context.User.FindFirst("userId")?.Value;
        var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            context.Fail();
            return;
        }

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            context.Fail();
            return;
        }

        // Admins and Technicians have access to all tickets
        if (roleClaim == "Admin" || roleClaim == "Technician")
        {
            context.Succeed(requirement);
            return;
        }

        // Check ticket ownership or assignment
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var ticket = await dbContext.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                context.Fail();
                return;
            }

            // Allow access if user is the customer who created the ticket
            if (ticket.CustomerId == userId)
            {
                context.Succeed(requirement);
                return;
            }

            // Allow access if user is the assigned technician
            if (ticket.AssignedTechnicianId.HasValue && ticket.AssignedTechnicianId.Value == userId)
            {
                context.Succeed(requirement);
                return;
            }
        }

        // If none of the conditions are met, fail the authorization
        context.Fail();
    }
}
