using FloristAI.Core.Entities;

namespace FloristAI.Application.Boutique
{
    public interface IShopRepository
    {
        Task<bool> AddShops(List<Shop> shop);
    }
}
