using E_Commerce.Common;

namespace E_Commerce.Features.Cart;

public interface ICartService
{
    Task<PagedResult<Cart>> GetAllCarts(CartQueryParams queryParams);
    Task<CartDto> GetCartAsync(int userId);

    Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto);

    Task<CartDto?> UpdateCartItemAsync(int userId, int productId, UpdateCartItemDto updateCartItemDto);

    Task<bool> RemoveFromCartAsync(int userId, int productId);
}

public class PageResult<T>
{
}