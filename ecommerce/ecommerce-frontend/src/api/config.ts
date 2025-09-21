// API配置
export const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7037/api';

export const apiConfig = {
  baseURL: API_BASE_URL,
};

// 产品相关API
export const productAPI = {
  // 获取产品列表
  getProducts: `${API_BASE_URL}/products`,
  // 获取单个产品
  getProduct: (id: string) => `${API_BASE_URL}/products/${id}`,
  // 创建产品
  createProduct: `${API_BASE_URL}/products`,
};

// 订单相关API
export const orderAPI = {
  // 获取订单列表
  getOrders: `${API_BASE_URL}/orders`,
  // 获取单个订单
  getOrder: (id: string) => `${API_BASE_URL}/orders/${id}`,
  // 创建订单
  createOrder: `${API_BASE_URL}/orders`,
};

// 支付相关API
export const paymentAPI = {
  // 处理支付
  processPayment: `${API_BASE_URL}/payments/process`,
  // 验证支付
  validatePayment: (id: string) => `${API_BASE_URL}/payments/validate/${id}`,
  // 处理退款
  processRefund: `${API_BASE_URL}/payments/refund`,
  // 获取支付状态
  getPaymentStatus: (orderId: string) => `${API_BASE_URL}/payments/status/${orderId}`,
  // 获取支付方式
  getPaymentMethods: `${API_BASE_URL}/payments/methods`,
  // 获取支付历史
  getPaymentHistory: (orderId: string) => `${API_BASE_URL}/payments/history/${orderId}`,
};

// 库存相关API
export const inventoryAPI = {
  // 检查库存
  checkStock: `${API_BASE_URL}/inventory/check`,
  // 获取产品库存信息
  getProductInventory: (id: string) => `${API_BASE_URL}/inventory/info/${id}`,
  // 扣除库存
  deductStock: `${API_BASE_URL}/inventory/deduct`,
  // 恢复库存
  restoreStock: `${API_BASE_URL}/inventory/restore`,
  // 锁定库存
  lockStock: `${API_BASE_URL}/inventory/lock`,
  // 释放锁定库存
  releaseLockedStock: `${API_BASE_URL}/inventory/release`,
  // 批量更新库存
  batchUpdateInventory: `${API_BASE_URL}/inventory/batch-update`,
  // 获取操作类型
  getOperationTypes: `${API_BASE_URL}/inventory/operation-types`,
  // 获取低库存产品
  getLowStockProducts: `${API_BASE_URL}/inventory/low-stock`,
};