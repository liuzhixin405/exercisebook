import React, { useState } from 'react';
import Navbar from './components/Navbar';
import HomePage from './pages/HomePage';
import ProductList from './pages/ProductList';
import OrdersPage from './pages/OrdersPage';
import AddressPage from './pages/AddressPage';
import AdminDashboard from './pages/AdminDashboard';
import Cart from './components/Cart';
import LoginModal from './components/LoginModal';
import RegisterModal from './components/RegisterModal';
import UserManagement from './components/UserManagement';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import './App.css';

function AppContent() {
  const [currentView, setCurrentView] = useState<'home' | 'products' | 'orders' | 'addresses' | 'cart' | 'users' | 'admin'>('home');
  const [showLoginModal, setShowLoginModal] = useState(false);
  const [showRegisterModal, setShowRegisterModal] = useState(false);
  const { isAuthenticated, user, logout } = useAuth();
  // 监听全局导航事件，统一内部视图切换
  React.useEffect(() => {
    const handler = (e: Event) => {
      const detail = (e as CustomEvent<string>).detail;
      if (detail === 'addresses') {
        setCurrentView('addresses');
      } else if (detail === 'cart') {
        setCurrentView('cart');
      } else if (detail === 'orders') {
        setCurrentView('orders');
      } else if (detail === 'home') {
        setCurrentView('home');
      }
    };
    window.addEventListener('app:navigate', handler as EventListener);
    return () => window.removeEventListener('app:navigate', handler as EventListener);
  }, []);

  const handleHomeClick = () => {
    setCurrentView('home');
  };

  const handleProductsClick = () => {
    setCurrentView('products');
  };

  const handleOrdersClick = () => {
    setCurrentView('orders');
  };

  const handleAddressesClick = () => {
    setCurrentView('addresses');
  };

  const handleCartClick = () => {
    setCurrentView('cart');
  };

  const handleLoginClick = () => {
    if (isAuthenticated) {
      logout();
    } else {
      setShowLoginModal(true);
    }
  };

  const handleUsersClick = () => {
    setCurrentView('users');
  };

  const handleAdminClick = () => {
    setCurrentView('admin');
  };

  const handleCheckout = () => {
    // 这里可以添加结算逻辑
    console.log('Checkout clicked');
  };

  return (
    <div className="App">
      <Navbar 
        onHomeClick={handleHomeClick}
        onProductsClick={handleProductsClick}
        onOrdersClick={handleOrdersClick}
        onAddressesClick={handleAddressesClick}
        onCartClick={handleCartClick} 
        onLoginClick={handleLoginClick}
        onUsersClick={handleUsersClick}
        onAdminClick={handleAdminClick}
        isAuthenticated={isAuthenticated}
        user={user}
      />
      <main className="min-h-screen bg-gray-50">
        {currentView === 'home' ? (
          <HomePage />
        ) : currentView === 'products' ? (
          <ProductList />
        ) : currentView === 'orders' ? (
          <OrdersPage />
        ) : currentView === 'addresses' ? (
          <AddressPage />
        ) : currentView === 'cart' ? (
          <div className="container mx-auto px-4 py-8">
            <div className="flex items-center justify-between mb-6">
              <h1 className="text-3xl font-bold text-gray-900">购物车</h1>
              <button
                onClick={() => setCurrentView('home')}
                className="text-blue-600 hover:text-blue-700 font-medium"
              >
                ← 继续购物
              </button>
            </div>
            <Cart onCheckout={handleCheckout} />
          </div>
        ) : currentView === 'admin' ? (
          <AdminDashboard />
        ) : (
          <UserManagement />
        )}
      </main>
      
      <LoginModal 
        isOpen={showLoginModal} 
        onClose={() => setShowLoginModal(false)}
        onSwitchToRegister={() => {
          setShowLoginModal(false);
          setShowRegisterModal(true);
        }}
      />
      
      <RegisterModal 
        isOpen={showRegisterModal} 
        onClose={() => setShowRegisterModal(false)}
        onSwitchToLogin={() => {
          setShowRegisterModal(false);
          setShowLoginModal(true);
        }}
      />
    </div>
  );
}

function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  );
}

export default App;