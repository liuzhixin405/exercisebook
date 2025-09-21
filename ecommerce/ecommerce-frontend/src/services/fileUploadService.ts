import apiClient from '../api/client';
import { apiConfig } from '../api/config';

export interface UploadResponse {
  success: boolean;
  fileName: string;
  fileUrl: string;
  fileSize: number;
  message: string;
}

export interface ImageInfo {
  fileName: string;
  fileUrl: string;
  fileSize: number;
  uploadDate: string;
}

export interface ImageListResponse {
  images: ImageInfo[];
}

// 将相对路径转换为完整的URL
const getFullImageUrl = (relativeUrl: string): string => {
  if (relativeUrl.startsWith('http')) {
    return relativeUrl; // 已经是完整URL
  }
  
  // 从API配置中获取基础URL，去掉/api后缀
  const baseUrl = apiConfig.baseURL.replace('/api', '');
  return `${baseUrl}${relativeUrl}`;
};

export const fileUploadService = {
  // 上传产品图片
  uploadProductImage: async (file: File): Promise<UploadResponse> => {
    const formData = new FormData();
    formData.append('file', file);

    const response = await apiClient.post<UploadResponse>('/fileupload/product-image', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });

    // 将返回的fileUrl转换为完整URL
    const result = response.data;
    result.fileUrl = getFullImageUrl(result.fileUrl);
    return result;
  },

  // 删除产品图片
  deleteProductImage: async (fileName: string): Promise<{ success: boolean; message: string }> => {
    const response = await apiClient.delete(`/fileupload/product-image/${fileName}`);
    return response.data;
  },

  // 获取所有已上传的图片列表
  getProductImages: async (): Promise<ImageListResponse> => {
    const response = await apiClient.get<ImageListResponse>('/fileupload/product-images');
    
    // 将每个图片的fileUrl转换为完整URL
    const result = response.data;
    result.images = result.images.map(image => ({
      ...image,
      fileUrl: getFullImageUrl(image.fileUrl)
    }));
    
    return result;
  }
};
