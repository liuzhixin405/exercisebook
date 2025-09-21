import apiClient from '../api/client';
import { Product, CreateProductDto } from '../interfaces';

export const getProducts = async (): Promise<Product[]> => {
  const response = await apiClient.get<Product[]>('/products');
  return response.data;
};

export const getProduct = async (id: string): Promise<Product> => {
  const response = await apiClient.get<Product>(`/products/${id}`);
  return response.data;
};

export const getProductsByCategory = async (category: string): Promise<Product[]> => {
  const response = await apiClient.get<Product[]>(`/products/category/${category}`);
  return response.data;
};

export const searchProducts = async (query: string): Promise<Product[]> => {
  const response = await apiClient.get<Product[]>(`/products/search?q=${encodeURIComponent(query)}`);
  return response.data;
};

export const createProduct = async (product: CreateProductDto): Promise<Product> => {
  const response = await apiClient.post<Product>('/products', product);
  return response.data;
};

export const updateProduct = async (id: string, product: Partial<CreateProductDto>): Promise<Product> => {
  const response = await apiClient.put<Product>(`/products/${id}`, product);
  return response.data;
};

export const deleteProduct = async (id: string): Promise<boolean> => {
  const response = await apiClient.delete(`/products/${id}`);
  return response.status === 200;
};