import axios from './axios';

export const attachmentAPI = {
  uploadAttachment: async (file, ticketId, messageId = null) => {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('ticketId', ticketId);
    if (messageId) {
      formData.append('messageId', messageId);
    }

    // Override the default Content-Type to let browser set multipart/form-data with boundary
    const { data } = await axios.post('/attachment/upload', formData, {
      headers: {
        'Content-Type': null
      },
      transformRequest: [(data) => data] // Prevent axios from processing FormData
    });
    return data;
  },

  getTicketAttachments: async (ticketId) => {
    const { data } = await axios.get(`/attachment/ticket/${ticketId}`);
    return data;
  },

  deleteAttachment: async (attachmentId) => {
    const { data } = await axios.delete(`/attachment/${attachmentId}`);
    return data;
  },

  getAttachmentUrl: (fileUrl) => {
    return `${import.meta.env.VITE_API_URL || 'http://localhost:5000'}${fileUrl}`;
  }
};
