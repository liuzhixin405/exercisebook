import React, { useState, useRef } from 'react';
import { Upload, X, Image as ImageIcon, Trash2 } from 'lucide-react';
import { fileUploadService, ImageInfo } from '../services/fileUploadService';

interface ImageUploadProps {
  value?: string;
  onChange: (imageUrl: string) => void;
  onError?: (error: string) => void;
  className?: string;
}

const ImageUpload: React.FC<ImageUploadProps> = ({ 
  value, 
  onChange, 
  onError,
  className = '' 
}) => {
  const [isUploading, setIsUploading] = useState(false);
  const [showImageGallery, setShowImageGallery] = useState(false);
  const [uploadedImages, setUploadedImages] = useState<ImageInfo[]>([]);
  const [dragOver, setDragOver] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileSelect = async (file: File) => {
    // 验证文件类型
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      onError?.('只支持 JPG、PNG、GIF、WebP 格式的图片');
      return;
    }

    // 验证文件大小 (5MB)
    const maxSize = 5 * 1024 * 1024;
    if (file.size > maxSize) {
      onError?.('图片大小不能超过 5MB');
      return;
    }

    setIsUploading(true);
    try {
      const response = await fileUploadService.uploadProductImage(file);
      if (response.success) {
        onChange(response.fileUrl);
        onError?.(undefined as any); // 清除错误
      } else {
        onError?.('上传失败，请重试');
      }
    } catch (error: any) {
      onError?.(error.message || '上传失败，请重试');
    } finally {
      setIsUploading(false);
    }
  };

  const handleFileInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      handleFileSelect(file);
    }
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setDragOver(false);
    
    const file = e.dataTransfer.files[0];
    if (file) {
      handleFileSelect(file);
    }
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setDragOver(true);
  };

  const handleDragLeave = (e: React.DragEvent) => {
    e.preventDefault();
    setDragOver(false);
  };

  const loadImageGallery = async () => {
    try {
      const response = await fileUploadService.getProductImages();
      setUploadedImages(response.images);
      setShowImageGallery(true);
    } catch (error) {
      onError?.('加载图片库失败');
    }
  };

  const selectImageFromGallery = (imageUrl: string) => {
    onChange(imageUrl);
    setShowImageGallery(false);
  };

  const removeImage = () => {
    onChange('');
  };

  return (
    <div className={`space-y-4 ${className}`}>
      {/* 当前图片显示 */}
      {value && (
        <div className="relative inline-block">
          <img
            src={value}
            alt="产品图片"
            className="w-32 h-32 object-cover rounded-lg border border-gray-300"
            onError={(e) => {
              (e.target as HTMLImageElement).style.display = 'none';
            }}
          />
          <button
            type="button"
            onClick={removeImage}
            className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full p-1 hover:bg-red-600"
          >
            <X className="w-4 h-4" />
          </button>
        </div>
      )}

      {/* 上传区域 */}
      <div
        className={`border-2 border-dashed rounded-lg p-6 text-center transition-colors ${
          dragOver
            ? 'border-blue-500 bg-blue-50'
            : 'border-gray-300 hover:border-gray-400'
        }`}
        onDrop={handleDrop}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
      >
        <input
          ref={fileInputRef}
          type="file"
          accept="image/*"
          onChange={handleFileInputChange}
          className="hidden"
        />

        {isUploading ? (
          <div className="space-y-2">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
            <p className="text-sm text-gray-600">上传中...</p>
          </div>
        ) : (
          <div className="space-y-2">
            <Upload className="w-8 h-8 text-gray-400 mx-auto" />
            <div>
              <p className="text-sm text-gray-600">
                拖拽图片到此处，或
                <button
                  type="button"
                  onClick={() => fileInputRef.current?.click()}
                  className="text-blue-600 hover:text-blue-700 ml-1"
                >
                  点击选择文件
                </button>
              </p>
              <p className="text-xs text-gray-500 mt-1">
                支持 JPG、PNG、GIF、WebP 格式，最大 5MB
              </p>
            </div>
          </div>
        )}
      </div>

      {/* 图片库按钮 */}
      <div className="flex justify-center">
        <button
          type="button"
          onClick={loadImageGallery}
          className="flex items-center space-x-2 px-4 py-2 text-sm text-gray-600 bg-gray-100 rounded-md hover:bg-gray-200"
        >
          <ImageIcon className="w-4 h-4" />
          <span>从图片库选择</span>
        </button>
      </div>

      {/* 图片库模态框 */}
      {showImageGallery && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-4xl max-h-[80vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h3 className="text-lg font-semibold">选择图片</h3>
              <button
                type="button"
                onClick={() => setShowImageGallery(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                <X className="w-6 h-6" />
              </button>
            </div>

            {uploadedImages.length === 0 ? (
              <p className="text-gray-500 text-center py-8">暂无已上传的图片</p>
            ) : (
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                {uploadedImages.map((image) => (
                  <div
                    key={image.fileName}
                    className="relative group cursor-pointer"
                    onClick={() => selectImageFromGallery(image.fileUrl)}
                  >
                    <img
                      src={image.fileUrl}
                      alt={image.fileName}
                      className="w-full h-24 object-cover rounded-lg border border-gray-300"
                    />
                    <div className="absolute inset-0 bg-black bg-opacity-0 group-hover:bg-opacity-20 rounded-lg transition-all duration-200 flex items-center justify-center">
                      <div className="opacity-0 group-hover:opacity-100 bg-white rounded-full p-2">
                        <ImageIcon className="w-4 h-4 text-blue-600" />
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default ImageUpload;
