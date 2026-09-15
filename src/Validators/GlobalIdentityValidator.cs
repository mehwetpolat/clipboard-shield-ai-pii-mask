using System;
using System.Text.RegularExpressions;
using PIIMask.App.Engine;

namespace PIIMask.App.Validators
{
    /// <summary>
    /// Ulusal Kimlikler ve Resmi Belgeler Modülü (VKN, US SSN, US EIN, UK NINO, EU VAT, Pasaport)
    /// </summary>
    public static class GlobalIdentityValidator
    {
        private static readonly Regex VknRegex = new Regex(@"\b\d{10}\b", RegexOptions.Compiled);
        private static readonly Regex SsnRegex = new Regex(@"\b\d{3}-\d{2}-\d{4}\b", RegexOptions.Compiled);
        private static readonly Regex EinRegex = new Regex(@"\b\d{2}-\d{7}\b", RegexOptions.Compiled);
        private static readonly Regex NinoRegex = new Regex(@"\b[A-CEGHJ-PR-TW-Z]{2}\d{6}[A-D]\b", RegexOptions.Compiled);
        private static readonly Regex EuVatRegex = new Regex(@"\b(?:ATU\d{8}|BE0\d{9}|DE\d{9}|FR[A-Z0-9]{2}\d{9}|GB\d{9}|IT\d{11}|NL\d{9}B\d{2}|ES[A-Z0-9]\d{7}[A-Z0-9])\b", RegexOptions.Compiled);

        /// <summary>
        /// 10 Haneli Vergi Kimlik Numarası (VKN) Doğrulama
        /// </summary>
        public static bool IsValidVKN(string vkn)
        {
            if (string.IsNullOrEmpty(vkn) || vkn.Length != 10) return false;
            int[] d = new int[10];
            for (int i = 0; i < 10; i++)
            {
                if (vkn[i] < '0' || vkn[i] > '9') return false;
                d[i] = vkn[i] - '0';
            }

            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                int c1 = (d[i] + (9 - i)) % 10;
                int c2 = (int)((c1 * Math.Pow(2, 9 - i)) % 9);
                if (c1 != 0 && c2 == 0) c2 = 9;
                sum += c2;
            }

            int lastDigit = (10 - (sum % 10)) % 10;
            return lastDigit == d[9];
        }

        /// <summary>
        /// Metindeki uluslararası kimlikleri ve resmi belgeleri maskeler
        /// </summary>
        public static string Mask(string text, MaskContext ctx)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string result = text;

            // VKN
            result = VknRegex.Replace(result, delegate(Match m)
            {
                if (IsValidVKN(m.Value))
                {
                    return "[VKN_" + ctx.Next() + "]";
                }
                return m.Value;
            });

            // US SSN
            result = SsnRegex.Replace(result, delegate(Match m) { return "[US_SSN_" + ctx.Next() + "]"; });

            // US EIN
            result = EinRegex.Replace(result, delegate(Match m) { return "[US_EIN_" + ctx.Next() + "]"; });

            // UK NINO
            result = NinoRegex.Replace(result, delegate(Match m) { return "[UK_NINO_" + ctx.Next() + "]"; });

            // EU VAT
            result = EuVatRegex.Replace(result, delegate(Match m) { return "[EU_VAT_" + ctx.Next() + "]"; });

            return result;
        }
    }
}
