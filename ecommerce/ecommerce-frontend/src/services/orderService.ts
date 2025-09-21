import apiClient from '../api/client';
import { Order, CreateOrderDto } from '../interfaces';
import AddressService from './addressService';

// 本地定义验证结果类型，避免接口聚合未导出导致的构建错误
interface AddressValidationResult {
  isValid: boolean;
  message: string;
  requiresAddressManagement: boolean;
  addressId?: string;
  // 仅用于类型提示，不强依赖 Address 结构
  address?: unknown;
}

export const getOrders = async (): Promise<Order[]> => {
  try {
    const response = await apiClient.get<Order[]>('/orders');
    return response.data;
  } catch (error) {
    console.error('Failed to fetch orders:', error);
    throw error;
  }
};

export const getOrderById = async (id: string): Promise<Order> => {
  try {
    const response = await apiClient.get<Order>(`/orders/${id}`);
    return response.data;
  } catch (error) {
    console.error('Failed to fetch order:', error);
    throw error;
  }
};

export const validateAddress = async (orderData: CreateOrderDto): Promise<AddressValidationResult> => {
  try {
    const addressService = AddressService.getInstance();

    // 如果传了 addressId，走 /api/address/validate
    if (orderData.addressId) {
      const ok = await addressService.validateAddress(orderData.addressId);
      return {
        isValid: !!ok,
        message: ok ? '地址验证通过' : '指定的地址不存在或无权限访问，请先设置默认地址',
        requiresAddressManagement: !ok
      };
    }

    // 未传 addressId：尝试获取默认地址
    const defaultAddress = await addressService.getDefaultAddress();
    if (!defaultAddress) {
      return {
        isValid: false,
        message: '请先设置默认收货地址',
        requiresAddressManagement: true
      };
    }

    return {
      isValid: true,
      message: '地址验证通过',
      requiresAddressManagement: false,
      addressId: defaultAddress.id,
      address: defaultAddress
    };
  } catch (error: any) {
    console.error('Failed to validate address:', error);
    const message = error?.message || '地址验证失败，请重试';
    return {
      isValid: false,
      message,
      requiresAddressManagement: true
    };
  }
};

export const createOrder = async (orderData: CreateOrderDto): Promise<Order> => {
  try {
    // 统一在服务层先进行地址验证，确保所有调用方都经过校验
    const validation = await validateAddress(orderData);
    if (!validation.isValid) {
      const message = validation.message || '请先设置默认收货地址';
      throw new Error(message);
    }

    const response = await apiClient.post<Order>('/orders', orderData);
    return response.data;
  } catch (error: any) {
    console.error('Failed to create order:', error);

    // 处理HTTP错误响应
    if (error.response?.data?.message) {
      throw new Error(error.response.data.message);
    }

    // 处理网络错误或其他错误
    if (error.message) {
      throw new Error(error.message);
    }

    throw new Error('创建订单失败，请重试');
  }
};

export const updateOrderStatus = async (id: string, status: string): Promise<Order> => {
  try {
    const response = await apiClient.put<Order>(`/orders/${id}/status`, { status });
    return response.data;
  } catch (error) {
    console.error('Failed to update order status:', error);
    throw error;
  }
};

export const cancelOrder = async (id: string): Promise<Order> => {
  try {
    const response = await apiClient.put<Order>(`/orders/${id}/cancel`);
    return response.data;
  } catch (error) {
    console.error('Failed to cancel order:', error);
    throw error;
  }
};

export const completeOrder = async (id: string): Promise<Order> => {
  try {
    const response = await apiClient.put<Order>(`/orders/${id}/complete`);
    return response.data;
  } catch (error) {
    console.error('Failed to complete order:', error);
    throw error;
  }
};