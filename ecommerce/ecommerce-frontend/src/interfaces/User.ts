export interface User {
  id: string;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  address: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  lastLoginAt?: string;
  role: string;
}

export interface CreateUserDto {
  userName: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  address: string;
  role: string;
}

export interface UpdateUserDto {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  address: string;
  isActive: boolean;
  role: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface LoginResponseDto {
  token: string;
  refreshToken: string;
  user: User;
  expiresAt: string;
}

export interface RefreshTokenDto {
  refreshToken: string;
}

export interface AuthContextType {
  user: User | null;
  login: (email: string, password: string) => Promise<boolean>;
  register: (userData: CreateUserDto) => Promise<boolean>;
  logout: () => void;
  isAuthenticated: boolean;
  loading: boolean;
}

// Order related interfaces
export interface OrderItem {
  id: string;
  productId: string;
  productName: string;
  productImage: string;
  quantity: number;
  price: number;
  subtotal: number;
}

export interface Order {
  id: string;
  userId: string;
  customerName: string;
  phoneNumber: string;
  shippingAddress: string;
  totalAmount: number;
  status: OrderStatus;
  createdAt: string;
  updatedAt: string;
  paidAt?: string;
  shippedAt?: string;
  deliveredAt?: string;
  cancelledAt?: string;
  paymentMethod: string;
  trackingNumber: string;
  notes: string;
  items: OrderItem[];
}

export enum OrderStatus {
  Pending = 'Pending',
  Paid = 'Paid',
  Confirmed = 'Confirmed',
  Shipped = 'Shipped',
  Delivered = 'Delivered',
  Cancelled = 'Cancelled',
  Refunded = 'Refunded'
}

export interface CreateOrderDto {
  customerName: string;
  phoneNumber: string;
  shippingAddress: string;
  paymentMethod: string;
  notes: string;
  items: CreateOrderItemDto[];
}

export interface CreateOrderItemDto {
  productId: string;
  quantity: number;
}

export interface PaymentDto {
  orderId: string;
  paymentMethod: string;
  amount: number;
}

export interface UpdateOrderStatusDto {
  status: OrderStatus;
  trackingNumber: string;
  notes: string;
}
