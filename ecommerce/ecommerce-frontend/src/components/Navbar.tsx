import React, { useState, useEffect } from 'react';
import { ShoppingCart, User, Menu, X, Users, Settings, LogIn, LogOut } from 'lucide-react';
import { cartService } from '../services/cartService';
import { User as UserType } from '../interfaces/User';
import Button from './ui/Button';

interface NavbarProps {
  onCartClick?: () => void;
  onLoginClick?: () => void;
  onUsersClick?: () => void;
  onAdminClick?: () => void;
  onHomeClick?: () => void;
  onProductsClick?: () => void;
  onOrdersClick?: () => void;
  onAddressesClick?: () => void;
  isAuthenticated?: boolean;
  user?: UserType | null;
}

const Navbar: React.FC<NavbarProps> = ({ 
  onCartClick, 
  onLoginClick, 
  onUsersClick, 
  onAdminClick, 
  onHomeClick, 
  onProductsClick, 
  onOrdersClick, 
  onAddressesClick,
  isAuthenticated, 
  user 
}) => {
  const [cartItemCount, setCartItemCount] = useState(0);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  useEffect(() => {
    updateCartCount();
    // 监听购物车变化
    const interval = setInterval(updateCartCount, 1000);
    return () => clearInterval(interval);
  }, []);

  const updateCartCount = () => {
    setCartItemCount(cartService.getCartItemCount());
  };

  return (
    <nav className="bg-white shadow-lg">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          <div className="flex items-center">
            <div className="flex-shrink-0">
              <h1 className="text-xl font-bold text-blue-600">电商系统</h1>
            </div>
            
            {/* 桌面端导航 */}
            <div className="hidden md:ml-6 md:flex md:space-x-8">
              <button 
                onClick={onHomeClick}
                className="text-gray-900 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium transition-colors"
              >
                首页
              </button>
              <button 
                onClick={onProductsClick}
                className="text-gray-900 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium transition-colors"
              >
                产品
              </button>
              {isAuthenticated && (
                <>
                  <button 
                    onClick={onOrdersClick}
                    className="text-gray-900 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium transition-colors"
                  >
                    订单
                  </button>
                  <button 
                    onClick={onAddressesClick}
                    className="text-gray-900 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium transition-colors"
                  >
                    地址管理
                  </button>
                </>
              )}
            </div>
          </div>

          <div className="flex items-center space-x-4">
            {/* 购物车按钮 */}
            <button
              onClick={onCartClick}
              className="relative p-2 text-gray-600 hover:text-blue-600 transition-colors"
            >
              <ShoppingCart className="w-6 h-6" />
              {cartItemCount > 0 && (
                <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
                  {cartItemCount}
                </span>
              )}
            </button>

            {/* 后台管理按钮 - 仅管理员可见 */}
            {isAuthenticated && user?.role === 'Admin' && (
              <button
                onClick={onAdminClick}
                className="p-2 text-gray-600 hover:text-blue-600 transition-colors"
                title="后台管理"
              >
                <Settings className="w-6 h-6" />
              </button>
            )}

            {/* 用户管理按钮 - 仅管理员可见 */}
            {isAuthenticated && user?.role === 'Admin' && (
              <button
                onClick={onUsersClick}
                className="p-2 text-gray-600 hover:text-blue-600 transition-colors"
                title="用户管理"
              >
                <Users className="w-6 h-6" />
              </button>
            )}

            {/* 用户信息 */}
            {isAuthenticated && user ? (
              <div className="flex items-center space-x-2">
                <span className="text-sm text-gray-700 hidden md:block">
                  {user.userName || user.email}
                </span>
                <button
                  onClick={onLoginClick}
                  className="p-2 text-gray-600 hover:text-red-600 transition-colors"
                  title="退出登录"
                >
                  <LogOut className="w-6 h-6" />
                </button>
              </div>
            ) : (
              <button
                onClick={onLoginClick}
                className="p-2 text-gray-600 hover:text-green-600 transition-colors"
                title="登录"
              >
                <LogIn className="w-6 h-6" />
              </button>
            )}

            {/* 移动端菜单按钮 */}
            <div className="md:hidden">
              <button
                onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                className="p-2 text-gray-600 hover:text-blue-600 transition-colors"
              >
                {isMobileMenuOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* 移动端菜单 */}
      {isMobileMenuOpen && (
        <div className="md:hidden">
          <div className="px-2 pt-2 pb-3 space-y-1 sm:px-3 bg-white border-t">
            <button
              onClick={() => {
                onHomeClick?.();
                setIsMobileMenuOpen(false);
              }}
              className="text-gray-900 hover:text-blue-600 block px-3 py-2 rounded-md text-base font-medium w-full text-left"
            >
              首页
            </button>
            <button
              onClick={() => {
                onProductsClick?.();
                setIsMobileMenuOpen(false);
              }}
              className="text-gray-900 hover:text-blue-600 block px-3 py-2 rounded-md text-base font-medium w-full text-left"
            >
              产品
            </button>
            {isAuthenticated && (
              <>
                <button
                  onClick={() => {
                    onOrdersClick?.();
                    setIsMobileMenuOpen(false);
                  }}
                  className="text-gray-900 hover:text-blue-600 block px-3 py-2 rounded-md text-base font-medium w-full text-left"
                >
                  订单
                </button>
                <button
                  onClick={() => {
                    onAddressesClick?.();
                    setIsMobileMenuOpen(false);
                  }}
                  className="text-gray-900 hover:text-blue-600 block px-3 py-2 rounded-md text-base font-medium w-full text-left"
                >
                  地址管理
                </button>
              </>
            )}
          </div>
        </div>
      )}
    </nav>
  );
};

export default Navbar;
