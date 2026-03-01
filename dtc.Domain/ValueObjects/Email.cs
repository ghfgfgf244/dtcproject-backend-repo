using System.Text.RegularExpressions;

namespace dtc.Domain.ValueObjects
{
    public sealed class Email : ValueObject
    {
        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            var normalized = email.Trim().ToLowerInvariant();

            if (!EmailRegex.IsMatch(normalized))
                throw new ArgumentException("Email format is invalid");

            return new Email(normalized);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
