import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import AuthGuard from './guards/AuthGuard';
import GuestGuard from './guards/GuestGuard';
import RoleGuard from './guards/RoleGuard';
import { UserType } from './types';

// Layouts
import Navbar from './components/layouts/Navbar';

// Auth Pages
import LoginPage from './pages/auth/LoginPage';
import RegisterPage from './pages/auth/RegisterPage';
import AdminLoginPage from './pages/auth/AdminLoginPage';
import TechnicianLoginPage from './pages/auth/TechnicianLoginPage';

// Customer Pages
import CustomerDashboard from './pages/customer/CustomerDashboard';
import CreateTicketPage from './pages/customer/CreateTicketPage';
import MyTicketsPage from './pages/customer/MyTicketsPage';
import TicketDetailPage from './pages/customer/TicketDetailPage';

// Admin Pages
import AdminDashboard from './pages/admin/AdminDashboard';
import AllTicketsPage from './pages/admin/AllTicketsPage';
import UsersManagementPage from './pages/admin/UsersManagementPage';
import TechniciansManagementPage from './pages/admin/TechniciansManagementPage';

// Technician Pages
import TechnicianDashboard from './pages/technician/TechnicianDashboard';

function App() {
  return (
    <AuthProvider>
      <Router>
        <div className="min-h-screen bg-gray-50">
          <Navbar />
          <Routes>
            {/* Public Routes */}
            <Route path="/" element={<Navigate to="/auth/login" replace />} />

            {/* Auth Routes */}
            <Route
              path="/auth/login"
              element={
                <GuestGuard>
                  <LoginPage />
                </GuestGuard>
              }
            />
            <Route
              path="/auth/admin-login"
              element={
                <GuestGuard>
                  <AdminLoginPage />
                </GuestGuard>
              }
            />
            <Route
              path="/auth/technician-login"
              element={
                <GuestGuard>
                  <TechnicianLoginPage />
                </GuestGuard>
              }
            />
            <Route
              path="/auth/register"
              element={
                <GuestGuard>
                  <RegisterPage />
                </GuestGuard>
              }
            />

            {/* Admin Routes */}
            <Route
              path="/admin"
              element={
                <AuthGuard>
                  <RoleGuard allowedRoles={[UserType.Admin]}>
                    <AdminDashboard />
                  </RoleGuard>
                </AuthGuard>
              }
            />
            <Route
              path="/admin/tickets"
              element={
                <AuthGuard>
                  <RoleGuard allowedRoles={[UserType.Admin]}>
                    <AllTicketsPage />
                  </RoleGuard>
                </AuthGuard>
              }
            />
            <Route
              path="/admin/tickets/:id"
              element={
                <AuthGuard>
                  <RoleGuard allowedRoles={[UserType.Admin]}>
                    <TicketDetailPage />
                  </RoleGuard>
                </AuthGuard>
              }
            />
            <Route
              path="/admin/users"
              element={
                <AuthGuard>
                  <RoleGuard allowedRoles={[UserType.Admin]}>
                    <UsersManagementPage />
                  </RoleGuard>
                </AuthGuard>
              }
            />
            <Route
              path="/admin/technicians"
              element={
                <AuthGuard>
                  <RoleGuard allowedRoles={[UserType.Admin]}>
                    <TechniciansManagementPage />
                  </RoleGuard>
                </AuthGuard>
              }
            />

            {/* Technician Routes */}
            <Route
              path="/technician"
              element={
                <AuthGuard>
                  <RoleGuard allowedRoles={[UserType.Technician]}>
                    <TechnicianDashboard />
                  </RoleGuard>
                </AuthGuard>
              }
            />
            <Route
              path="/technician/tickets/:id"
              element={
                <AuthGuard>
                  <RoleGuard allowedRoles={[UserType.Technician]}>
                    <TicketDetailPage />
                  </RoleGuard>
                </AuthGuard>
              }
            />

            {/* Customer Routes */}
            <Route
              path="/customer"
              element={
                <AuthGuard>
                  <RoleGuard allowedRoles={[UserType.Customer]}>
                    <CustomerDashboard />
                  </RoleGuard>
                </AuthGuard>
              }
            />
            <Route
              path="/customer/tickets"
              element={
                <AuthGuard>
                  <RoleGuard allowedRoles={[UserType.Customer]}>
                    <MyTicketsPage />
                  </RoleGuard>
                </AuthGuard>
              }
            />
            <Route
              path="/customer/tickets/new"
              element={
                <AuthGuard>
                  <RoleGuard allowedRoles={[UserType.Customer]}>
                    <CreateTicketPage />
                  </RoleGuard>
                </AuthGuard>
              }
            />
            <Route
              path="/customer/tickets/:id"
              element={
                <AuthGuard>
                  <RoleGuard allowedRoles={[UserType.Customer]}>
                    <TicketDetailPage />
                  </RoleGuard>
                </AuthGuard>
              }
            />

            {/* 404 */}
            <Route path="*" element={<Navigate to="/auth/login" replace />} />
          </Routes>
        </div>
      </Router>
    </AuthProvider>
  );
}

export default App;
