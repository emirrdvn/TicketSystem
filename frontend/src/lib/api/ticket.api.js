import axios from './axios';

export const ticketAPI = {
  // Get all tickets
  getAllTickets: async () => {
    const response = await axios.get('/ticket');
    return response.data;
  },

  // Get ticket by ID
  getTicketById: async (id) => {
    const response = await axios.get(`/ticket/${id}`);
    return response.data;
  },

  // Get ticket by number
  getTicketByNumber: async (ticketNumber) => {
    const response = await axios.get(`/ticket/number/${ticketNumber}`);
    return response.data;
  },

  // Get my tickets (customer)
  getMyTickets: async () => {
    const response = await axios.get('/ticket/my');
    return response.data;
  },

  // Get assigned tickets (technician)
  getAssignedTickets: async () => {
    const response = await axios.get('/ticket/assigned');
    return response.data;
  },

  // Get tickets by category
  getTicketsByCategory: async (categoryId) => {
    const response = await axios.get(`/ticket/category/${categoryId}`);
    return response.data;
  },

  // Get tickets by status
  getTicketsByStatus: async (status) => {
    const response = await axios.get(`/ticket/status/${status}`);
    return response.data;
  },

  // Get tickets by technician's categories
  getTicketsByMyCategories: async () => {
    const response = await axios.get('/ticket/my-categories');
    return response.data;
  },

  // Create ticket
  createTicket: async (ticketData) => {
    const response = await axios.post('/ticket', ticketData);
    return response.data;
  },

  // Update ticket status
  updateTicketStatus: async (ticketId, status, comment) => {
    const response = await axios.patch(`/ticket/${ticketId}/status`, {
      newStatus: status,
      comment
    });
    return response.data;
  },

  // Assign ticket to technician
  assignTicket: async (ticketId, technicianId) => {
    const response = await axios.patch(`/ticket/${ticketId}/assign/${technicianId}`);
    return response.data;
  },

  // Delete ticket
  deleteTicket: async (ticketId) => {
    await axios.delete(`/ticket/${ticketId}`);
  },

  // Get ticket messages
  getTicketMessages: async (ticketId) => {
    const response = await axios.get(`/ticket/${ticketId}/messages`);
    return response.data;
  },

  // Send message
  sendMessage: async (ticketId, message) => {
    const response = await axios.post('/ticket/messages', {
      ticketId,
      message
    });
    return response.data;
  }
};
