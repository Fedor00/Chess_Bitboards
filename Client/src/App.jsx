import 'bootstrap/dist/css/bootstrap.min.css'
import './App.css'
import { AuthProvider } from './contexts/AuthContext'

import { BrowserRouter, Routes, Route } from 'react-router-dom'
import PageNotFound from './pages/PageNotFound'
import Unauthorized from './pages/Unauthorized'
import Homepage from './pages/Homepage'
import Login from './pages/Login'
import ProtectedRoutes from './components/ProtectedRoutes'
import Play from './components/Play'
import Register from './pages/Register'
import { ChessSignalRProvider } from './contexts/SignalRContext'
function App() {
  return (
    <AuthProvider>
      <ChessSignalRProvider>
        <BrowserRouter>
          <Routes>
            <Route path="login" element={<Login />} />
            <Route path="register" element={<Register />} />
            <Route path="unauthorized" element={<Unauthorized />} />
            <Route element={<ProtectedRoutes allowedRoles={['User']} />}>
              <Route index element={<Homepage />}></Route>
              <Route path="user-play" element={<Play></Play>} />
              <Route path="games" element={<div></div>} />
            </Route>
            <Route path="*" element={<PageNotFound />} />
          </Routes>
        </BrowserRouter>
      </ChessSignalRProvider>
    </AuthProvider>
  )
}

export default App
