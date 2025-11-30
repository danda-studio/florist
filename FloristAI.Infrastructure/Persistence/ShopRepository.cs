using FloristAI.Application.Boutique;
using FloristAI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FloristAI.Infrastructure.Persistence
{
    public class ShopRepository : IShopRepository
    {
        private readonly PostgresDbContext _dbcontext;

        public ShopRepository(PostgresDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> AddShops(List<Shop> shops)
        {
            if (shops == null || shops.Count == 0)
                return false;

            // ключ по координатам (округлённый)
            static string CoordKey(double lat, double lon) =>
                $"{Math.Round(lat, 6):F6}:{Math.Round(lon, 6):F6}";

            // Собираем набор адресов и координат из входного списка (игнорируем пустые)
            var addresses = shops
                .Where(s => !string.IsNullOrWhiteSpace(s.Address))
                .Select(s => s.Address!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var coords = shops
                .Where(s => !(Math.Abs(s.Latitude) < double.Epsilon && Math.Abs(s.Longitude) < double.Epsilon))
                .Select(s => CoordKey(s.Latitude, s.Longitude))
                .Distinct()
                .ToList();

            if (addresses.Count == 0 && coords.Count == 0)
                return false;

            // Один запрос к БД: получаем существующие адреса и координаты
            var existing = await _dbcontext.Shops
                .AsNoTracking()
                .Where(s =>
                    (addresses.Count > 0 && addresses.Contains(s.Address)) ||
                    (coords.Count > 0 && !(Math.Abs(s.Latitude) < double.Epsilon && Math.Abs(s.Longitude) < double.Epsilon)))
                .Select(s => new { s.Address, s.Latitude, s.Longitude })
                .ToListAsync();

            var existingAddressSet = new HashSet<string>(existing
                .Where(e => !string.IsNullOrWhiteSpace(e.Address))
                .Select(e => e.Address!.Trim()), StringComparer.OrdinalIgnoreCase);

            var existingCoordSet = new HashSet<string>(existing
                .Where(e => !(Math.Abs(e.Latitude) < double.Epsilon && Math.Abs(e.Longitude) < double.Epsilon))
                .Select(e => CoordKey(e.Latitude, e.Longitude)));

            // Оставляем только те магазины, которых нет в БД по адресу и по координатам
            var toAdd = shops
                .Where(s =>
                {
                    var hasAddr = !string.IsNullOrWhiteSpace(s.Address) && existingAddressSet.Contains(s.Address!.Trim());
                    var hasCoord = !(Math.Abs(s.Latitude) < double.Epsilon && Math.Abs(s.Longitude) < double.Epsilon) &&
                                   existingCoordSet.Contains(CoordKey(s.Latitude, s.Longitude));
                    return !hasAddr && !hasCoord;
                })
                .ToList();

            if (toAdd.Count == 0)
                return false;

            await _dbcontext.AddRangeAsync(toAdd);
            await _dbcontext.SaveChangesAsync();
            return true;
        }
    }
}
