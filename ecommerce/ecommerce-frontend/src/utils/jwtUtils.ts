/**
 * JWT Token 工具函数
 */

interface JwtPayload {
  sub: string; // 用户ID
  email: string;
  role: string;
  exp: number; // 过期时间
  iat: number; // 签发时间
  [key: string]: any;
}

/**
 * 解析JWT token
 * @param token JWT token字符串
 * @returns 解析后的payload，如果解析失败返回null
 */
export const parseJwtToken = (token: string): JwtPayload | null => {
  try {
    console.log('Parsing JWT token:', token ? 'token exists' : 'token is null');
    // JWT token由三部分组成：header.payload.signature
    const parts = token.split('.');
    if (parts.length !== 3) {
      console.error('Invalid JWT token format - expected 3 parts, got:', parts.length);
      return null;
    }

    // 解码payload部分（第二部分）
    const payload = parts[1];
    
    // Base64URL解码
    const decodedPayload = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
    
    // 解析JSON
    const parsedPayload = JSON.parse(decodedPayload);
    
    console.log('Successfully parsed JWT payload:', parsedPayload);
    return parsedPayload as JwtPayload;
  } catch (error) {
    console.error('Failed to parse JWT token:', error);
    return null;
  }
};

/**
 * 检查JWT token是否过期
 * @param token JWT token字符串
 * @returns 是否过期
 */
export const isTokenExpired = (token: string): boolean => {
  const payload = parseJwtToken(token);
  if (!payload) {
    console.log('Token is expired - no payload');
    return true;
  }

  // exp是Unix时间戳（秒），需要转换为毫秒
  const expirationTime = payload.exp * 1000;
  const currentTime = Date.now();
  const isExpired = currentTime >= expirationTime;
  
  console.log('Token expiration check:', {
    currentTime: new Date(currentTime).toISOString(),
    expirationTime: new Date(expirationTime).toISOString(),
    isExpired
  });

  return isExpired;
};

/**
 * 从JWT token中获取用户信息
 * @param token JWT token字符串
 * @returns 用户信息，如果解析失败返回null
 */
export const getUserFromToken = (token: string): { id: string; email: string; role: string } | null => {
  const payload = parseJwtToken(token);
  if (!payload) {
    return null;
  }

  return {
    id: payload.sub,
    email: payload.email,
    role: payload.role
  };
};

/**
 * 检查token是否有效（未过期且格式正确）
 * @param token JWT token字符串
 * @returns 是否有效
 */
export const isValidToken = (token: string): boolean => {
  if (!token) {
    return false;
  }

  return !isTokenExpired(token);
};
