using FloristAI.Application.Boutique.Model.Request;
using FloristAI.Application.Boutique.Model.Response;

namespace FloristAI.Application.Boutique
{
    public interface IShopService
    {
        Task<bool> AddShops(AddShopsRequest request);
        Task<GetCoordinatesFromAddressResponse> GetCoordinatesFromAddress(string address);
        Task<ValidationAddressResponse> ValidationAddress(ValidationAddressRequest request);
    }
}
