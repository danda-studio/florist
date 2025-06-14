namespace FloristAI.Core.Entities
{
    /// <summary>
    /// Модель магазина
    /// </summary>
    public class Shop
    {
        /// <summary>
        /// Уникальный идентификатор магазина.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Адрес магазина.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Географическая широта магазина.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Географическая долгота магазина.
        /// </summary>
        public double Longitude { get; set; }
    }
}
