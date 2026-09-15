using System;
using System.Text.RegularExpressions;
using PIIMask.App.Engine;

namespace PIIMask.App.Validators
{
    /// <summary>
    /// Veritabanı Bağlantı URL'leri, Ağ IP Adresleri, MAC ve UUID Modülü
    /// </summary>
    public static class NetworkValidator
    {
        private static readonly Regex DbUrlRegex = new Regex(@"\b(?:postgresql|postgres|mysql|mongodb(?:\+srv)?|redis|mssql):\/\/[^\s""'<>\)]+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex Ipv4Regex = new Regex(@"\b(?:(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\b", RegexOptions.Compiled);
        private static readonly Regex Ipv6Regex = new Regex(@"\b(?:[0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}\b", RegexOptions.Compiled);
        private static readonly Regex MacRegex = new Regex(@"\b(?:[0-9A-Fa-f]{2}[:-]){5}(?:[0-9A-Fa-f]{2})\b", RegexOptions.Compiled);
        private static readonly Regex UuidRegex = new Regex(@"\b[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}\b", RegexOptions.Compiled);

        /// <summary>
        /// Database URL'lerini maskeler
        /// </summary>
        public static string MaskDatabaseUrls(string text, MaskContext ctx)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return DbUrlRegex.Replace(text, delegate(Match m) { return "[DATABASE_URL_" + ctx.Next() + "]"; });
        }

        /// <summary>
        /// IP Adresleri, MAC ve UUID'leri maskeler
        /// </summary>
        public static string MaskNetworkAndIds(string text, MaskContext ctx)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string result = text;

            result = Ipv4Regex.Replace(result, delegate(Match m) { return "[IP_ADRESI_" + ctx.Next() + "]"; });
            result = Ipv6Regex.Replace(result, delegate(Match m) { return "[IPV6_ADRESI_" + ctx.Next() + "]"; });
            result = MacRegex.Replace(result, delegate(Match m) { return "[MAC_ADRESI_" + ctx.Next() + "]"; });
            result = UuidRegex.Replace(result, delegate(Match m) { return "[UUID_" + ctx.Next() + "]"; });

            return result;
        }
    }
}
