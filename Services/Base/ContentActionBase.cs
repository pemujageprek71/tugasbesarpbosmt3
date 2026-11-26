using TA_sem3.Models;

namespace TA_sem3.Services.Base
{
    public abstract class ContentActionBase
    {
        // Method yang diwariskan class turunan
        protected bool IsValidKeyword(string keyword)
        {
            return !string.IsNullOrWhiteSpace(keyword);
        }

        // Abstract method wajib diimplementasikan
        public abstract IEnumerable<Destination> Execute(
            IEnumerable<Destination> destinations,
            string keyword);
    }
}
