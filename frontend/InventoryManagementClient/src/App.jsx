import { Routes, Route } from 'react-router-dom';
import { Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import MainPage from './pages/MainPage';
import ProtectedRoute from './components/ProtectedRoute';
import NotFoundPage from './pages/NotFoundPage';

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      
      <Route element={<ProtectedRoute />}>
        <Route path="/" element={<MainPage />} />
        <Route path="/home" element={<MainPage />} />
      </Route>
      
      <Route path="*" element={<NotFoundPage/>} />
    </Routes>
  );
}

export default App;