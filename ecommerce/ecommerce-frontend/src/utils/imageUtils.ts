import { apiConfig } from '../api/config';

/**
 * 将相对路径转换为完整的图片URL
 * @param imageUrl 图片URL（可能是相对路径或完整URL）
 * @returns 完整的图片URL
 */
export const getFullImageUrl = (imageUrl: string): string => {
  if (!imageUrl) {
    return '';
  }
  
  if (imageUrl.startsWith('http')) {
    return imageUrl; // 已经是完整URL
  }
  
  // 从API配置中获取基础URL，去掉/api后缀
  const baseUrl = apiConfig.baseURL.replace('/api', '');
  return `${baseUrl}${imageUrl}`;
};

/**
 * 检查图片URL是否有效
 * @param imageUrl 图片URL
 * @returns 是否为有效的图片URL
 */
export const isValidImageUrl = (imageUrl: string): boolean => {
  if (!imageUrl) {
    return false;
  }
  
  // 检查是否为有效的URL格式
  try {
    const url = new URL(imageUrl.startsWith('http') ? imageUrl : getFullImageUrl(imageUrl));
    return url.protocol === 'http:' || url.protocol === 'https:';
  } catch {
    return false;
  }
};
