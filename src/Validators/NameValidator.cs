using System;
using System.Text.RegularExpressions;
using PIIMask.App.Engine;

namespace PIIMask.App.Validators
{
    /// <summary>
    /// Kişi İsim ve Soyisimleri (NER & JSON Alanları) Doğrulama ve Maskeleme Modülü
    /// </summary>
    public static class NameValidator
    {
        // Unvanlı İsim Kalıpları (Sayın Ahmet Yılmaz, Dr. Mehmet Öz, Sn. Ali Vural, vb.)
        private static readonly Regex TitleNameRegex = new Regex(@"\b(?:Sayın|Sn\.|Dr\.|Av\.|Müh\.|Prof\.)\s+([A-ZÇĞİÖŞÜ][a-zçğıöşü]+(?:\s+[A-ZÇĞİÖŞÜ][a-zçğıöşü]+)+)\b", RegexOptions.Compiled);

        // JSON & Kod İçi İsim Alanları ("full_name": "Mehmet Polat", "customer_name": "...", vb.)
        private static readonly Regex JsonNameRegex = new Regex(@"(?i)([""']?(?:full_name|fullname|first_name|firstname|last_name|lastname|customer_name|user_name|username|author|owner|name|ad_soyad|adi_soyadi|isim|soyisim|kullanici_adi)[""']?\s*[:=]\s*[""'])([A-ZÇĞİÖŞÜa-zçğıöşü\s]{2,40})([""'])", RegexOptions.Compiled);

        /// <summary>
        /// Metin içerisindeki isim ve soyisimleri maskeler
        /// </summary>
        public static string Mask(string text, MaskContext ctx)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string result = text;

            // 1. JSON & Key-value İsim Alanları
            result = JsonNameRegex.Replace(result, delegate(Match m)
            {
                return m.Groups[1].Value + "[KULLANICI_" + ctx.Next() + "]" + m.Groups[3].Value;
            });

            // 2. Unvanlı Kişi İsimleri
            result = TitleNameRegex.Replace(result, delegate(Match m)
            {
                return "[KULLANICI_" + ctx.Next() + "]";
            });

            return result;
        }
    }
}
