
using TA_sem3.Models;

namespace TA_sem3.Services.Filters
{
    public class DestinationFilterDictionary
    {
        private readonly Dictionary<
            string,
            Func<Destination, string, bool>> filters;

        public DestinationFilterDictionary()
        {
            filters = new Dictionary<
                string,
                Func<Destination, string, bool>>
            {
                {
                    "name",
                    (d, value) => d.Name.Contains(
                        value,
                        StringComparison.OrdinalIgnoreCase)
                },
                {
                    "region",
                    (d, value) => d.Region.Equals(
                        value,
                        StringComparison.OrdinalIgnoreCase)
                },
                {
                    "category",
                    (d, value) =>
                        d.DestinationCategories != null &&
                        d.DestinationCategories.Any(dc =>
                            dc.Category != null &&
                            dc.Category.CategoryName.Equals(
                                value,
                                StringComparison.OrdinalIgnoreCase))
                }
            };
        }

        // Mendapatkan semua jenis filter
        public IEnumerable<string> GetFilterTypes()
        {
            return filters.Keys;
        }

        // Menjalankan filter berdasarkan key
        public IEnumerable<Destination> ApplyFilter(
            IEnumerable<Destination> destinations,
            string filterType,
            string value)
        {
            if (!filters.TryGetValue(
                filterType.ToLowerInvariant(),
                out var filter))
            {
                throw new ArgumentException(
                    "Jenis filter tidak tersedia.");
            }

            if (string.IsNullOrWhiteSpace(value))
                return destinations;

            return destinations.Where(d =>
                filter(d, value.Trim()));
        }
    }
}
