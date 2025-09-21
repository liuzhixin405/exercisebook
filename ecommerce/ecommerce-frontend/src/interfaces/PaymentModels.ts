export enum PaymentState {
  Pending = 'Pending',
  Processing = 'Processing',
  Completed = 'Completed',
  Failed = 'Failed',
  Cancelled = 'Cancelled',
  Refunded = 'Refunded'
}

export enum RefundStatus {
  Pending = 'Pending',
  Processing = 'Processing',
  Completed = 'Completed',
  Failed = 'Failed',
  Cancelled = 'Cancelled'
}

export enum PaymentMethod {
  CreditCard = 'CreditCard',
  DebitCard = 'DebitCard',
  PayPal = 'PayPal',
  Alipay = 'Alipay',
  WeChatPay = 'WeChatPay',
  BankTransfer = 'BankTransfer',
  Cash = 'Cash',
  Other = 'Other'
}

export interface PaymentRequest {
  orderId: string;
  paymentMethod: string;
  amount: number;
  currency: string;
  description: string;
  metadata: Record<string, string>;
}

export interface PaymentResult {
  success: boolean;
  paymentId: string;
  transactionId: string;
  status: PaymentStatus;
  message: string;
  processedAt: Date;
  additionalData: Record<string, string>;
}

export interface PaymentStatus {
  paymentId: string;
  orderId: string;
  state: PaymentState;
  amount: number;
  currency: string;
  paymentMethod: string;
  createdAt: Date;
  processedAt?: Date;
  message: string;
}

export interface PaymentValidationResult {
  isValid: boolean;
  paymentId: string;
  status: PaymentStatus;
  message: string;
  validatedAt: Date;
}

export interface RefundRequest {
  orderId: string;
  paymentId: string;
  amount: number;
  reason: string;
  description: string;
}

export interface RefundResult {
  success: boolean;
  refundId: string;
  paymentId: string;
  amount: number;
  status: RefundStatus;
  message: string;
  processedAt: Date;
}
