using System;
using System.Text.RegularExpressions;
using PIIMask.App.Engine;

namespace PIIMask.App.Validators
{
    /// <summary>
    /// Telefon Numaraları Doğrulama ve Maskeleme Modülü (TR Yerel GSM 05xx, +90 ve Uluslararası Formatlar)
    /// </summary>
    public static class PhoneValidator
    {
        private static readonly Regex TrPhoneRegex = new Regex(@"(?:\+?90\s*|\b0)?5\d{2}[\s.-]?\d{3}[\s.-]?\d{2}[\s.-]?\d{2}\b", RegexOptions.Compiled);
        private static readonly Regex NanpPhoneRegex = new Regex(@"\b(?:\+?1[\s.-]?)?\(?[2-9]\d{2}\)?[\s.-]?[2-9]\d{2}[\s.-]?\d{4}\b", RegexOptions.Compiled);

        /// <summary>
        /// Metin içerisindeki tüm telefon numaralarını maskeler
        /// </summary>
        public static string Mask(string text, MaskContext ctx)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string result = text;

            // TR GSM & Sabit
            result = TrPhoneRegex.Replace(result, delegate(Match m)
            {
                return "[TELEFON_" + ctx.Next() + "]";
            });

            // NANP & Uluslararası
            result = NanpPhoneRegex.Replace(result, delegate(Match m)
            {
                return "[TELEFON_" + ctx.Next() + "]";
            });

            return result;
        }
    }
}
