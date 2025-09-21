export enum InventoryOperationType {
  Add = 'Add',
  Deduct = 'Deduct',
  Reserve = 'Reserve',
  Release = 'Release',
  Lock = 'Lock',
  Unlock = 'Unlock',
  Adjust = 'Adjust',
  Set = 'Set'
}

export interface InventoryCheckResult {
  isAvailable: boolean;
  productId: string;
  requestedQuantity: number;
  availableStock: number;
  reservedStock: number;
  message: string;
}

export interface InventoryOperationResult {
  success: boolean;
  productId: string;
  quantity: number;
  oldStock: number;
  newStock: number;
  message: string;
  operationTime: Date;
}

export interface ProductInventoryInfo {
  productId: string;
  productName: string;
  totalStock: number;
  availableStock: number;
  reservedStock: number;
  lockedStock: number;
  lastUpdated: Date;
  isLowStock: boolean;
  lowStockThreshold: number;
}

export interface InventoryUpdate {
  productId: string;
  quantity: number;
  operationType: InventoryOperationType;
  reason: string;
  orderId?: string;
  notes: string;
}

export interface BatchInventoryUpdateResult {
  overallSuccess: boolean;
  totalOperations: number;
  successfulOperations: number;
  failedOperations: number;
  results: InventoryOperationResult[];
  message: string;
}

export interface InventoryTransaction {
  id: string;
  productId: string;
  operationType: InventoryOperationType;
  quantity: number;
  beforeStock: number;
  afterStock: number;
  orderId?: string;
  reason: string;
  notes: string;
  createdAt: Date;
  createdBy: string;
}

export interface InventoryUpdateRequest {
  productId: string;
  quantity: number;
  reason: string;
  notes: string;
}

export interface InventoryLockRequest {
  productId: string;
  quantity: number;
  orderId: string;
  reason: string;
  notes: string;
}
