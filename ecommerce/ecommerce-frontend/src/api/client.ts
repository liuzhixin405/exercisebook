import axios from 'axios';
import { apiConfig } from './config';
import authService from '../services/authService';

// 创建axios实例
const apiClient = axios.create({
  baseURL: apiConfig.baseURL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// 请求拦截器
apiClient.interceptors.request.use(
  (config) => {
    // 添加认证token
    const token = authService.getToken();
    if (token) {
      config.headers = config.headers ?? {};
      (config.headers as any)['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// 响应拦截器
let isRefreshing = false;
let pendingRequests: Array<(token: string) => void> = [];

apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const originalRequest = error.config;

    // 若未授权，尝试刷新令牌后重试一次
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        if (!isRefreshing) {
          isRefreshing = true;
          const refreshToken = authService.getRefreshToken();
          if (!refreshToken) {
            isRefreshing = false;
            return Promise.reject(error);
          }

          const refreshed = await authService.refreshTokenAsync({ refreshToken });
          isRefreshing = false;

          // 处理挂起请求
          pendingRequests.forEach((cb) => cb(refreshed.token));
          pendingRequests = [];
        } else {
          // 等待刷新完成
          const newToken: string = await new Promise((resolve) => {
            pendingRequests.push((token: string) => resolve(token));
          });
          (originalRequest.headers as any)['Authorization'] = `Bearer ${newToken}`;
          return apiClient(originalRequest);
        }

        // 刷新完成后使用新token重试原始请求
        const newToken = authService.getToken();
        if (newToken) {
          (originalRequest.headers as any)['Authorization'] = `Bearer ${newToken}`;
          return apiClient(originalRequest);
        }
      } catch (refreshError) {
        isRefreshing = false;
        pendingRequests = [];
        // 刷新失败，透传错误
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default apiClient;