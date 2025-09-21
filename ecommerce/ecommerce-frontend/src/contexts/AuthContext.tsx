import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { User, AuthContextType, CreateUserDto } from '../interfaces/User';
import authService from '../services/authService';
import { parseJwtToken, isTokenExpired, getUserFromToken } from '../utils/jwtUtils';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Check if user is already authenticated on app start
    const initializeAuth = async () => {
      try {
        const token = authService.getToken();
        console.log('Initializing auth with token:', token ? 'exists' : 'null');
        
        if (token) {
          // 检查token是否过期
          if (isTokenExpired(token)) {
            console.log('Token is expired, clearing auth');
            // Token已过期，清除本地存储
            await authService.logout();
            setUser(null);
          } else {
            console.log('Token is valid, parsing user info');
            // Token有效，解析用户信息
            const userInfo = getUserFromToken(token);
            if (userInfo) {
              console.log('User info parsed:', userInfo);
              // 创建一个基本的用户对象
              const user: User = {
                id: userInfo.id,
                userName: userInfo.email.split('@')[0], // 从邮箱提取用户名
                email: userInfo.email,
                firstName: '',
                lastName: '',
                phoneNumber: '',
                address: '',
                isActive: true,
                createdAt: '',
                updatedAt: '',
                role: userInfo.role
              };
              setUser(user);
              console.log('User state set:', user);
            } else {
              console.log('Failed to parse user info from token');
              // 如果无法解析用户信息，清除token
              await authService.logout();
              setUser(null);
            }
          }
        } else {
          console.log('No token found');
          setUser(null);
        }
      } catch (error) {
        console.error('Auth initialization error:', error);
        // 如果初始化失败，清除可能损坏的token
        await authService.logout();
        setUser(null);
      } finally {
        setLoading(false);
      }
    };

    initializeAuth();
  }, []);

  const login = async (email: string, password: string): Promise<boolean> => {
    try {
      console.log('Attempting login for:', email);
      const response = await authService.login({ email, password });
      console.log('Login response:', response);
      
      if (response && response.user) {
        setUser(response.user);
        console.log('User state updated after login:', response.user);
        return true;
      } else {
        console.error('Login response missing user data:', response);
        return false;
      }
    } catch (error) {
      console.error('Login error:', error);
      return false;
    }
  };

  const register = async (userData: CreateUserDto): Promise<boolean> => {
    try {
      console.log('Attempting registration for:', userData.email);
      const response = await authService.register(userData);
      console.log('Registration response:', response);
      
      if (response && response.user) {
        setUser(response.user);
        console.log('User state updated after registration:', response.user);
        return true;
      } else {
        console.error('Registration response missing user data:', response);
        return false;
      }
    } catch (error) {
      console.error('Registration error:', error);
      return false;
    }
  };

  const logout = async () => {
    try {
      console.log('Attempting logout');
      await authService.logout();
      console.log('Logout successful');
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      setUser(null);
      // 确保清除所有认证状态
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      console.log('User state cleared and tokens removed');
    }
  };

  const value: AuthContextType = {
    user,
    login,
    register,
    logout,
    isAuthenticated: !!user,
    loading,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
