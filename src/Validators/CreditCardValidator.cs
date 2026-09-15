using System;
using System.Text;
using System.Text.RegularExpressions;
using PIIMask.App.Engine;

namespace PIIMask.App.Validators
{
    /// <summary>
    /// Kredi Kartı, Banka Kartı (Luhn Modulo 10), IBAN (Mod 97-10), CVV/CVC ve Bankacılık Kodları Modülü
    /// </summary>
    public static class CreditCardValidator
    {
        private static readonly Regex CardRegex = new Regex(@"\b(?:\d{4}[ -]?){3}\d{4}\b|\b\d{13,19}\b", RegexOptions.Compiled);
        private static readonly Regex IbanRegex = new Regex(@"\b[A-Z]{2}\d{2}[A-Z0-9\s]{12,32}\b", RegexOptions.Compiled);
        private static readonly Regex AbaRegex = new Regex(@"\b\d{9}\b", RegexOptions.Compiled);

        // CVV / CVC Güvenlik Kodları ("cvv": "382", cvv = 123, cvc: "456", vb.)
        private static readonly Regex CvvRegex = new Regex(@"(?i)([""']?(?:cvv|cvc|cvv2|cvc2|security_code|guvenlik_kodu|card_cvv)[""']?\s*[:=]\s*[""']?)(\d{3,4})([""']?)", RegexOptions.Compiled);

        // Kart Son Kullanma Tarihi ("expiry": "12/28", "exp_date": "05/2027", vb.)
        private static readonly Regex ExpiryRegex = new Regex(@"(?i)([""']?(?:exp_date|expiry|expiration_date|exp_month_year|son_kullanma)[""']?\s*[:=]\s*[""'])([0-1]?\d[\/-]\d{2,4})([""'])", RegexOptions.Compiled);

        /// <summary>
        /// Luhn (Modulo 10) Algoritması ile Kredi/Banka Kartı Doğrulama
        /// </summary>
        public static bool IsValidLuhn(string number)
        {
            string clean = Regex.Replace(number, @"[\s-]", "");
            if (clean.Length < 13 || clean.Length > 19) return false;

            int sum = 0;
            bool alternate = false;
            for (int i = clean.Length - 1; i >= 0; i--)
            {
                char c = clean[i];
                if (c < '0' || c > '9') return false;
                int n = c - '0';

                if (alternate)
                {
                    n *= 2;
                    if (n > 9) n = (n % 10) + 1;
                }

                sum += n;
                alternate = !alternate;
            }

            return (sum % 10 == 0);
        }

        /// <summary>
        /// ISO 7064 Modulo 97-10 Algoritması ile IBAN Doğrulama
        /// </summary>
        public static bool IsValidIBAN(string iban)
        {
            string clean = Regex.Replace(iban.ToUpper(), @"[\s-]", "");
            if (clean.Length < 15 || clean.Length > 34) return false;

            string rearranged = clean.Substring(4) + clean.Substring(0, 4);
            StringBuilder numStr = new StringBuilder();
            foreach (char c in rearranged)
            {
                if (char.IsDigit(c)) numStr.Append(c);
                else if (c >= 'A' && c <= 'Z') numStr.Append((c - 'A' + 10).ToString());
                else return false;
            }

            int remainder = 0;
            string s = numStr.ToString();
            for (int i = 0; i < s.Length; i++)
            {
                remainder = (remainder * 10 + (s[i] - '0')) % 97;
            }

            return remainder == 1;
        }

        /// <summary>
        /// ABD ABA Routing Transit Numarası (Ağırlıklı Checksum) Doğrulama
        /// </summary>
        public static bool IsValidABA(string aba)
        {
            if (aba == null || aba.Length != 9) return false;
            int[] d = new int[9];
            for (int i = 0; i < 9; i++)
            {
                if (!char.IsDigit(aba[i])) return false;
                d[i] = aba[i] - '0';
            }
            int sum = 3 * (d[0] + d[3] + d[6]) + 7 * (d[1] + d[4] + d[7]) + 1 * (d[2] + d[5] + d[8]);
            return (sum % 10 == 0);
        }

        /// <summary>
        /// Metin içerisindeki tüm geçerli IBAN, Kredi Kartı, CVV ve ABA numaralarını maskeler
        /// </summary>
        public static string Mask(string text, MaskContext ctx)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string result = text;

            // 1. CVV / CVC Güvenlik Kodları
            result = CvvRegex.Replace(result, delegate(Match m)
            {
                return m.Groups[1].Value + "[CVV_" + ctx.Next() + "]" + m.Groups[3].Value;
            });

            // 2. Kart Son Kullanma Tarihi
            result = ExpiryRegex.Replace(result, delegate(Match m)
            {
                return m.Groups[1].Value + "[SKT_" + ctx.Next() + "]" + m.Groups[3].Value;
            });

            // 3. IBAN
            result = IbanRegex.Replace(result, delegate(Match m)
            {
                if (IsValidIBAN(m.Value))
                {
                    return "[IBAN_" + ctx.Next() + "]";
                }
                return m.Value;
            });

            // 4. Kredi Kartı (PAN)
            result = CardRegex.Replace(result, delegate(Match m)
            {
                if (IsValidLuhn(m.Value))
                {
                    return "[KREDI_KARTI_" + ctx.Next() + "]";
                }
                return m.Value;
            });

            // 5. ABA Routing
            result = AbaRegex.Replace(result, delegate(Match m)
            {
                if (IsValidABA(m.Value))
                {
                    return "[ABA_ROUTING_" + ctx.Next() + "]";
                }
                return m.Value;
            });

            return result;
        }
    }
}
