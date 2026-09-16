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
        // Her hane arasına boşluk, tire veya nokta konabilen TR GSM numaraları.
        // Bu sayede 05 55 555 55 55, 0 555 5555555 ve 0 5 5 ... biçimleri de yakalanır.
        private static readonly Regex TrPhoneRegex = new Regex(@"(?<!\d)(?:(?:\+\s*9\s*0|0)\s*)?5(?:[ \t.-]*\d){9}(?!\d)", RegexOptions.Compiled);
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
