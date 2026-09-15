using System;
using System.Text.RegularExpressions;
using PIIMask.App.Engine;

namespace PIIMask.App.Validators
{
    /// <summary>
    /// T.C. Kimlik Numarası (TCKN) Doğrulama ve Maskeleme Modülü
    /// 11 Basamaklı Çift Sağlama (Dual Checksum) Algoritması
    /// </summary>
    public static class TcknValidator
    {
        private static readonly Regex TcknRegex = new Regex(@"\b\d{11}\b", RegexOptions.Compiled);

        /// <summary>
        /// TCKN'nin matematiksel algoritma kurallarına uygunluğunu doğrular
        /// </summary>
        public static bool IsValid(string tckn)
        {
            if (string.IsNullOrEmpty(tckn) || tckn.Length != 11) return false;
            if (tckn[0] == '0') return false;

            int[] digits = new int[11];
            for (int i = 0; i < 11; i++)
            {
                if (tckn[i] < '0' || tckn[i] > '9') return false;
                digits[i] = tckn[i] - '0';
            }

            // 1, 3, 5, 7, 9. hanelerin toplamının 7 katından 2, 4, 6, 8. hanelerin toplamı çıkarıldığında mod 10 = 10. hane
            int oddSum = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
            int evenSum = digits[1] + digits[3] + digits[5] + digits[7];

            int d10 = ((oddSum * 7) - evenSum) % 10;
            if (d10 < 0) d10 += 10;
            if (d10 != digits[9]) return false;

            // İlk 10 hanenin toplamının mod 10'u = 11. hane
            int sumFirst10 = 0;
            for (int i = 0; i < 10; i++) sumFirst10 += digits[i];
            if ((sumFirst10 % 10) != digits[10]) return false;

            return true;
        }

        /// <summary>
        /// Metin içerisindeki tüm geçerli TCKN'leri tespit edip maskeler
        /// </summary>
        public static string Mask(string text, MaskContext ctx)
        {
            if (string.IsNullOrEmpty(text)) return text;

            return TcknRegex.Replace(text, delegate(Match m)
            {
                if (IsValid(m.Value))
                {
                    return "[TCKN_" + ctx.Next() + "]";
                }
                return m.Value;
            });
        }
    }
}
