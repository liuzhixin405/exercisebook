export const formatPrice = (price: number): string => {
  return new Intl.NumberFormat('zh-CN', {
    style: 'currency',
    currency: 'CNY',
  }).format(price);
};

export const formatDate = (date: string): string => {
  return new Date(date).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
};

export const formatOrderStatus = (status: string): string => {
  const statusMap: Record<string, string> = {
    'Pending': '待处理',
    'Confirmed': '已确认',
    'Shipped': '已发货',
    'Delivered': '已送达',
    'Cancelled': '已取消',
  };
  return statusMap[status] || status;
};
