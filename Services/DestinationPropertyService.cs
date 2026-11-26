
using System;
using TA_sem3.Models;

namespace TA_sem3.Services
{
    public class DestinationPropertyService
    {
        // =========================
        // GET METHOD
        // =========================

        public string GetDestinationName(Destination destination)
        {
            return destination.Name;
        }

        public string GetRegion(Destination destination)
        {
            return destination.Region;
        }

        public bool GetStatus(Destination destination)
        {
            return destination.IsActive;
        }

        // =========================
        // SET METHOD
        // =========================

        public void SetDestinationName(
            Destination destination, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Nama destinasi tidak boleh kosong.");
            }

            destination.Name = name.Trim();
        }

        public void SetRegion(
            Destination destination, string region)
        {
            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException(
                    "Wilayah tidak boleh kosong.");
            }

            destination.Region = region.Trim();
        }

        public void SetStatus(
            Destination destination, bool status)
        {
            destination.IsActive = status;
        }
    }
}
