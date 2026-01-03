export interface OrderListDto {
  orderId:number
  orderDate:string
  status:string
  paymentStatus:string
  totalAmount:number
  items:OrderItemViewDto[]
}

export interface OrderItemViewDto {
  productId:number
  productName:string
  unitPrice:number
  quantity:number
}
