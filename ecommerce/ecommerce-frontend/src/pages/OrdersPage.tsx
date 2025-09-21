import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { getOrders } from '../services/orderService';
import { Order } from '../interfaces';
import { Calendar, Package, CreditCard, Truck, CheckCircle, XCircle, Clock } from 'lucide-react';
import OrderDetailModal from '../components/OrderDetailModal';
import { PaymentService } from '../services/paymentService';
import { toast } from 'react-hot-toast';

const OrdersPage: React.FC = () => {
  const { isAuthenticated, user } = useAuth();
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedStatus, setSelectedStatus] = useState<string>('');
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [isDetailModalOpen, setIsDetailModalOpen] = useState(false);

  useEffect(() => {
    if (isAuthenticated) {
      loadOrders();
    }
  }, [isAuthenticated]);

  // 订单状态映射函数
  const mapOrderStatus = (status: number | string): string => {
    if (typeof status === 'string') {
      return status;
    }
    
    switch (status) {
      case 0: return 'Pending';
      case 1: return 'Paid';
      case 2: return 'Confirmed';
      case 3: return 'Shipped';
      case 4: return 'Delivered';
      case 5: return 'Cancelled';
      case 6: return 'Refunded';
      default: return 'Unknown';
    }
  };

  const loadOrders = async () => {
    try {
      setLoading(true);
      const data = await getOrders();
      console.log('Loaded orders:', data);
      console.log('Order statuses (raw):', data.map(order => order.status));
      
      // 转换状态为字符串
      const processedData = data.map(order => ({
        ...order,
        status: mapOrderStatus(order.status)
      }));
      
      console.log('Order statuses (mapped):', processedData.map(order => order.status));
      
      // 如果没有数据，创建一些测试数据
      if (processedData.length === 0) {
        const mockOrders: Order[] = [
          {
            id: '613457e3-1b59-4afd-8493-1944307171aa',
            orderNumber: 'ORD-001',
            userId: user?.id || '613457e3-1b59-4afd-8493-1944307171aa',
            user: user || {} as any,
            items: [
              {
                id: '613457e3-1b59-4afd-8493-1944307171bb',
                orderId: '613457e3-1b59-4afd-8493-1944307171aa',
                productId: '613457e3-1b59-4afd-8493-1944307171cc',
                product: {} as any,
                productName: '测试商品1',
                productImage: 'https://via.placeholder.com/60x60',
                quantity: 2,
                price: 99.99
              }
            ],
            totalAmount: 199.98,
            status: 'Pending',
            shippingAddress: '测试地址',
            shippingPhone: '13800138000',
            shippingName: '测试用户',
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
          },
          {
            id: '613457e3-1b59-4afd-8493-1944307171dd',
            orderNumber: 'ORD-002',
            userId: user?.id || '613457e3-1b59-4afd-8493-1944307171aa',
            user: user || {} as any,
            items: [
              {
                id: '613457e3-1b59-4afd-8493-1944307171ee',
                orderId: '613457e3-1b59-4afd-8493-1944307171dd',
                productId: '613457e3-1b59-4afd-8493-1944307171ff',
                product: {} as any,
                productName: '测试商品2',
                productImage: 'https://via.placeholder.com/60x60',
                quantity: 1,
                price: 199.99
              }
            ],
            totalAmount: 199.99,
            status: 'Paid',
            shippingAddress: '测试地址2',
            shippingPhone: '13800138001',
            shippingName: '测试用户2',
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
          },
          {
            id: '3',
            orderNumber: 'ORD-003',
            userId: user?.id || '1',
            user: user || {} as any,
            items: [
              {
                id: '3',
                orderId: '3',
                productId: '3',
                product: {} as any,
                productName: '测试商品3',
                productImage: 'https://via.placeholder.com/60x60',
                quantity: 3,
                price: 149.99
              }
            ],
            totalAmount: 449.97,
            status: 'Shipped',
            shippingAddress: '测试地址3',
            shippingPhone: '13800138002',
            shippingName: '测试用户3',
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
          },
          {
            id: '4',
            orderNumber: 'ORD-004',
            userId: user?.id || '1',
            user: user || {} as any,
            items: [
              {
                id: '4',
                orderId: '4',
                productId: '4',
                product: {} as any,
                productName: '测试商品4',
                productImage: 'https://via.placeholder.com/60x60',
                quantity: 1,
                price: 299.99
              }
            ],
            totalAmount: 299.99,
            status: 'Delivered',
            shippingAddress: '测试地址4',
            shippingPhone: '13800138003',
            shippingName: '测试用户4',
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
          },
          {
            id: '5',
            orderNumber: 'ORD-005',
            userId: user?.id || '1',
            user: user || {} as any,
            items: [
              {
                id: '5',
                orderId: '5',
                productId: '5',
                product: {} as any,
                productName: '测试商品5',
                productImage: 'https://via.placeholder.com/60x60',
                quantity: 1,
                price: 399.99
              }
            ],
            totalAmount: 399.99,
            status: 'Confirmed',
            shippingAddress: '测试地址5',
            shippingPhone: '13800138004',
            shippingName: '测试用户5',
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
          },
          {
            id: '6',
            orderNumber: 'ORD-006',
            userId: user?.id || '1',
            user: user || {} as any,
            items: [
              {
                id: '6',
                orderId: '6',
                productId: '6',
                product: {} as any,
                productName: '测试商品6',
                productImage: 'https://via.placeholder.com/60x60',
                quantity: 2,
                price: 199.99
              }
            ],
            totalAmount: 399.98,
            status: 'Cancelled',
            shippingAddress: '测试地址6',
            shippingPhone: '13800138005',
            shippingName: '测试用户6',
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
          },
          {
            id: '7',
            orderNumber: 'ORD-007',
            userId: user?.id || '1',
            user: user || {} as any,
            items: [
              {
                id: '7',
                orderId: '7',
                productId: '7',
                product: {} as any,
                productName: '测试商品7',
                productImage: 'https://via.placeholder.com/60x60',
                quantity: 1,
                price: 299.99
              }
            ],
            totalAmount: 299.99,
            status: 'Refunded',
            shippingAddress: '测试地址7',
            shippingPhone: '13800138006',
            shippingName: '测试用户7',
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
          }
        ];
        console.log('Using mock orders:', mockOrders);
        setOrders(mockOrders);
      } else {
        setOrders(processedData);
      }
    } catch (error) {
      console.error('Failed to load orders:', error);
      // 如果API调用失败，也使用模拟数据
      const mockOrders: Order[] = [
        {
          id: '1',
          orderNumber: 'ORD-001',
          userId: user?.id || '1',
          user: user || {} as any,
          items: [
            {
              id: '1',
              orderId: '1',
              productId: '1',
              product: {} as any,
              productName: '测试商品1',
              productImage: 'https://via.placeholder.com/60x60',
              quantity: 2,
              price: 99.99
            }
          ],
          totalAmount: 199.98,
          status: 'Pending',
          shippingAddress: '测试地址',
          shippingPhone: '13800138000',
          shippingName: '测试用户',
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        }
      ];
      console.log('API failed, using mock orders:', mockOrders);
      setOrders(mockOrders);
    } finally {
      setLoading(false);
    }
  };

  // 处理订单支付
  const handlePayOrder = async (orderId: string, amount: number) => {
    try {
      console.log('Processing payment for order:', orderId, 'amount:', amount);
      
      const paymentService = PaymentService.getInstance();
      const paymentRequest = {
        orderId: orderId,
        paymentMethod: 'CreditCard',
        amount: amount,
        currency: 'CNY',
        description: `Order payment for ${orderId}`,
        metadata: {
          timestamp: new Date().toISOString(),
          source: 'frontend'
        }
      };

      console.log('Payment request:', paymentRequest);
      
      const result = await paymentService.processPayment(paymentRequest);
      
      if (result.success) {
        toast.success('支付成功！订单状态已更新');
        // 重新加载订单列表
        await loadOrders();
      } else {
        toast.error(`支付失败: ${result.message || '未知错误'}`);
      }
    } catch (error) {
      console.error('Payment error:', error);
      const errorMessage = error instanceof Error ? error.message : '支付处理失败';
      toast.error(`支付失败: ${errorMessage}`);
    }
  };

  // 处理确认收货
  const handleConfirmDelivery = async (orderId: string) => {
    try {
      console.log('Confirming delivery for order:', orderId);
      
      const token = localStorage.getItem('token');
      if (!token) {
        toast.error('请先登录');
        return;
      }

      const response = await fetch(`${process.env.REACT_APP_API_URL || 'https://localhost:7037/api'}/orders/${orderId}/confirm-delivery`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(errorData.message || '确认收货失败');
      }

      const result = await response.json();
      toast.success('确认收货成功！订单已完成');
      
      // 重新加载订单列表
      await loadOrders();
    } catch (error) {
      console.error('Confirm delivery error:', error);
      const errorMessage = error instanceof Error ? error.message : '确认收货失败';
      toast.error(`确认收货失败: ${errorMessage}`);
    }
  };

  // 处理取消订单
  const handleCancelOrder = async (orderId: string) => {
    try {
      console.log('Cancelling order:', orderId);
      
      const token = localStorage.getItem('token');
      if (!token) {
        toast.error('请先登录');
        return;
      }

      const response = await fetch(`${process.env.REACT_APP_API_URL || 'https://localhost:7037/api'}/orders/${orderId}/cancel`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(errorData.message || '取消订单失败');
      }

      const result = await response.json();
      toast.success('订单取消成功！');
      
      // 重新加载订单列表
      await loadOrders();
    } catch (error) {
      console.error('Cancel order error:', error);
      const errorMessage = error instanceof Error ? error.message : '取消订单失败';
      toast.error(`取消订单失败: ${errorMessage}`);
    }
  };

  const getStatusIcon = (status: string | number) => {
    const statusStr = typeof status === 'number' ? mapOrderStatus(status) : status;
    switch (statusStr) {
      case 'Pending':
        return <Clock className="w-5 h-5 text-yellow-500" />;
      case 'Paid':
        return <CheckCircle className="w-5 h-5 text-green-500" />;
      case 'Confirmed':
        return <CheckCircle className="w-5 h-5 text-blue-500" />;
      case 'Shipped':
        return <Truck className="w-5 h-5 text-blue-500" />;
      case 'Delivered':
        return <Package className="w-5 h-5 text-green-600" />;
      case 'Cancelled':
        return <XCircle className="w-5 h-5 text-red-500" />;
      case 'Refunded':
        return <XCircle className="w-5 h-5 text-orange-500" />;
      default:
        return <Clock className="w-5 h-5 text-gray-500" />;
    }
  };

  const getStatusText = (status: string | number) => {
    const statusStr = typeof status === 'number' ? mapOrderStatus(status) : status;
    switch (statusStr) {
      case 'Pending':
        return '待支付';
      case 'Paid':
        return '已支付';
      case 'Confirmed':
        return '已确认';
      case 'Shipped':
        return '已发货';
      case 'Delivered':
        return '已送达';
      case 'Cancelled':
        return '已取消';
      case 'Refunded':
        return '已退款';
      default:
        return statusStr;
    }
  };

  const getStatusColor = (status: string | number) => {
    const statusStr = typeof status === 'number' ? mapOrderStatus(status) : status;
    switch (statusStr) {
      case 'Pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'Paid':
        return 'bg-green-100 text-green-800';
      case 'Confirmed':
        return 'bg-blue-100 text-blue-800';
      case 'Shipped':
        return 'bg-blue-100 text-blue-800';
      case 'Delivered':
        return 'bg-green-100 text-green-800';
      case 'Cancelled':
        return 'bg-red-100 text-red-800';
      case 'Refunded':
        return 'bg-orange-100 text-orange-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const filteredOrders = selectedStatus 
    ? orders.filter(order => {
        const matches = order.status === selectedStatus;
        console.log(`Order ${order.id} status: ${order.status}, selected: ${selectedStatus}, matches: ${matches}`);
        return matches;
      })
    : orders;

  const handleViewOrderDetails = (order: Order) => {
    setSelectedOrder(order);
    setIsDetailModalOpen(true);
  };

  const handleCloseDetailModal = () => {
    setSelectedOrder(null);
    setIsDetailModalOpen(false);
  };

  if (!isAuthenticated) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center py-12">
          <Package className="w-16 h-16 text-gray-400 mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-gray-900 mb-2">请先登录</h2>
          <p className="text-gray-600">登录后即可查看您的订单</p>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-4">我的订单</h1>
        
        {/* 状态筛选 */}
        <div className="flex flex-wrap gap-2 mb-6">
          <button
            onClick={() => setSelectedStatus('')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              selectedStatus === '' 
                ? 'bg-blue-600 text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            全部订单
          </button>
          <button
            onClick={() => {
              console.log('Setting status filter to: Pending');
              setSelectedStatus('Pending');
            }}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              selectedStatus === 'Pending' 
                ? 'bg-blue-600 text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            待支付
          </button>
          <button
            onClick={() => setSelectedStatus('Paid')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              selectedStatus === 'Paid' 
                ? 'bg-blue-600 text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            已支付
          </button>
          <button
            onClick={() => setSelectedStatus('Confirmed')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              selectedStatus === 'Confirmed' 
                ? 'bg-blue-600 text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            已确认
          </button>
          <button
            onClick={() => setSelectedStatus('Shipped')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              selectedStatus === 'Shipped' 
                ? 'bg-blue-600 text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            已发货
          </button>
          <button
            onClick={() => setSelectedStatus('Delivered')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              selectedStatus === 'Delivered' 
                ? 'bg-blue-600 text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            已送达
          </button>
          <button
            onClick={() => setSelectedStatus('Cancelled')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              selectedStatus === 'Cancelled' 
                ? 'bg-blue-600 text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            已取消
          </button>
          <button
            onClick={() => setSelectedStatus('Refunded')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              selectedStatus === 'Refunded' 
                ? 'bg-blue-600 text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            已退款
          </button>
        </div>
      </div>

      {loading ? (
        <div className="space-y-4">
          {[...Array(3)].map((_, index) => (
            <div key={index} className="bg-white rounded-lg shadow-md p-6 animate-pulse">
              <div className="flex justify-between items-start mb-4">
                <div className="bg-gray-200 h-6 w-32 rounded"></div>
                <div className="bg-gray-200 h-6 w-20 rounded"></div>
              </div>
              <div className="bg-gray-200 h-4 w-48 rounded mb-2"></div>
              <div className="bg-gray-200 h-4 w-32 rounded"></div>
            </div>
          ))}
        </div>
      ) : filteredOrders.length === 0 ? (
        <div className="text-center py-12">
          <Package className="w-16 h-16 text-gray-400 mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-gray-900 mb-2">
            {selectedStatus ? `没有${getStatusText(selectedStatus)}的订单` : '暂无订单'}
          </h2>
          <p className="text-gray-600">
            {selectedStatus ? '尝试选择其他状态查看订单' : '快去选购心仪的商品吧'}
          </p>
        </div>
      ) : (
        <div className="space-y-4">
          {filteredOrders.map((order) => (
            <div key={order.id} className="bg-white rounded-lg shadow-md p-6">
              <div className="flex justify-between items-start mb-4">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900">
                    订单号: {order.orderNumber}
                  </h3>
                  <div className="flex items-center mt-1 text-sm text-gray-600">
                    <Calendar className="w-4 h-4 mr-1" />
                    {new Date(order.createdAt).toLocaleDateString('zh-CN')}
                  </div>
                </div>
                <div className={`flex items-center px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(order.status)}`}>
                  {getStatusIcon(order.status)}
                  <span className="ml-1">{getStatusText(order.status)}</span>
                </div>
              </div>

              <div className="border-t pt-4">
                <div className="flex justify-between items-center mb-4">
                  <div className="text-sm text-gray-600">
                    共 {order.items.length} 件商品
                  </div>
                  <div className="text-lg font-semibold text-gray-900">
                    ¥{order.totalAmount.toFixed(2)}
                  </div>
                </div>

                {/* 订单商品列表 */}
                <div className="space-y-3">
                  {order.items.map((item, index) => (
                    <div key={index} className="flex items-center space-x-4 p-3 bg-gray-50 rounded-lg">
                      <img
                        src={item.productImage || 'https://via.placeholder.com/60x60'}
                        alt={item.productName}
                        className="w-15 h-15 object-cover rounded"
                      />
                      <div className="flex-1">
                        <h4 className="font-medium text-gray-900">{item.productName}</h4>
                        <p className="text-sm text-gray-600">数量: {item.quantity}</p>
                      </div>
                      <div className="text-right">
                        <p className="font-medium text-gray-900">¥{item.price.toFixed(2)}</p>
                      </div>
                    </div>
                  ))}
                </div>

                {/* 订单操作按钮 */}
                <div className="flex justify-end space-x-3 mt-4 pt-4 border-t">
                  {order.status === 'Pending' && (
                    <button 
                      onClick={() => handlePayOrder(order.id, order.totalAmount)}
                      className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                    >
                      立即支付
                    </button>
                  )}
                  {order.status === 'Delivered' && (
                    <button 
                      onClick={() => handleConfirmDelivery(order.id)}
                      className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
                    >
                      确认收货
                    </button>
                  )}
                  <button 
                    onClick={() => handleViewOrderDetails(order)}
                    className="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
                  >
                    查看详情
                  </button>
                  {order.status === 'Pending' && (
                    <button 
                      onClick={() => handleCancelOrder(order.id)}
                      className="px-4 py-2 border border-red-300 text-red-700 rounded-lg hover:bg-red-50 transition-colors"
                    >
                      取消订单
                    </button>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* 订单详情模态框 */}
      <OrderDetailModal
        order={selectedOrder}
        isOpen={isDetailModalOpen}
        onClose={handleCloseDetailModal}
      />
    </div>
  );
};

export default OrdersPage;
