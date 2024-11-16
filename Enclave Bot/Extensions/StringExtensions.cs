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

        public static Int32 ToUnixTimestamp(this DateTime datetime) => (int)datetime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        public static string ToDiscordUnixTimestampFormat(this DateTime datetime) => $"<t:{datetime.ToUnixTimestamp()}:R>";
    }
}
