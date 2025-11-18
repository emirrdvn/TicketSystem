import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

const axiosInstance = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle errors
axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expired or invalid
      // Get user type before clearing localStorage
      const storedUser = localStorage.getItem('user');
      let userType = null;

      try {
        if (storedUser) {
          const parsedUser = JSON.parse(storedUser);
          userType = parsedUser.userType;
        }
      } catch (e) {
        // Ignore parse error
      }

      // Clear auth data
      localStorage.removeItem('token');
      localStorage.removeItem('user');

      // Redirect based on user type
      let redirectPath = '/auth/login'; // Default: Customer login

      // UserType enum values: Admin = 1, Technician = 2, Customer = 3
      if (userType === 1) { // Admin
        redirectPath = '/auth/admin-login';
      } else if (userType === 2) { // Technician
        redirectPath = '/auth/technician-login';
      }

      window.location.href = redirectPath;
    }
    return Promise.reject(error);
  }
);

export default axiosInstance;
export { API_URL };
