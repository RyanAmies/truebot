namespace TRUEbot.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength, bool withEllipsis = false)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength
                ? value : withEllipsis ? value.Substring(0, maxLength - 3) + "..." : value.Substring(0, maxLength);
        }

        public static string Normalise(this string text)
        {
            return text.UnifyApostrophe().ToUpper();
        }

        public static string UnifyApostrophe(this string text)
        {
            return text.Replace("‘", "'").Replace("’", "'").Replace("`", "'").Replace("'", "'");
        }
    }
}

