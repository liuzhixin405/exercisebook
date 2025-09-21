import React, { useState, useEffect } from 'react';
import { CartItem } from '../interfaces';
import { formatPrice } from '../utils/format';
import { cartService } from '../services/cartService';
import Button from './ui/Button';
import { Card, CardContent, CardHeader, CardTitle } from './ui/Card';
import { Trash2, Minus, Plus, ShoppingCart, CreditCard } from 'lucide-react';
import PaymentProcessor from './PaymentProcessor';
import { useAuth } from '../contexts/AuthContext';
import { toast } from 'react-hot-toast';
import { createOrder, validateAddress } from '../services/orderService';
import { CreateOrderDto, CreateOrderItemDto } from '../interfaces';

interface CartProps {
  onCheckout?: () => void;
}

const Cart: React.FC<CartProps> = ({ onCheckout }) => {
  const [cartItems, setCartItems] = useState<CartItem[]>([]);
  const [total, setTotal] = useState(0);
  const [showPayment, setShowPayment] = useState(false);
  const [isCreatingOrder, setIsCreatingOrder] = useState(false);
  const [currentOrderId, setCurrentOrderId] = useState<string>('');
  const { user, isAuthenticated } = useAuth();

  useEffect(() => {
    updateCart();
  }, []);

  const updateCart = () => {
    const items = cartService.getCart();
    setCartItems(items);
    const totalAmount = cartService.getCartTotal();
    setTotal(totalAmount);
    console.log('Cart updated:', { items, totalAmount });
  };

  const handleUpdateQuantity = (productId: string, quantity: number) => {
    cartService.updateQuantity(productId, quantity);
    updateCart();
  };

  const handleRemoveItem = (productId: string) => {
    cartService.removeFromCart(productId);
    updateCart();
  };

  const handleClearCart = () => {
    cartService.clearCart();
    updateCart();
  };

  const handleCheckout = async () => {
    console.log('=== CHECKOUT BUTTON CLICKED ===');
    console.log('Checkout clicked, checking conditions...');
    console.log('isAuthenticated:', isAuthenticated);
    console.log('cartItems.length:', cartItems.length);
    console.log('user:', user);
    console.log('user phoneNumber:', user?.phoneNumber);
    console.log('user address:', user?.address);

    if (!isAuthenticated) {
      console.log('User not authenticated');
      toast.error('Please login to checkout');
      return;
    }

    if (cartItems.length === 0) {
      console.log('Cart is empty');
      toast.error('Cart is empty');
      return;
    }

    // 检查用户信息是否完整，如果为空则使用默认值
    const phoneNumber = user?.phoneNumber || '13800000000';
    const address = user?.address || '北京市朝阳区某某街道123号';
    
    console.log('Using user info:', { phoneNumber, address });

    console.log('All checks passed, proceeding with checkout...');

    setIsCreatingOrder(true);
    
    try {
      // Create order items
      const orderItems: CreateOrderItemDto[] = cartItems.map(item => ({
        productId: item.product.id,
        quantity: item.quantity
      }));

      // Create order DTO
      const orderDto: CreateOrderDto = {
        userId: user?.id,
        // 不提供 addressId，让后端自动获取默认地址
        customerName: `${user?.firstName || ''} ${user?.lastName || ''}`.trim() || user?.userName || 'Customer',
        phoneNumber: phoneNumber,
        shippingAddress: address,
        paymentMethod: 'CreditCard', // Default payment method
        notes: 'Order from cart',
        items: orderItems
      };

      console.log('Validating address with data:', orderDto);

      // 先验证地址
      const addressValidation = await validateAddress(orderDto);
      
      if (!addressValidation.isValid) {
        if (addressValidation.requiresAddressManagement) {
          toast.error(addressValidation.message || '请先设置收货地址');
          window.dispatchEvent(new CustomEvent('app:navigate', { detail: 'addresses' }));
        } else {
          toast.error(addressValidation.message);
        }
        return;
      }

      console.log('Address validation passed, creating order...');

      // Create order
      const order = await createOrder(orderDto);
      
      // Store order ID and show payment processor
      setCurrentOrderId(order.id);
      setShowPayment(true);
      
      toast.success('Order created successfully! Please complete payment.');
      
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to create order';
      console.error('Order creation error:', error);
      
      // 检查是否是地址相关的错误
      if (errorMessage.includes('地址') || errorMessage.includes('Address') || errorMessage.includes('请先设置')) {
        toast.error('请先设置收货地址，然后重新尝试下单');
        // 延迟后询问用户是否前往设置地址
        setTimeout(() => {
          if (confirm('检测到您还没有设置收货地址，是否前往设置？')) {
            window.location.href = '/address';
          }
        }, 2000);
      } else {
        toast.error(errorMessage);
      }
    } finally {
      setIsCreatingOrder(false);
    }
  };

  const handlePaymentSuccess = (result: any) => {
    toast.success('Payment completed successfully!');
    cartService.clearCart();
    updateCart();
    setShowPayment(false);
    
    // 支付成功后跳转到订单列表页面
    setTimeout(() => {
      // 使用应用内部导航
      window.dispatchEvent(new CustomEvent('app:navigate', { detail: 'orders' }));
    }, 1500);
    
    onCheckout?.();
  };

  const handlePaymentFailure = (error: string) => {
    toast.error(`Payment failed: ${error}`);
  };

  const handlePaymentCancel = () => {
    setShowPayment(false);
  };

  if (cartItems.length === 0) {
    return (
      <Card className="p-8 text-center">
        <ShoppingCart className="w-16 h-16 mx-auto text-gray-400 mb-4" />
        <h3 className="text-lg font-semibold text-gray-600 mb-2">购物车为空</h3>
        <p className="text-gray-500">快去添加一些商品吧！</p>
      </Card>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-bold">购物车</h2>
        <Button variant="outline" size="sm" onClick={handleClearCart}>
          <Trash2 className="w-4 h-4 mr-1" />
          清空购物车
        </Button>
      </div>

      <div className="space-y-4">
        {cartItems.map((item) => (
          <Card key={item.product.id}>
            <CardContent className="p-4">
              <div className="flex items-center space-x-4">
                <img
                  src={item.product.imageUrl || 'https://via.placeholder.com/80x80'}
                  alt={item.product.name}
                  className="w-20 h-20 object-cover rounded"
                />
                
                <div className="flex-1">
                  <h3 className="font-semibold">{item.product.name}</h3>
                  <p className="text-sm text-gray-600">{item.product.description}</p>
                  <p className="text-lg font-bold text-blue-600">
                    {formatPrice(item.product.price)}
                  </p>
                </div>

                <div className="flex items-center space-x-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => handleUpdateQuantity(item.product.id, item.quantity - 1)}
                    disabled={item.quantity <= 1}
                  >
                    <Minus className="w-4 h-4" />
                  </Button>
                  
                  <span className="w-12 text-center font-semibold">
                    {item.quantity}
                  </span>
                  
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => handleUpdateQuantity(item.product.id, item.quantity + 1)}
                    disabled={item.quantity >= item.product.stock}
                  >
                    <Plus className="w-4 h-4" />
                  </Button>
                </div>

                <div className="text-right">
                  <p className="font-bold text-lg">
                    {formatPrice(item.product.price * item.quantity)}
                  </p>
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => handleRemoveItem(item.product.id)}
                    className="text-red-600 hover:text-red-700"
                  >
                    <Trash2 className="w-4 h-4" />
                  </Button>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>订单摘要</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-2">
            <div className="flex justify-between">
              <span>商品总数:</span>
              <span>{cartService.getCartItemCount()} 件</span>
            </div>
            <div className="flex justify-between text-lg font-bold">
              <span>总计:</span>
              <span className="text-blue-600">{formatPrice(total)}</span>
            </div>
          </div>
        </CardContent>
        <CardContent>
          <Button
            className="w-full"
            size="lg"
            onClick={handleCheckout}
            disabled={isCreatingOrder || !isAuthenticated}
          >
            {isCreatingOrder ? (
              <div className="flex items-center justify-center">
                <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                Creating Order...
              </div>
            ) : !isAuthenticated ? (
              'Login to Checkout'
            ) : (
              <div className="flex items-center justify-center">
                <CreditCard className="w-4 h-4 mr-2" />
                Proceed to Checkout
              </div>
            )}
          </Button>
        </CardContent>
      </Card>

      {/* Payment Processor Modal */}
      {showPayment && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg max-w-md w-full mx-4 max-h-[90vh] overflow-y-auto">
            <PaymentProcessor
              orderId={currentOrderId}
              amount={total}
              currency="CNY"
              onPaymentSuccess={handlePaymentSuccess}
              onPaymentFailure={handlePaymentFailure}
              onCancel={handlePaymentCancel}
            />
          </div>
        </div>
      )}
    </div>
  );
};

export default Cart;
