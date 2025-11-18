import axios from './axios';

export const userAPI = {
  // Get all users
  getAllUsers: async () => {
    const response = await axios.get('/user');
    return response.data;
  },

  // Get user by ID
  getUserById: async (id) => {
    const response = await axios.get(`/user/${id}`);
    return response.data;
  },

  // Get all technicians
  getTechnicians: async () => {
    const response = await axios.get('/user/technicians');
    return response.data;
  },

  // Create user
  createUser: async (userData) => {
    const response = await axios.post('/user', userData);
    return response.data;
  },

  // Update user
  updateUser: async (id, userData) => {
    const response = await axios.put(`/user/${id}`, userData);
    return response.data;
  },

  // Delete user
  deleteUser: async (id) => {
    await axios.delete(`/user/${id}`);
  },

  // Activate user
  activateUser: async (id) => {
    const response = await axios.patch(`/user/${id}/activate`);
    return response.data;
  },

  // Deactivate user
  deactivateUser: async (id) => {
    const response = await axios.patch(`/user/${id}/deactivate`);
    return response.data;
  },

  // Get users by type
  getUsersByType: async (userType) => {
    const response = await axios.get(`/user/type/${userType}`);
    return response.data;
  },

  // Get technician categories
  getTechnicianCategories: async (technicianId) => {
    const response = await axios.get(`/user/${technicianId}/categories`);
    return response.data;
  },

  // Assign categories to technician
  assignCategoriesToTechnician: async (technicianId, categoryIds) => {
    const response = await axios.post(`/user/${technicianId}/categories`, categoryIds);
    return response.data;
  }
};
