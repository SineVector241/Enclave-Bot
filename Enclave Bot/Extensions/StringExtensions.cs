namespace Enclave_Bot.Extensions
{
    public static class StringExtensions
    {
        public static string? Truncate(this string? value, int maxLength, string truncationSuffix = "…")
        {
            return value?.Length > maxLength
                ? value.Substring(0, maxLength - truncationSuffix.Length) + truncationSuffix
                : value;
        }
    }
}
