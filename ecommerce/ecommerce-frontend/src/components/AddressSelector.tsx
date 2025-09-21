import React, { useState, useEffect } from 'react';
import { Address } from '../interfaces/Address';
import AddressService from '../services/addressService';
import { MapPin, Plus, Star, Phone, User, Check } from 'lucide-react';
import { toast } from 'react-hot-toast';

interface AddressSelectorProps {
  selectedAddressId?: string;
  onAddressSelect: (address: Address) => void;
  onAddNewAddress?: () => void;
  showAddButton?: boolean;
}

const AddressSelector: React.FC<AddressSelectorProps> = ({
  selectedAddressId,
  onAddressSelect,
  onAddNewAddress,
  showAddButton = true
}) => {
  const [addresses, setAddresses] = useState<Address[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedId, setSelectedId] = useState<string | undefined>(selectedAddressId);

  useEffect(() => {
    loadAddresses();
  }, []);

  useEffect(() => {
    setSelectedId(selectedAddressId);
  }, [selectedAddressId]);

  const loadAddresses = async () => {
    try {
      setLoading(true);
      const data = await AddressService.getInstance().getAddresses();
      setAddresses(data);
      
      // 如果没有选中的地址，自动选择默认地址
      if (!selectedId && data.length > 0) {
        const defaultAddress = data.find(addr => addr.isDefault) || data[0];
        setSelectedId(defaultAddress.id);
        onAddressSelect(defaultAddress);
      }
    } catch (error) {
      console.error('Failed to load addresses:', error);
      toast.error('加载地址失败');
    } finally {
      setLoading(false);
    }
  };

  const handleAddressSelect = (address: Address) => {
    setSelectedId(address.id);
    onAddressSelect(address);
  };

  if (loading) {
    return (
      <div className="space-y-4">
        <h3 className="text-lg font-semibold text-gray-900">选择收货地址</h3>
        <div className="text-center py-8">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-2 text-gray-600">加载中...</p>
        </div>
      </div>
    );
  }

  if (addresses.length === 0) {
    return (
      <div className="space-y-4">
        <h3 className="text-lg font-semibold text-gray-900">选择收货地址</h3>
        <div className="text-center py-8 border-2 border-dashed border-gray-300 rounded-lg">
          <MapPin className="w-12 h-12 text-gray-400 mx-auto mb-4" />
          <p className="text-gray-600 mb-4">您还没有添加任何地址</p>
          {showAddButton && onAddNewAddress && (
            <button
              onClick={onAddNewAddress}
              className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
            >
              添加地址
            </button>
          )}
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h3 className="text-lg font-semibold text-gray-900">选择收货地址</h3>
        {showAddButton && onAddNewAddress && (
          <button
            onClick={onAddNewAddress}
            className="flex items-center px-3 py-1 text-sm text-blue-600 hover:text-blue-700 transition-colors"
          >
            <Plus className="w-4 h-4 mr-1" />
            添加新地址
          </button>
        )}
      </div>

      <div className="space-y-3">
        {addresses.map((address) => (
          <div
            key={address.id}
            className={`border-2 rounded-lg p-4 cursor-pointer transition-all ${
              selectedId === address.id
                ? 'border-blue-500 bg-blue-50'
                : 'border-gray-200 hover:border-gray-300'
            }`}
            onClick={() => handleAddressSelect(address)}
          >
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <div className="flex items-center mb-2">
                  <User className="w-4 h-4 text-gray-500 mr-2" />
                  <span className="font-medium text-gray-900">{address.name}</span>
                  {address.isDefault && (
                    <Star className="w-4 h-4 text-yellow-500 ml-2 fill-current" />
                  )}
                </div>
                
                <div className="flex items-center text-gray-600 mb-1">
                  <Phone className="w-4 h-4 mr-2" />
                  <span className="text-sm">{address.phone}</span>
                </div>
                
                <div className="flex items-start text-gray-600">
                  <MapPin className="w-4 h-4 mr-2 mt-0.5" />
                  <span className="text-sm">{address.fullAddress}</span>
                </div>
              </div>
              
              {selectedId === address.id && (
                <Check className="w-5 h-5 text-blue-600" />
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default AddressSelector;
