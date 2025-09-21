import { 
  InventoryCheckResult, 
  InventoryOperationResult, 
  ProductInventoryInfo, 
  InventoryUpdate, 
  BatchInventoryUpdateResult,
  InventoryUpdateRequest,
  InventoryLockRequest
} from '../interfaces/InventoryModels';
import authService from './authService';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7037/api';

export class InventoryService {
  private static instance: InventoryService;

  private constructor() {}

  public static getInstance(): InventoryService {
    if (!InventoryService.instance) {
      InventoryService.instance = new InventoryService();
    }
    return InventoryService.instance;
  }

  public async checkStock(productId: string, quantity: number): Promise<InventoryCheckResult> {
    const response = await fetch(`${API_BASE_URL}/inventory/check/${productId}?quantity=${quantity}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Stock check failed');
    }

    return await response.json();
  }

  public async getProductInventory(productId: string): Promise<ProductInventoryInfo> {
    const response = await fetch(`${API_BASE_URL}/inventory/info/${productId}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Failed to get inventory info');
    }

    return await response.json();
  }

  public async deductStock(productId: string, quantity: number, reason: string = '', notes: string = ''): Promise<InventoryOperationResult> {
    const token = authService.getToken();
    if (!token) {
      throw new Error('Authentication required');
    }

    const request: InventoryUpdateRequest = {
      productId,
      quantity,
      reason,
      notes
    };

    const response = await fetch(`${API_BASE_URL}/inventory/deduct`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Stock deduction failed');
    }

    return await response.json();
  }

  public async restoreStock(productId: string, quantity: number, reason: string = '', notes: string = ''): Promise<InventoryOperationResult> {
    const token = authService.getToken();
    if (!token) {
      throw new Error('Authentication required');
    }

    const request: InventoryUpdateRequest = {
      productId,
      quantity,
      reason,
      notes
    };

    const response = await fetch(`${API_BASE_URL}/inventory/restore`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Stock restoration failed');
    }

    return await response.json();
  }

  public async lockStock(productId: string, quantity: number, orderId: string, reason: string = '', notes: string = ''): Promise<InventoryOperationResult> {
    const token = authService.getToken();
    if (!token) {
      throw new Error('Authentication required');
    }

    const request: InventoryLockRequest = {
      productId,
      quantity,
      orderId,
      reason,
      notes
    };

    const response = await fetch(`${API_BASE_URL}/inventory/lock`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Stock locking failed');
    }

    return await response.json();
  }

  public async releaseLockedStock(productId: string, quantity: number, orderId: string, reason: string = '', notes: string = ''): Promise<InventoryOperationResult> {
    const token = authService.getToken();
    if (!token) {
      throw new Error('Authentication required');
    }

    const request: InventoryLockRequest = {
      productId,
      quantity,
      orderId,
      reason,
      notes
    };

    const response = await fetch(`${API_BASE_URL}/inventory/release`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Stock release failed');
    }

    return await response.json();
  }

  public async batchUpdateInventory(updates: InventoryUpdate[]): Promise<BatchInventoryUpdateResult> {
    const token = authService.getToken();
    if (!token) {
      throw new Error('Authentication required');
    }

    const response = await fetch(`${API_BASE_URL}/inventory/batch-update`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(updates),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Batch inventory update failed');
    }

    return await response.json();
  }

  public async getOperationTypes(): Promise<any[]> {
    const response = await fetch(`${API_BASE_URL}/inventory/operation-types`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Failed to get operation types');
    }

    return await response.json();
  }
}

export default InventoryService.getInstance();
