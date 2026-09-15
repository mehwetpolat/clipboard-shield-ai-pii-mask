using System;
using PIIMask.App.Validators;

namespace PIIMask.App.Engine
{
    /// <summary>
    /// Tüm Doğrulayıcı Modülleri Yöneten Merkezi Maskeleme Motoru
    /// </summary>
    public static class MaskEngine
    {
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

            // 3. Finansal Bilgiler (Kredi Kartı Luhn, IBAN Mod 97-10, CVV, ABA Routing)
            if (config.CreditCard)
            {
                result = CreditCardValidator.Mask(result, ctx);
            }

            // 4. Ulusal Kimlikler (TCKN Çift Sağlama, VKN, SSN, EIN, NINO, EU VAT)
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
                result = NameValidator.Mask(result, ctx);
            }

            return result;
        }
    }
}
