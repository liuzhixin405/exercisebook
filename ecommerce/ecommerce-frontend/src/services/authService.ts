import { LoginDto, LoginResponseDto, RefreshTokenDto, CreateUserDto } from '../interfaces/User';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7037/api';

export class AuthService {
  private static instance: AuthService;
  private token: string | null = null;
  private refreshToken: string | null = null;

  private constructor() {
    // Load tokens from localStorage on initialization
    this.token = localStorage.getItem('token');
    this.refreshToken = localStorage.getItem('refreshToken');
  }

  public static getInstance(): AuthService {
    if (!AuthService.instance) {
      AuthService.instance = new AuthService();
    }
    return AuthService.instance;
  }

  public async register(createUserDto: CreateUserDto): Promise<LoginResponseDto> {
    const response = await fetch(`${API_BASE_URL}/auth/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(createUserDto),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Registration failed');
    }

    const data: LoginResponseDto = await response.json();
    this.setTokens(data.token, data.refreshToken);
    return data;
  }

  public async login(loginDto: LoginDto): Promise<LoginResponseDto> {
    console.log('AuthService: Attempting login to:', `${API_BASE_URL}/auth/login`);
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(loginDto),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      console.error('AuthService: Login failed with status:', response.status, errorData);
      throw new Error(errorData.message || 'Login failed');
    }

    const data: LoginResponseDto = await response.json();
    console.log('AuthService: Login successful, setting tokens');
    this.setTokens(data.token, data.refreshToken);
    return data;
  }

  public async refreshTokenAsync(refreshTokenDto: RefreshTokenDto): Promise<LoginResponseDto> {
    const response = await fetch(`${API_BASE_URL}/auth/refresh`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(refreshTokenDto),
    });

    if (!response.ok) {
      throw new Error('Token refresh failed');
    }

    const data: LoginResponseDto = await response.json();
    this.setTokens(data.token, data.refreshToken);
    return data;
  }

  public async logout(): Promise<void> {
    if (this.refreshToken) {
      try {
        await fetch(`${API_BASE_URL}/auth/logout`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${this.token}`,
          },
          body: JSON.stringify({ refreshToken: this.refreshToken }),
        });
      } catch (error) {
        console.error('Logout error:', error);
      }
    }

    this.clearTokens();
  }

  public getToken(): string | null {
    return this.token;
  }

  public getRefreshToken(): string | null {
    return this.refreshToken;
  }

  public isAuthenticated(): boolean {
    return !!this.token;
  }

  private setTokens(token: string, refreshToken: string): void {
    console.log('AuthService: Setting tokens');
    this.token = token;
    this.refreshToken = refreshToken;
    localStorage.setItem('token', token);
    localStorage.setItem('refreshToken', refreshToken);
    console.log('AuthService: Tokens saved to localStorage');
  }

  private clearTokens(): void {
    console.log('AuthService: Clearing tokens');
    this.token = null;
    this.refreshToken = null;
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    console.log('AuthService: Tokens cleared from localStorage');
  }
}

export default AuthService.getInstance();
