import React, { useState, useEffect } from 'react';
import { Order, OrderItem } from '../interfaces';
import { toast } from 'react-hot-toast';
import { 
  Eye, 
  CheckCircle, 
  Truck, 
  Package, 
  XCircle, 
  Search, 
  Filter,
  MoreVertical,
  RefreshCw
} from 'lucide-react';

interface OrderManagementProps {
  className?: string;
}

const OrderManagement: React.FC<OrderManagementProps> = ({ className = '' }) => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedOrders, setSelectedOrders] = useState<string[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('');
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [showOrderDetail, setShowOrderDetail] = useState(false);
  const [showShipmentModal, setShowShipmentModal] = useState(false);
  const [trackingNumber, setTrackingNumber] = useState('');

  // 订单状态选项
  const statusOptions = [
    { value: '', label: '全部状态' },
    { value: '0', label: '待支付' },
    { value: '1', label: '已支付' },
    { value: '2', label: '已确认' },
    { value: '3', label: '已发货' },
    { value: '4', label: '已送达' },
    { value: '5', label: '已完成' },
    { value: '6', label: '已取消' },
    { value: '7', label: '已退款' }
  ];

  useEffect(() => {
    loadOrders();
  }, []);

  const loadOrders = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token');
      if (!token) {
        toast.error('请先登录');
        return;
      }

      const response = await fetch(`${process.env.REACT_APP_API_URL || 'https://localhost:7037/api'}/admin/orders`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error('获取订单列表失败');
      }

      const data = await response.json();
      setOrders(data);
    } catch (error) {
      console.error('Error loading orders:', error);
      toast.error('加载订单失败');
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status: string | number) => {
    const colorMap: { [key: string]: string } = {
      // 数字枚举值
      '0': 'text-yellow-600 bg-yellow-100',      // Pending
      '1': 'text-blue-600 bg-blue-100',          // Paid
      '2': 'text-green-600 bg-green-100',        // Confirmed
      '3': 'text-purple-600 bg-purple-100',      // Shipped
      '4': 'text-indigo-600 bg-indigo-100',      // Delivered
      '5': 'text-green-700 bg-green-200',        // Completed
      '6': 'text-red-600 bg-red-100',            // Cancelled
      '7': 'text-gray-600 bg-gray-100',          // Refunded
      // 字符串形式
      'Pending': 'text-yellow-600 bg-yellow-100',
      'Paid': 'text-blue-600 bg-blue-100',
      'Confirmed': 'text-green-600 bg-green-100',
      'Shipped': 'text-purple-600 bg-purple-100',
      'Delivered': 'text-indigo-600 bg-indigo-100',
      'Completed': 'text-green-700 bg-green-200',
      'Cancelled': 'text-red-600 bg-red-100',
      'Refunded': 'text-gray-600 bg-gray-100'
    };
    return colorMap[status.toString()] || 'text-gray-600 bg-gray-100';
  };

  const getStatusText = (status: string | number) => {
    // 处理数字枚举值
    const statusMap: { [key: string]: string } = {
      '0': '待支付',      // Pending
      '1': '已支付',      // Paid
      '2': '已确认',      // Confirmed
      '3': '已发货',      // Shipped
      '4': '已送达',      // Delivered
      '5': '已完成',      // Completed
      '6': '已取消',      // Cancelled
      '7': '已退款',      // Refunded
      // 也支持字符串形式
      'Pending': '待支付',
      'Paid': '已支付',
      'Confirmed': '已确认',
      'Shipped': '已发货',
      'Delivered': '已送达',
      'Completed': '已完成',
      'Cancelled': '已取消',
      'Refunded': '已退款'
    };
    return statusMap[status.toString()] || status.toString();
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleString('zh-CN');
  };

  const formatPrice = (price: number) => {
    return `¥${price.toFixed(2)}`;
  };

  // 筛选订单
  const filteredOrders = orders.filter(order => {
    const matchesSearch = order.id.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         order.customerName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         order.phoneNumber?.includes(searchTerm);
    const matchesStatus = !statusFilter || order.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  // 处理订单状态更新
  const handleStatusUpdate = async (orderId: string, newStatus: string) => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        toast.error('请先登录');
        return;
      }

      const response = await fetch(`${process.env.REACT_APP_API_URL || 'https://localhost:7037/api'}/admin/orders/${orderId}/status`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ status: newStatus }),
      });

      if (!response.ok) {
        throw new Error('更新订单状态失败');
      }

      toast.success('订单状态更新成功');
      loadOrders();
    } catch (error) {
      console.error('Error updating order status:', error);
      toast.error('更新订单状态失败');
    }
  };

  // 处理确认订单
  const handleConfirmOrder = async (orderId: string) => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        toast.error('请先登录');
        return;
      }

      const response = await fetch(`${process.env.REACT_APP_API_URL || 'https://localhost:7037/api'}/admin/orders/${orderId}/confirm`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error('确认订单失败');
      }

      toast.success('订单确认成功');
      loadOrders();
    } catch (error) {
      console.error('Error confirming order:', error);
      toast.error('确认订单失败');
    }
  };

  // 处理发货
  const handleShipOrder = async () => {
    if (!selectedOrder || !trackingNumber.trim()) {
      toast.error('请输入快递单号');
      return;
    }

    try {
      const token = localStorage.getItem('token');
      if (!token) {
        toast.error('请先登录');
        return;
      }

      const response = await fetch(`${process.env.REACT_APP_API_URL || 'https://localhost:7037/api'}/admin/orders/${selectedOrder.id}/ship`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ trackingNumber: trackingNumber.trim() }),
      });

      if (!response.ok) {
        throw new Error('发货失败');
      }

      toast.success('订单发货成功');
      setShowShipmentModal(false);
      setTrackingNumber('');
      setSelectedOrder(null);
      loadOrders();
    } catch (error) {
      console.error('Error shipping order:', error);
      toast.error('发货失败');
    }
  };

  // 处理取消订单
  const handleCancelOrder = async (orderId: string) => {
    if (!window.confirm('确定要取消这个订单吗？')) {
      return;
    }

    try {
      const token = localStorage.getItem('token');
      if (!token) {
        toast.error('请先登录');
        return;
      }

      const response = await fetch(`${process.env.REACT_APP_API_URL || 'https://localhost:7037/api'}/admin/orders/${orderId}/cancel`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error('取消订单失败');
      }

      toast.success('订单取消成功');
      loadOrders();
    } catch (error) {
      console.error('Error cancelling order:', error);
      toast.error('取消订单失败');
    }
  };

  // 批量操作
  const handleBatchStatusUpdate = async (newStatus: string) => {
    if (selectedOrders.length === 0) {
      toast.error('请选择要操作的订单');
      return;
    }

    if (!window.confirm(`确定要将选中的 ${selectedOrders.length} 个订单状态更新为 ${getStatusText(newStatus)} 吗？`)) {
      return;
    }

    try {
      const token = localStorage.getItem('token');
      if (!token) {
        toast.error('请先登录');
        return;
      }

      const response = await fetch(`${process.env.REACT_APP_API_URL || 'https://localhost:7037/api'}/admin/orders/batch-status`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ 
          orderIds: selectedOrders,
          status: newStatus 
        }),
      });

      if (!response.ok) {
        throw new Error('批量更新失败');
      }

      const result = await response.json();
      toast.success(`批量更新成功：${result.successCount} 个订单`);
      setSelectedOrders([]);
      loadOrders();
    } catch (error) {
      console.error('Error batch updating orders:', error);
      toast.error('批量更新失败');
    }
  };

  // 全选/取消全选
  const handleSelectAll = () => {
    if (selectedOrders.length === filteredOrders.length) {
      setSelectedOrders([]);
    } else {
      setSelectedOrders(filteredOrders.map(order => order.id));
    }
  };

  // 选择单个订单
  const handleSelectOrder = (orderId: string) => {
    setSelectedOrders(prev => 
      prev.includes(orderId) 
        ? prev.filter(id => id !== orderId)
        : [...prev, orderId]
    );
  };

  if (loading) {
    return (
      <div className={`flex items-center justify-center h-64 ${className}`}>
        <div className="text-center">
          <RefreshCw className="w-8 h-8 animate-spin text-blue-600 mx-auto mb-4" />
          <p className="text-gray-600">加载订单中...</p>
        </div>
      </div>
    );
  }

  return (
    <div className={`space-y-6 ${className}`}>
      {/* 头部操作栏 */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div>
            <h2 className="text-2xl font-bold text-gray-900">订单管理</h2>
            <p className="text-gray-600">管理所有订单状态和发货信息</p>
          </div>
          <div className="flex items-center space-x-3">
            <button
              onClick={loadOrders}
              className="px-4 py-2 text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50 flex items-center"
            >
              <RefreshCw className="w-4 h-4 mr-2" />
              刷新
            </button>
          </div>
        </div>

        {/* 搜索和筛选 */}
        <div className="mt-6 flex flex-col sm:flex-row gap-4">
          <div className="flex-1">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
              <input
                type="text"
                placeholder="搜索订单ID、客户姓名或电话..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
          </div>
          <div className="sm:w-48">
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              {statusOptions.map(option => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>
        </div>

        {/* 批量操作 */}
        {selectedOrders.length > 0 && (
          <div className="mt-4 p-4 bg-blue-50 rounded-lg">
            <div className="flex items-center justify-between">
              <span className="text-blue-700 font-medium">
                已选择 {selectedOrders.length} 个订单
              </span>
              <div className="flex items-center space-x-2">
                <select
                  onChange={(e) => {
                    if (e.target.value) {
                      handleBatchStatusUpdate(e.target.value);
                      e.target.value = '';
                    }
                  }}
                  className="px-3 py-1 text-sm border border-blue-300 rounded focus:ring-2 focus:ring-blue-500"
                >
                  <option value="">批量操作</option>
                  <option value="Confirmed">批量确认</option>
                  <option value="Shipped">批量发货</option>
                  <option value="Delivered">批量送达</option>
                  <option value="Cancelled">批量取消</option>
                </select>
                <button
                  onClick={() => setSelectedOrders([])}
                  className="px-3 py-1 text-sm text-gray-600 hover:text-gray-800"
                >
                  取消选择
                </button>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* 订单列表 */}
      <div className="bg-white rounded-lg shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left">
                  <input
                    type="checkbox"
                    checked={selectedOrders.length === filteredOrders.length && filteredOrders.length > 0}
                    onChange={handleSelectAll}
                    className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  订单信息
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  客户信息
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  金额
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  状态
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  创建时间
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  操作
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredOrders.map((order) => (
                <tr key={order.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap">
                    <input
                      type="checkbox"
                      checked={selectedOrders.includes(order.id)}
                      onChange={() => handleSelectOrder(order.id)}
                      className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                    />
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div>
                      <div className="text-sm font-medium text-gray-900">
                        {order.id.substring(0, 8)}...
                      </div>
                      <div className="text-sm text-gray-500">
                        {order.items.length} 件商品
                      </div>
                      {order.trackingNumber && (
                        <div className="text-xs text-blue-600">
                          快递: {order.trackingNumber}
                        </div>
                      )}
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div>
                      <div className="text-sm font-medium text-gray-900">
                        {order.customerName || order.shippingName || 'N/A'}
                      </div>
                      <div className="text-sm text-gray-500">
                        {order.phoneNumber || order.shippingPhone || 'N/A'}
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {formatPrice(order.totalAmount)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getStatusColor(order.status)}`}>
                      {getStatusText(order.status)}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {formatDate(order.createdAt)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <div className="flex items-center space-x-2">
                      <button
                        onClick={() => {
                          setSelectedOrder(order);
                          setShowOrderDetail(true);
                        }}
                        className="text-blue-600 hover:text-blue-900"
                        title="查看详情"
                      >
                        <Eye className="w-4 h-4" />
                      </button>
                      
                      {/* 状态更改下拉菜单 */}
                      <div className="relative">
                        <select
                          value={order.status}
                          onChange={(e) => {
                            if (e.target.value !== order.status.toString()) {
                              if (window.confirm(`确定要将订单状态从"${getStatusText(order.status)}"更改为"${getStatusText(e.target.value)}"吗？`)) {
                                handleStatusUpdate(order.id, e.target.value);
                              } else {
                                // 重置选择框的值
                                e.target.value = order.status.toString();
                              }
                            }
                          }}
                          className="text-xs border border-gray-300 rounded px-2 py-1 bg-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                          title="更改订单状态"
                        >
                          <option value="0">待支付</option>
                          <option value="1">已支付</option>
                          <option value="2">已确认</option>
                          <option value="3">已发货</option>
                          <option value="4">已送达</option>
                          <option value="5">已完成</option>
                          <option value="6">已取消</option>
                          <option value="7">已退款</option>
                        </select>
                      </div>
                      
                      {/* 快捷操作按钮 */}
                      {(order.status === '1' || order.status === 'Paid') && (
                        <button
                          onClick={() => handleConfirmOrder(order.id)}
                          className="text-green-600 hover:text-green-900"
                          title="确认订单"
                        >
                          <CheckCircle className="w-4 h-4" />
                        </button>
                      )}
                      
                      {(order.status === '2' || order.status === 'Confirmed') && (
                        <button
                          onClick={() => {
                            setSelectedOrder(order);
                            setShowShipmentModal(true);
                          }}
                          className="text-purple-600 hover:text-purple-900"
                          title="发货"
                        >
                          <Truck className="w-4 h-4" />
                        </button>
                      )}
                      
                      {(order.status === '3' || order.status === 'Shipped') && (
                        <button
                          onClick={() => handleStatusUpdate(order.id, '4')}
                          className="text-indigo-600 hover:text-indigo-900"
                          title="标记送达"
                        >
                          <Package className="w-4 h-4" />
                        </button>
                      )}
                      
                      {((order.status === '0' || order.status === 'Pending') || (order.status === '1' || order.status === 'Paid')) && (
                        <button
                          onClick={() => handleCancelOrder(order.id)}
                          className="text-red-600 hover:text-red-900"
                          title="取消订单"
                        >
                          <XCircle className="w-4 h-4" />
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {filteredOrders.length === 0 && (
          <div className="text-center py-12">
            <Package className="w-12 h-12 text-gray-400 mx-auto mb-4" />
            <h3 className="text-lg font-medium text-gray-900 mb-2">没有找到订单</h3>
            <p className="text-gray-500">
              {searchTerm || statusFilter ? '请尝试调整搜索条件' : '还没有任何订单'}
            </p>
          </div>
        )}
      </div>

      {/* 订单详情模态框 */}
      {showOrderDetail && selectedOrder && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg max-w-4xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h3 className="text-xl font-bold text-gray-900">订单详情</h3>
                <button
                  onClick={() => setShowOrderDetail(false)}
                  className="text-gray-400 hover:text-gray-600"
                >
                  <XCircle className="w-6 h-6" />
                </button>
              </div>

              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* 订单信息 */}
                <div className="space-y-4">
                  <div>
                    <h4 className="font-medium text-gray-900 mb-2">订单信息</h4>
                    <div className="bg-gray-50 p-4 rounded-lg space-y-2">
                      <div className="flex justify-between">
                        <span className="text-gray-600">订单ID:</span>
                        <span className="font-mono text-sm">{selectedOrder.id}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-600">状态:</span>
                        <span className={`px-2 py-1 text-xs font-semibold rounded-full ${getStatusColor(selectedOrder.status)}`}>
                          {getStatusText(selectedOrder.status)}
                        </span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-600">总金额:</span>
                        <span className="font-medium">{formatPrice(selectedOrder.totalAmount)}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-600">支付方式:</span>
                        <span>{selectedOrder.paymentMethod || 'N/A'}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-600">创建时间:</span>
                        <span>{formatDate(selectedOrder.createdAt)}</span>
                      </div>
                      {selectedOrder.paidAt && (
                        <div className="flex justify-between">
                          <span className="text-gray-600">支付时间:</span>
                          <span>{formatDate(selectedOrder.paidAt)}</span>
                        </div>
                      )}
                      {selectedOrder.shippedAt && (
                        <div className="flex justify-between">
                          <span className="text-gray-600">发货时间:</span>
                          <span>{formatDate(selectedOrder.shippedAt)}</span>
                        </div>
                      )}
                    </div>
                  </div>

                  {/* 收货信息 */}
                  <div>
                    <h4 className="font-medium text-gray-900 mb-2">收货信息</h4>
                    <div className="bg-gray-50 p-4 rounded-lg space-y-2">
                      <div>
                        <span className="text-gray-600">收货人:</span>
                        <span className="ml-2">{selectedOrder.customerName || selectedOrder.shippingName || 'N/A'}</span>
                      </div>
                      <div>
                        <span className="text-gray-600">联系电话:</span>
                        <span className="ml-2">{selectedOrder.phoneNumber || selectedOrder.shippingPhone || 'N/A'}</span>
                      </div>
                      <div>
                        <span className="text-gray-600">收货地址:</span>
                        <p className="mt-1 text-sm text-gray-800">{selectedOrder.shippingAddress}</p>
                      </div>
                    </div>
                  </div>
                </div>

                {/* 商品列表 */}
                <div>
                  <h4 className="font-medium text-gray-900 mb-2">商品列表</h4>
                  <div className="space-y-3">
                    {selectedOrder.items.map((item: OrderItem) => (
                      <div key={item.id} className="flex items-center space-x-3 p-3 border border-gray-200 rounded-lg">
                        <div className="w-12 h-12 bg-gray-200 rounded-lg flex items-center justify-center">
                          <span className="text-lg">📦</span>
                        </div>
                        <div className="flex-1">
                          <h5 className="font-medium text-gray-900">
                            {item.product?.name || item.productName || `商品 ${item.productId}`}
                          </h5>
                          <p className="text-sm text-gray-600">数量: {item.quantity}</p>
                        </div>
                        <div className="text-right">
                          <p className="font-medium text-gray-900">{formatPrice(item.price)}</p>
                          <p className="text-sm text-gray-600">小计: {formatPrice(item.price * item.quantity)}</p>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* 发货模态框 */}
      {showShipmentModal && selectedOrder && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg max-w-md w-full">
            <div className="p-6">
              <div className="flex justify-between items-center mb-4">
                <h3 className="text-lg font-bold text-gray-900">订单发货</h3>
                <button
                  onClick={() => {
                    setShowShipmentModal(false);
                    setTrackingNumber('');
                    setSelectedOrder(null);
                  }}
                  className="text-gray-400 hover:text-gray-600"
                >
                  <XCircle className="w-5 h-5" />
                </button>
              </div>

              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    订单ID
                  </label>
                  <input
                    type="text"
                    value={selectedOrder.id}
                    disabled
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg bg-gray-50 text-gray-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    快递单号 *
                  </label>
                  <input
                    type="text"
                    value={trackingNumber}
                    onChange={(e) => setTrackingNumber(e.target.value)}
                    placeholder="请输入快递单号"
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                </div>

                <div className="flex justify-end space-x-3 pt-4">
                  <button
                    onClick={() => {
                      setShowShipmentModal(false);
                      setTrackingNumber('');
                      setSelectedOrder(null);
                    }}
                    className="px-4 py-2 text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50"
                  >
                    取消
                  </button>
                  <button
                    onClick={handleShipOrder}
                    className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
                  >
                    确认发货
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default OrderManagement;
