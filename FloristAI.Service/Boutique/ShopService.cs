using AutoMapper;
using FloristAI.Application.Boutique.Model.Request;
using FloristAI.Application.Boutique.Model.Response;
using FloristAI.Application.Language;
using FloristAI.Core.Entities;
using FloristAI.Core.Entities.ReferralsAndPartners;
using System.Text.RegularExpressions;

namespace FloristAI.Application.Boutique
{
    public class ShopService: IShopService
    {
        private readonly IShopRepository _shopRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IMapper _mapper;

        public ShopService(IShopRepository shopRepository, ILocalizationService localizationService, IMapper mapper)
        {
            _shopRepository = shopRepository;
            _localizationService = localizationService;
            _mapper = mapper;
        }

        public async Task<bool> AddShops(AddShopsRequest request)
        {
            var shops  = _mapper.Map<List<Shop>>(request.Shops);
            return await _shopRepository.AddShops(shops);
        }

        public Task<ValidationAddressResponse> ValidationAddress (ValidationAddressRequest request)
        {
            var errors = new List<string>();
            var addressRegex = new Regex(
                @"^ул\.\s.+\sд\.?\s?\d+(\s\d+)?\sп\.?$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase
            );

            if (!addressRegex.IsMatch(request.Address))
            {
                errors.Add(string.Format(_localizationService.GetErrorText("InvalidFormat", request.LanguageCode), request.Name));
            }

            return Task.FromResult(new ValidationAddressResponse
            {
                Errors = errors
            });
        }

        public async Task<GetCoordinatesFromAddressResponse> GetCoordinatesFromAddress(string address)
        {
            try
            {
                using var http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "FloristAI-App");

                var response = await http.GetStringAsync(
                    $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(address)}"
                );

                var data = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(response);

                if (data == null || data.Count == 0)
                    return null;

                double lat = double.Parse(data[0]["lat"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double lon = double.Parse(data[0]["lon"].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                return new GetCoordinatesFromAddressResponse
                {
                    Latitude = lat,
                    Longitude = lon
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
