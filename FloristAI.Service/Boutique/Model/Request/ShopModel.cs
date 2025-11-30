
namespace FloristAI.Application.Boutique.Model.Request
{
    public class ShopModel
    {
        public int Id { get; set; }

        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Географическая широта магазина.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Географическая долгота магазина.
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// Ссылка координаты в Google Maps
        /// </summary>
        public string UrlGoogleMap { get; set; } = string.Empty;

        public string LanguageCode { get; set; }
    }
}
