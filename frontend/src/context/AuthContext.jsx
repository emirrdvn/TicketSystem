import { createContext, useState, useContext, useEffect } from 'react';
import { authAPI } from '../lib/api/auth.api';
import { UserType } from '../types';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  // Load user from localStorage on mount
  useEffect(() => {
    const token = localStorage.getItem('token');
    const storedUser = localStorage.getItem('user');

    if (token && storedUser) {
      try {
        const parsedUser = JSON.parse(storedUser);
        setUser(parsedUser);
        setIsAuthenticated(true);
      } catch (error) {
        console.error('Error parsing stored user:', error);
        localStorage.removeItem('token');
        localStorage.removeItem('user');
      }
    }
    setLoading(false);
  }, []);

  const login = async (email, password) => {
    try {
      const response = await authAPI.login(email, password);

      // Store token and user info
      localStorage.setItem('token', response.token);
      localStorage.setItem('user', JSON.stringify({
        userId: response.userId,
        email: response.email,
        fullName: response.fullName,
        userType: response.userType
      }));

      setUser({
        userId: response.userId,
        email: response.email,
        fullName: response.fullName,
        userType: response.userType
      });
      setIsAuthenticated(true);

      return response;
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  };

  const register = async (fullName, email, phoneNumber, password) => {
    try {
      const response = await authAPI.register(fullName, email, phoneNumber, password);

      // Store token and user info
      localStorage.setItem('token', response.token);
      localStorage.setItem('user', JSON.stringify({
        userId: response.userId,
        email: response.email,
        fullName: response.fullName,
        userType: response.userType
      }));

      setUser({
        userId: response.userId,
        email: response.email,
        fullName: response.fullName,
        userType: response.userType
      });
      setIsAuthenticated(true);

      return response;
    } catch (error) {
      console.error('Register error:', error);
      throw error;
    }
  };

  const logout = () => {
    // Kullanıcının tipini al (logout yapmadan önce)
    const currentUserType = user?.userType;

    // Logout yap
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
    setIsAuthenticated(false);

    // Kullanıcı tipine göre yönlendir
    let redirectPath = '/auth/login'; // Default: Customer login

    if (currentUserType === UserType.Admin) {
      redirectPath = '/auth/admin-login';
    } else if (currentUserType === UserType.Technician) {
      redirectPath = '/auth/technician-login';
    }

    // Yönlendirme için window.location kullan (AuthProvider içinde navigate kullanamayız)
    window.location.href = redirectPath;
  };

  const isAdmin = () => user?.userType === UserType.Admin;
  const isTechnician = () => user?.userType === UserType.Technician;
  const isCustomer = () => user?.userType === UserType.Customer;

  const value = {
    user,
    loading,
    isAuthenticated,
    login,
    register,
    logout,
    isAdmin,
    isTechnician,
    isCustomer
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export default AuthContext;
