using SmartOrder.API.Models.DTOs.Orders;

namespace SmartOrder.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(string userId, CreateOrderDto dto);
        Task<List<OrderListDto>> GetMyOrdersAsync(string userId, OrderQueryDto query);
        Task<List<OrderListDto>> GetCreatedOrdersAsync(string userId, OrderQueryDto query);

        Task UpdateOrderAsync(int orderId, string userId, UpdateOrderDto dto);
        Task CancelOrderAsync(int orderId, string userId);


    }
}
