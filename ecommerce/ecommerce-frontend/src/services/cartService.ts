import { CartItem, Product } from '../interfaces';

class CartService {
  private readonly CART_KEY = 'ecommerce_cart';

  getCart(): CartItem[] {
    const cartData = localStorage.getItem(this.CART_KEY);
    return cartData ? JSON.parse(cartData) : [];
  }

  addToCart(product: Product, quantity: number = 1): void {
    const cart = this.getCart();
    const existingItem = cart.find(item => item.product.id === product.id);

    if (existingItem) {
      existingItem.quantity += quantity;
    } else {
      cart.push({ product, quantity });
    }

    this.saveCart(cart);
  }

  removeFromCart(productId: string): void {
    const cart = this.getCart();
    const updatedCart = cart.filter(item => item.product.id !== productId);
    this.saveCart(updatedCart);
  }

  updateQuantity(productId: string, quantity: number): void {
    const cart = this.getCart();
    const item = cart.find(item => item.product.id === productId);
    
    if (item) {
      if (quantity <= 0) {
        this.removeFromCart(productId);
      } else {
        item.quantity = quantity;
        this.saveCart(cart);
      }
    }
  }

  clearCart(): void {
    localStorage.removeItem(this.CART_KEY);
  }

  getCartTotal(): number {
    const cart = this.getCart();
    return cart.reduce((total, item) => total + (item.product.price * item.quantity), 0);
  }

  getCartItemCount(): number {
    const cart = this.getCart();
    return cart.reduce((count, item) => count + item.quantity, 0);
  }

  private saveCart(cart: CartItem[]): void {
    localStorage.setItem(this.CART_KEY, JSON.stringify(cart));
  }
}

export const cartService = new CartService();
