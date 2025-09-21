import { 
  PaymentRequest, 
  PaymentResult, 
  PaymentValidationResult, 
  RefundRequest, 
  RefundResult, 
  PaymentStatus
} from '../interfaces/PaymentModels';
import authService from './authService';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7037/api';

export class PaymentService {
  private static instance: PaymentService;

  private constructor() {}

  public static getInstance(): PaymentService {
    if (!PaymentService.instance) {
      PaymentService.instance = new PaymentService();
    }
    return PaymentService.instance;
  }

  public async processPayment(paymentRequest: PaymentRequest): Promise<PaymentResult> {
    const token = authService.getToken();
    if (!token) {
      throw new Error('Authentication required');
    }

    // 改回 PaymentController 统一入口
    const response = await fetch(`${API_BASE_URL}/payment/process`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(paymentRequest),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Payment processing failed');
    }

    return await response.json();
  }

  public async validatePayment(paymentId: string): Promise<PaymentValidationResult> {
    const token = authService.getToken();
    if (!token) {
      throw new Error('Authentication required');
    }

    const response = await fetch(`${API_BASE_URL}/payment/validate/${paymentId}`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Payment validation failed');
    }

    return await response.json();
  }

  public async processRefund(refundRequest: RefundRequest): Promise<RefundResult> {
    const token = authService.getToken();
    if (!token) {
      throw new Error('Authentication required');
    }

    const response = await fetch(`${API_BASE_URL}/payment/refund`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(refundRequest),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Refund processing failed');
    }

    return await response.json();
  }

  public async getPaymentStatus(orderId: string): Promise<PaymentStatus> {
    const token = authService.getToken();
    if (!token) {
      throw new Error('Authentication required');
    }

    const response = await fetch(`${API_BASE_URL}/payment/status/${orderId}`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Failed to get payment status');
    }

    return await response.json();
  }

  public async getPaymentMethods(): Promise<any[]> {
    const response = await fetch(`${API_BASE_URL}/payment/methods`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || 'Failed to get payment methods');
    }

    return await response.json();
  }
}

export default PaymentService.getInstance();
