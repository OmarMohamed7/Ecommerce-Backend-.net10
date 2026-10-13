using AutoMapper;
using E_Commerce.Common;
using E_Commerce.Db;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Features.Cart;

public class CartService(AppDbContext db, IMapper mapper) : ICartService
{
    public async Task<CartDto> GetCartAsync(int userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        return mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto)
    {
        var productExists = await db.Products.AnyAsync(x => x.Id == userId);
        if (!productExists)
            throw new KeyNotFoundException("Product not found");

        var cart = await GetOrCreateCartAsync(userId);

        var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == dto.ProductId);

        if (existingItem != null)
            existingItem.Quantity += dto.Quantity;
        else
            cart.Items.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                }
            );

        await db.SaveChangesAsync();
        return mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto?> UpdateCartItemAsync(int userId, int productId, UpdateCartItemDto updateCartItemDto)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem == null) return null;

        existingItem.Quantity = updateCartItemDto.Quantity;
        await db.SaveChangesAsync();
        return mapper.Map<CartDto>(cart);
    }

    public async Task<bool> RemoveFromCartAsync(int userId, int productId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem == null) return false;
        cart.Items.Remove(existingItem);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<PagedResult<Cart>> GetAllCarts(CartQueryParams queryParams)
    {
        var carts = await db.Cart
            .AsNoTracking()
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .OrderBy(x => x.UserId)
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        var totalCount = await db.Cart.CountAsync();

        return new PagedResult<Cart>(carts, totalCount, queryParams.Page, queryParams.PageSize);
        ;
    }


    private async Task<Cart> GetOrCreateCartAsync(int userId)
    {
        var cart = await db.Cart
            .Include(c => c.Items)
            .ThenInclude(p => p.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart != null) return cart;

        // New Cart
        cart = new Cart
        {
            UserId = userId
        };

        db.Cart.Add(cart);
        await db.SaveChangesAsync();
        return cart;
    }
}