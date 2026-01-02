export interface CreateOrderItemDto {
  productId: number
  quantity: number
}

export interface CreateOrderDto {
  items: CreateOrderItemDto[]
  paymentMode: 'PayNow' | 'PayLater'
  customerId?: string
}

export interface OrderItemDto {
  productId: number
  name: string
  unitPrice: number
  quantity: number
}

export interface OrderListDto {
  orderId: number
  orderDate: string
  status: string
  totalAmount: number
  items?: OrderItemDto[] // include products in each order
}

export interface OrderQueryDto {
  page?: number
  pageSize?: number
  status?: string
}
