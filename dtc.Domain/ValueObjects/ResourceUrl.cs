using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtc.Domain.ValueObjects
{
    public class ResourceUrl : ValueObject
    {
        public string Value { get; }

        private ResourceUrl(string value)
        {
            Value = value;
        }

        public static ResourceUrl Create(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("ResourceUrl is required");

            return new ResourceUrl(url.Trim());
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            throw new NotImplementedException();
        }
    }

}
