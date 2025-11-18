using TicketSystem.Application.DTOs.Request;
using TicketSystem.Application.DTOs.Response;
using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Services.Ticket;

public interface ITicketService
{
    Task<TicketResponse> CreateTicketAsync(CreateTicketRequest request, Guid customerId);
    Task<TicketResponse> GetTicketByIdAsync(int ticketId);
    Task<TicketResponse> GetTicketByNumberAsync(string ticketNumber);
    Task<IEnumerable<TicketResponse>> GetAllTicketsAsync();
    Task<IEnumerable<TicketResponse>> GetTicketsByCustomerAsync(Guid customerId);
    Task<IEnumerable<TicketResponse>> GetTicketsByTechnicianAsync(Guid technicianId);
    Task<IEnumerable<TicketResponse>> GetTicketsByCategoryAsync(int categoryId);
    Task<IEnumerable<TicketResponse>> GetTicketsByStatusAsync(TicketStatus status);
    Task<TicketResponse> UpdateTicketStatusAsync(UpdateTicketStatusRequest request, Guid userId);
    Task<TicketResponse> AssignTicketAsync(int ticketId, Guid technicianId);
    Task<bool> DeleteTicketAsync(int ticketId);
    Task<IEnumerable<TicketMessageResponse>> GetTicketMessagesAsync(int ticketId);
    Task<TicketMessageResponse> SendMessageAsync(SendMessageRequest request, Guid senderId);
}
