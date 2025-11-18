import axios from './axios';

export const authAPI = {
  // Login
  login: async (email, password) => {
    const response = await axios.post('/auth/login', { email, password });
    return response.data;
  },

  // Register
  register: async (fullName, email, phoneNumber, password) => {
    const response = await axios.post('/auth/register', {
      fullName,
      email,
      phoneNumber,
      password
    });
    return response.data;
  },

  // Refresh Token
  refreshToken: async (refreshToken) => {
    const response = await axios.post('/auth/refresh-token', refreshToken);
    return response.data;
  }
};
