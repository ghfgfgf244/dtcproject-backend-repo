using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.Entities.Location
{
    public class Address
    {
        public int Id { get; private set; }
        public string AddressName { get; private set; } = default!;

        protected Address() { }

        public Address(int id, string addressName)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0");

            Id = id;
            SetAddressName(addressName);
        }

        public Address(string addressName)
        {
            SetAddressName(addressName);
        }

        // =========================
        // BEHAVIORS
        // =========================

        public bool UpdateAddressName(string? newAddressName)
        {
            if (string.IsNullOrWhiteSpace(newAddressName))
                return false;

            var normalized = newAddressName.Trim();
            if (AddressName == normalized)
                return false;

            AddressName = normalized;
            return true;
        }

        // =========================
        // PRIVATE RULES
        // =========================

        private void SetAddressName(string addressName)
        {
            if (string.IsNullOrWhiteSpace(addressName))
                throw new ArgumentException("AddressName is required");

            AddressName = addressName.Trim();
        }
    }
}
