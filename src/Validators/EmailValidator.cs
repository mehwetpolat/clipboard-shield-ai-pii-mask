using System;
using System.Text.RegularExpressions;
using PIIMask.App.Engine;

namespace PIIMask.App.Validators
{
    /// <summary>
    /// E-Posta Adresleri Doğrulama ve Maskeleme Modülü (RFC 5322 Standardı)
    /// </summary>
    public static class EmailValidator
    {
        private static readonly Regex EmailRegex = new Regex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b", RegexOptions.Compiled);

        /// <summary>
        /// Metin içerisindeki e-posta adreslerini maskeler
        /// </summary>
        public static string Mask(string text, MaskContext ctx)
        {
            if (string.IsNullOrEmpty(text)) return text;

            return EmailRegex.Replace(text, delegate(Match m)
            {
                return "[EPOSTA_" + ctx.Next() + "]";
            });
        }
    }
}
