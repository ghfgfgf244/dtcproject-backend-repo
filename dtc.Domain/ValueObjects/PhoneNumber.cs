using System.Text.RegularExpressions;

namespace dtc.Domain.ValueObjects
{
    public sealed class PhoneNumber : ValueObject
    {
        private static readonly Regex PhoneRegex =
            new(@"^\+?[0-9]{9,15}$", RegexOptions.Compiled);

        public string Value { get; }

        private PhoneNumber(string value)
        {
            Value = value;
        }

        public static PhoneNumber Create(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone is required");

            var normalized = phone.Trim();

            if (!PhoneRegex.IsMatch(normalized))
                throw new ArgumentException("Phone number is invalid");

            return new PhoneNumber(normalized);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
