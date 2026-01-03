export interface CreateOrderItemDto {
  productId: number
  quantity: number
}

export interface CreateOrderDto {
  items: CreateOrderItemDto[]
  paymentMode: 'PayNow' | 'PayLater'
  customerId?: string
}

export interface UpdateOrderItemDto {
  productId: number
  quantity: number
}

export interface UpdateOrderDto {
  items?: UpdateOrderItemDto[]
  paymentMode?: 'PayNow' | 'PayLater'
}

export interface OrderItemDto {
  productId: number
  productName: string
  unitPrice: number
  quantity: number
}

export interface OrderListDto {
  orderId: number
  orderDate: string
  status: string
  paymentStatus: string
  totalAmount: number
  items: OrderItemDto[]
}

export interface OrderQueryDto {
  page: number
  pageSize: number
  status?: string
}
