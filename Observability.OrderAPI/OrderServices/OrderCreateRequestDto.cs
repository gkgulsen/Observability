using Observability.CommonShared.DTOs;

namespace Observability.OrderAPI.OrderServices
{
    public record class OrderCreateRequestDto
    {
        public int UserId { get; set; }
        public List<OrderItemDto> Items { get; set; } = null!;
    }

    
}
