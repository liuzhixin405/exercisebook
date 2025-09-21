import React, { useState, useEffect } from 'react';
import { PaymentRequest, PaymentResult } from '../interfaces/PaymentModels';
import paymentService from '../services/paymentService';
import { toast } from 'react-hot-toast';

interface PaymentProcessorProps {
  orderId: string;
  amount: number;
  currency?: string;
  onPaymentSuccess?: (result: PaymentResult) => void;
  onPaymentFailure?: (error: string) => void;
  onCancel?: () => void;
}

interface PaymentMethodOption {
  value: string;
  name: string;
  description: string;
}

const PaymentProcessor: React.FC<PaymentProcessorProps> = ({
  orderId,
  amount,
  currency = 'CNY',
  onPaymentSuccess,
  onPaymentFailure,
  onCancel
}) => {
  const [paymentMethod, setPaymentMethod] = useState<string>('CreditCard');
  const [isProcessing, setIsProcessing] = useState(false);
  const [availableMethods, setAvailableMethods] = useState<PaymentMethodOption[]>([]);

  useEffect(() => {
    loadPaymentMethods();
  }, []);

  const loadPaymentMethods = async () => {
    try {
      const methods = await paymentService.getPaymentMethods();
      setAvailableMethods(methods);
    } catch (error) {
      console.error('Failed to load payment methods:', error);
      // Fallback to default methods
      setAvailableMethods([
        { value: 'CreditCard', name: '信用卡', description: '使用信用卡支付' },
        { value: 'DebitCard', name: '借记卡', description: '使用借记卡支付' },
        { value: 'PayPal', name: 'PayPal', description: '使用PayPal账户支付' },
        { value: 'Alipay', name: '支付宝', description: '使用支付宝支付' },
        { value: 'WeChatPay', name: '微信支付', description: '使用微信支付' },
        { value: 'BankTransfer', name: '银行转账', description: '银行转账支付' },
        { value: 'Cash', name: '现金', description: '货到付款' }
      ]);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    setIsProcessing(true);
    
    try {
      console.log('Payment request details:', { orderId, paymentMethod, amount, currency });
      
      const paymentRequest: PaymentRequest = {
        orderId,
        paymentMethod,
        amount,
        currency,
        description: `Order payment for ${orderId}`,
        metadata: {
          timestamp: new Date().toISOString(),
          source: 'frontend'
        }
      };

      // 调用支付接口记录支付日志
      const result = await paymentService.processPayment(paymentRequest);
      
      console.log('Payment result:', result);
      
      // 支付成功处理
      toast.success(`支付成功！使用${getPaymentMethodName(paymentMethod)}支付了${currency} ${amount.toFixed(2)}`);
      
      // 确保返回结果包含订单ID
      const successResult = {
        ...result,
        orderId: orderId
      };
      
      onPaymentSuccess?.(successResult);
      
    } catch (error) {
      console.error('Payment error:', error);
      const errorMessage = error instanceof Error ? error.message : 'Payment processing failed';
      toast.error(`支付失败: ${errorMessage}`);
      onPaymentFailure?.(errorMessage);
    } finally {
      setIsProcessing(false);
    }
  };

  const getPaymentMethodName = (method: string) => {
    const methodMap: { [key: string]: string } = {
      'CreditCard': '信用卡',
      'DebitCard': '借记卡',
      'PayPal': 'PayPal',
      'Alipay': '支付宝',
      'WeChatPay': '微信支付',
      'BankTransfer': '银行转账',
      'Cash': '现金'
    };
    return methodMap[method] || method;
  };

  const getPaymentMethodIcon = (method: string) => {
    switch (method) {
      case 'CreditCard':
      case 'DebitCard':
        return '💳';
      case 'PayPal':
        return '🔵';
      case 'Alipay':
        return '🔵';
      case 'WeChatPay':
        return '🟢';
      case 'BankTransfer':
        return '🏦';
      case 'Cash':
        return '💵';
      default:
        return '💰';
    }
  };

  return (
    <div className="max-w-md mx-auto bg-white rounded-lg shadow-lg p-6">
      <div className="text-center mb-6">
        <h2 className="text-2xl font-bold text-gray-900">选择支付方式</h2>
        <p className="text-gray-600 mt-2">选择您的支付方式完成订单</p>
      </div>

      <div className="mb-6 p-4 bg-blue-50 rounded-lg">
        <div className="flex justify-between items-center">
          <span className="text-gray-700 font-medium">Order Total:</span>
          <span className="text-2xl font-bold text-blue-600">
            {currency} {amount.toFixed(2)}
          </span>
        </div>
        <div className="text-sm text-gray-500 mt-1">Order ID: {orderId}</div>
      </div>

      <form onSubmit={handleSubmit} className="space-y-4">
        {/* Payment Method Selection */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Payment Method *
          </label>
          <div className="grid grid-cols-2 gap-2">
            {availableMethods.map((method) => (
              <button
                key={method.value}
                type="button"
                onClick={() => setPaymentMethod(method.value)}
                className={`p-3 border rounded-lg text-center transition-colors ${
                  paymentMethod === method.value
                    ? 'border-blue-500 bg-blue-50 text-blue-700'
                    : 'border-gray-300 hover:border-gray-400'
                }`}
                title={method.description}
              >
                <div className="text-xl mb-1">{getPaymentMethodIcon(method.value)}</div>
                <div className="text-sm font-medium">{method.name}</div>
              </button>
            ))}
          </div>
        </div>

        {/* Action Buttons */}
        <div className="flex space-x-3 pt-4">
          <button
            type="button"
            onClick={onCancel}
            disabled={isProcessing}
            className="flex-1 px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:border-transparent disabled:opacity-50"
          >
            取消
          </button>
          <button
            type="submit"
            disabled={isProcessing}
            className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isProcessing ? (
              <div className="flex items-center justify-center">
                <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                处理中...
              </div>
            ) : (
              '确认支付'
            )}
          </button>
        </div>
      </form>

      {/* Security Notice */}
      <div className="mt-6 text-center">
        <div className="flex items-center justify-center text-green-600 mb-2">
          <svg className="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z" clipRule="evenodd" />
          </svg>
          Secure Payment
        </div>
        <p className="text-xs text-gray-500">
          Your payment information is encrypted and secure
        </p>
      </div>
    </div>
  );
};

export default PaymentProcessor;
