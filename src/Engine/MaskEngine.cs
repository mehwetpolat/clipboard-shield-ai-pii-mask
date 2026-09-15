using System;
using System.Text.RegularExpressions;
using PIIMask.App.Validators;

namespace PIIMask.App.Engine
{
    /// <summary>
    /// Tüm Doğrulayıcı Modülleri Yöneten Merkezi Maskeleme Motoru
    /// </summary>
    public static class MaskEngine
    {
        // Unvanlı İsimler (Sayın Ahmet Yılmaz, Dr. Mehmet Öz, vb.)
        private static readonly Regex TitleNameRegex = new Regex(@"\b(?:Sayın|Sn\.|Dr\.|Av\.|Müh\.|Prof\.)\s+([A-ZÇĞİÖŞÜ][a-zçğıöşü]+(?:\s+[A-ZÇĞİÖŞÜ][a-zçğıöşü]+)+)\b", RegexOptions.Compiled);

        // JSON & Kod İçi İsim Alanları ("full_name": "Mehmet Polat", "customer_name": "...", vb.)
        private static readonly Regex JsonNameRegex = new Regex(@"(?i)([""']?(?:full_name|fullname|first_name|firstname|last_name|lastname|customer_name|user_name|username|author|owner|name|ad_soyad|adi_soyadi|isim|soyisim|kullanici_adi)[""']?\s*[:=]\s*[""'])([A-ZÇĞİÖŞÜa-zçğıöşü\s]{2,40})([""'])", RegexOptions.Compiled);

        /// <summary>
        /// Gelen metni aktif filtre yapılandırmasına göre modüler doğrulayıcılardan geçirir
        /// </summary>
        public static string ProcessText(string text, FilterConfig config)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string result = text;
            MaskContext ctx = new MaskContext();

            // 1. Veritabanı URL'leri
            if (config.DatabaseURL)
            {
                result = NetworkValidator.MaskDatabaseUrls(result, ctx);
            }

            // 2. API Anahtarları, Geliştirici Sırları, Token'lar, Şifreler ve OTP Kodları
            if (config.APIKey)
            {
                result = ApiKeyValidator.Mask(result, ctx);
            }

            // 3. Finansal Bilgiler (Kredi Kartı Luhn, IBAN Mod 97-10, ABA Routing)
            if (config.CreditCard)
            {
                result = CreditCardValidator.Mask(result, ctx);
            }

            // 4. Ulusal Kimlikler (TCKN Çift Sağlama)
            if (config.TCKN)
            {
                result = TcknValidator.Mask(result, ctx);
                result = GlobalIdentityValidator.Mask(result, ctx);
            }

            // 5. E-Posta Adresleri
            if (config.Email)
            {
                result = EmailValidator.Mask(result, ctx);
            }

            // 6. Telefon Numaraları
            if (config.Phone)
            {
                result = PhoneValidator.Mask(result, ctx);
            }

            // 7. Ağ Bilgileri (IP Adresi, MAC, UUID)
            if (config.IPAddress)
            {
                result = NetworkValidator.MaskNetworkAndIds(result, ctx);
            }

            // 8. İsim ve Soyisim (JSON / Key-Value ve Unvanlı Kişiler)
            if (config.UserName)
            {
                // JSON & Key-value isimler
                result = JsonNameRegex.Replace(result, delegate(Match m)
                {
                    return m.Groups[1].Value + "[KULLANICI_" + ctx.Next() + "]" + m.Groups[3].Value;
                });

                // Unvanlı İsimler
                result = TitleNameRegex.Replace(result, delegate(Match m)
                {
                    return "[KULLANICI_" + ctx.Next() + "]";
                });
            }

            return result;
        }
    }
}
