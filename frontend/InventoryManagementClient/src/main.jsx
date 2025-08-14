import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import MainPage from './pages/MainPage'
import NotFoundPage from './pages/NotFoundPage'
import { createBrowserRouter, RouterProvider } from 'react-router'

const router = createBrowserRouter([
  {path: "/", element: <MainPage />},
  {path: "/home", element: <MainPage />},
  {path: "/login", element: <LoginPage />},
  {path: "/register", element: <RegisterPage />},
  {path: "*", element: <NotFoundPage />},
]);

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <RouterProvider router={router}/>
  </StrictMode>,
)
