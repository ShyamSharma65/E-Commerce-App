using ECommerceAPI.Models;

namespace ECommerceAPI.DTOs;

/// <summary>Payload for changing an order's status.</summary>
public sealed record UpdateOrderStatusRequestDto(OrderStatus Status);