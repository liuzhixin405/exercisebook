import { User, CreateUserDto, UpdateUserDto } from '../interfaces/User';
import authService from './authService';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7037/api';

class UserService {
  private getAuthHeaders(): HeadersInit {
    const token = authService.getToken();
    return {
      'Content-Type': 'application/json',
      ...(token && { 'Authorization': `Bearer ${token}` }),
    };
  }

  public async getAllUsers(): Promise<User[]> {
    const response = await fetch(`${API_BASE_URL}/users`, {
      method: 'GET',
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to fetch users');
    }

    return response.json();
  }

  public async getUserById(id: string): Promise<User> {
    const response = await fetch(`${API_BASE_URL}/users/${id}`, {
      method: 'GET',
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to fetch user');
    }

    return response.json();
  }

  public async getUserByEmail(email: string): Promise<User> {
    const response = await fetch(`${API_BASE_URL}/users/email/${email}`, {
      method: 'GET',
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to fetch user');
    }

    return response.json();
  }

  public async createUser(createUserDto: CreateUserDto): Promise<User> {
    const response = await fetch(`${API_BASE_URL}/users`, {
      method: 'POST',
      headers: this.getAuthHeaders(),
      body: JSON.stringify(createUserDto),
    });

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Failed to create user');
    }

    return response.json();
  }

  public async updateUser(id: string, updateUserDto: UpdateUserDto): Promise<User> {
    const response = await fetch(`${API_BASE_URL}/users/${id}`, {
      method: 'PUT',
      headers: this.getAuthHeaders(),
      body: JSON.stringify(updateUserDto),
    });

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Failed to update user');
    }

    return response.json();
  }

  public async deleteUser(id: string): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/users/${id}`, {
      method: 'DELETE',
      headers: this.getAuthHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to delete user');
    }
  }
}

export default new UserService();
