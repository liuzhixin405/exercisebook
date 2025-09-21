export interface Address {
  id: string;
  userId: string;
  name: string;
  phone: string;
  province: string;
  city: string;
  district: string;
  street: string;
  postalCode: string;
  isDefault: boolean;
  fullAddress: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateAddressDto {
  name: string;
  phone: string;
  province: string;
  city: string;
  district: string;
  street: string;
  postalCode: string;
  isDefault: boolean;
}

export interface UpdateAddressDto {
  name: string;
  phone: string;
  province: string;
  city: string;
  district: string;
  street: string;
  postalCode: string;
  isDefault: boolean;
}

export interface ValidateAddressRequest {
  addressId: string;
}
