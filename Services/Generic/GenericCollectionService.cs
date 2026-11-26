using TA_sem3.Models;

namespace TA_sem3.Services.Generic
{
    // Hasil pagination generic
    public class PageResult<T>
    {
        public List<T> Items { get; set; } = new();

        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling(
                (double)TotalItems / PageSize);
    }

    // Generic service
    public class GenericCollectionService<T>
    {
        private readonly IEnumerable<T> data;

        public GenericCollectionService(IEnumerable<T> items)
        {
            data = items;
        }

        // Mengambil semua data
        public IEnumerable<T> GetAll()
        {
            return data;
        }

        // Filter berdasarkan kondisi
        public IEnumerable<T> Filter(
            Func<T, bool> predicate)
        {
            return data.Where(predicate);
        }

        // Pagination
        public PageResult<T> GetPage(
            int page,
            int pageSize)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            var items = data.ToList();

            return new PageResult<T>
            {
                Items = items
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(),

                TotalItems = items.Count,
                CurrentPage = page,
                PageSize = pageSize
            };
        }
    }
}
