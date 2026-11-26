
using TA_sem3.Models;

namespace TA_sem3.Services.Interfaces
{
    public interface IDestinationAction
    {
        IEnumerable<Destination> Search(
            IEnumerable<Destination> destinations,
            string keyword);

        void SetActive(Destination destination, bool status);
    }
}
