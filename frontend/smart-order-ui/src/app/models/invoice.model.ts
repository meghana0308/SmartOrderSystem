export interface InvoiceListDto {
  invoiceId: number;
  orderId: number;
  invoiceDate: string; 
  totalAmount: number;
  paymentStatus: string;
}

export interface InvoiceItemDto {
  productName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface InvoiceDetailDto {
  invoiceId: number;
  orderId: number;
  invoiceDate: string;
  totalAmount: number;
  paymentStatus: string;
  items: InvoiceItemDto[];
}

export interface FinanceOrderDto {
  orderId: number;
  orderDate: string;
  orderStatus: string;
  paymentMode: string;
  paymentStatus: string;
  totalAmount: number;
  invoiceId?: number;
}
