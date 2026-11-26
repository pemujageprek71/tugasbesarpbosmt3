
using TA_sem3.Models;
using TA_sem3.Services.Base;
using TA_sem3.Services.Interfaces;

namespace TA_sem3.Services
{
    public class DestinationActionService
        : ContentActionBase, IDestinationAction
    {
        // Implementasi abstract method
        public override IEnumerable<Destination> Execute(
            IEnumerable<Destination> destinations,
            string keyword)
        {
            return Search(destinations, keyword);
        }

        // Implementasi interface
        public IEnumerable<Destination> Search(
            IEnumerable<Destination> destinations,
            string keyword)
        {
            if (!IsValidKeyword(keyword))
                return destinations;

            return destinations.Where(d =>
                d.Name.Contains(
                    keyword.Trim(),
                    StringComparison.OrdinalIgnoreCase));
        }

        public void SetActive(
            Destination destination, bool status)
        {
            destination.IsActive = status;
        }
    }
}
