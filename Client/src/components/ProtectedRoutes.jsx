import { useAuth } from '../contexts/AuthContext'
import { useLocation, Navigate, Outlet } from 'react-router-dom'
function ProtectedRoutes({ allowedRoles }) {
  const { user } = useAuth()
  console.log(user)
  const location = useLocation()
  return user?.roles?.find((role) => allowedRoles?.includes(role)) ? (
    <Outlet />
  ) : user ? (
    <Navigate to="/unauthorized" state={{ from: location }} replace />
  ) : (
    <Navigate to="/login" state={{ from: location }} replace />
  )
}

export default ProtectedRoutes
