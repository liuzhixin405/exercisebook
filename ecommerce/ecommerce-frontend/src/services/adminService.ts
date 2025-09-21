import apiClient from '../api/client';
import { Product, CreateProductDto, UpdateProductDto, BatchUpdateProductStatusDto } from '../interfaces';

export const adminService = {
  // 获取所有产品（管理员视图）
  getAllProducts: async (): Promise<Product[]> => {
    const response = await apiClient.get<Product[]>('/admin/products');
    return response.data;
  },

  // 创建新产品
  createProduct: async (product: CreateProductDto): Promise<Product> => {
    const response = await apiClient.post<Product>('/admin/products', product);
    return response.data;
  },

  // 获取单个产品详情
  getProduct: async (id: string): Promise<Product> => {
    const response = await apiClient.get<Product>(`/admin/products/${id}`);
    return response.data;
  },

  // 更新产品信息
  updateProduct: async (id: string, product: UpdateProductDto): Promise<Product> => {
    const response = await apiClient.put<Product>(`/admin/products/${id}`, product);
    return response.data;
  },

  // 删除产品
  deleteProduct: async (id: string): Promise<boolean> => {
    const response = await apiClient.delete(`/admin/products/${id}`);
    return response.status === 204;
  },

  // 批量更新产品状态
  batchUpdateProductStatus: async (batchUpdate: BatchUpdateProductStatusDto): Promise<{
    successCount: number;
    errorCount: number;
    errors: string[];
  }> => {
    const response = await apiClient.put('/admin/products/batch-status', batchUpdate);
    return response.data;
  }
};
