import { createContext, useContext, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();

  const backendUrl = 'https://localhost:7255';

  useEffect(() => {
    checkAuthStatus();
  }, []);

  const checkAuthStatus = async () => {
    try {
      const response = await fetch(backendUrl + '/api/account/me', {
        credentials: 'include'
      });

      if (response.status === 401) {
      return await refreshToken();
    }
      
      if (response.ok) {
        const userData = await response.json();
        setUser(userData);
      }
    } catch (error) {
      console.error('Auth check failed:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const refreshToken = async () => {
  try {
    const response = await fetch('/api/account/refresh', {
      method: 'POST',
      credentials: 'include'
    });
    
    if (response.ok) {
      return await checkAuthStatus();
    }
    
    return false;
  } catch (error) {
    console.error('Token refresh failed:', error);
    return false;
  }
};

  const login = async (email, password) => {
    try {
      const response = await fetch(backendUrl + '/api/account/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password }),
        credentials: 'include'
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Login failed');
      }

      if (process.env.NODE_ENV === 'development') {
      const cookies = response.headers.get('set-cookie');
      if (cookies) {
        cookies.split(',').forEach(cookie => {
          document.cookie = cookie;
        });
      }
    }
      await checkAuthStatus();
      navigate('/');
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  };

  const register = async (userData) => {
    try {
      const response = await fetch(backendUrl + '/api/account/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userData),
        credentials: 'include'
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Registration failed');
      }

      await login(userData.email, userData.password);
    } catch (error) {
      console.error('Registration error:', error);
      throw error;
    }
  };

  const logout = async () => {
    try {
      await fetch(backendUrl + '/api/account/logout', {
        method: 'POST',
        credentials: 'include'
      });
      setUser(null);
      navigate('/login');
    } catch (error) {
      console.error('Logout error:', error);
    }
  };

  const value = {
    user,
    isLoading,
    login,
    register,
    logout
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export const useAuth = () => useContext(AuthContext);