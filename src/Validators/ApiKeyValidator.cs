using System;
using System.Text.RegularExpressions;
using PIIMask.App.Engine;

namespace PIIMask.App.Validators
{
    /// <summary>
    /// Geliştirici Sırları, API Anahtarları, Token'lar, Şifreler ve Kriptografik Anahtarlar Modülü
    /// </summary>
    public static class ApiKeyValidator
    {
        // 1. Kriptografik Anahtarlar & Hizmet Hesapları
        private static readonly Regex PemKeyRegex = new Regex(@"-----BEGIN (?:RSA |EC |DSA |OPENSSH )?PRIVATE KEY-----[\s\S]*?-----END (?:RSA |EC |DSA |OPENSSH )?PRIVATE KEY-----", RegexOptions.Compiled);
        private static readonly Regex GcpSaRegex = new Regex(@"\{[^{}]*?""type""\s*:\s*""service_account""[^{}]*?\}", RegexOptions.Compiled);

        // 2. AI & LLM Sağlayıcıları
        private static readonly Regex OpenAiRegex = new Regex(@"\bsk-(?:proj|admin|svcacct)?[a-zA-Z0-9_-]{32,164}\b", RegexOptions.Compiled);
        private static readonly Regex AnthropicRegex = new Regex(@"\bsk-ant-(?:api\d{2}|admin\d{2})?-[a-zA-Z0-9_-]{32,100}\b", RegexOptions.Compiled);
        private static readonly Regex GeminiRegex = new Regex(@"\bAIzaSy[a-zA-Z0-9_-]{33}\b", RegexOptions.Compiled);
        private static readonly Regex DeepSeekRegex = new Regex(@"\bsk-[a-f0-9]{32}\b", RegexOptions.Compiled);
        private static readonly Regex HuggingFaceRegex = new Regex(@"\bhf_[a-zA-Z0-9]{34}\b", RegexOptions.Compiled);

        // 3. Bulut Altyapı Sağlayıcıları
        private static readonly Regex AwsAccessRegex = new Regex(@"\b(AKIA|ASIA)[0-9A-Z]{16}\b", RegexOptions.Compiled);
        private static readonly Regex AwsSecretRegex = new Regex(@"(?i)(aws_secret_access_key|aws_secret_key)\s*[:=]\s*[""']?([0-9a-zA-Z/+=]{40})[""']?", RegexOptions.Compiled);

        // 4. Git & Paket Depoları
        private static readonly Regex GitHubRegex = new Regex(@"\b(?:ghp|gho|ghu|ghs|ghr)_[a-zA-Z0-9]{36,255}\b|\bgithub_pat_[a-zA-Z0-9_]{82}\b", RegexOptions.Compiled);
        private static readonly Regex GitLabRegex = new Regex(@"\bglpat-[a-zA-Z0-9_-]{20,}\b", RegexOptions.Compiled);
        private static readonly Regex NpmRegex = new Regex(@"\bnpm_[a-zA-Z0-9]{36}\b", RegexOptions.Compiled);
        private static readonly Regex DockerRegex = new Regex(@"\bdckr_pat_[a-zA-Z0-9_-]{27,}\b", RegexOptions.Compiled);

        // 5. Ödeme ve Servis Token'ları (Stripe Keys & Customer/Sub/Card ID'leri, Slack, Discord, Telegram, Twilio, SendGrid)
        private static readonly Regex StripeKeyRegex = new Regex(@"\b(?:sk|rk|pk)_(?:live|test)_[0-9a-zA-Z]{24,99}\b", RegexOptions.Compiled);
        private static readonly Regex StripeIdRegex = new Regex(@"\b(?:cus|sub|card|tok|src|ba|ch|pi|in|txn|evt|pm|setup|seti|prod|price|plan|fee|re|iss|cl)_(?:live|test)?[0-9a-zA-Z]{14,99}\b", RegexOptions.Compiled);
        private static readonly Regex SlackRegex = new Regex(@"\bxox[baprs]-[0-9]{10,13}-[0-9]{10,13}[a-zA-Z0-9-]*\b", RegexOptions.Compiled);
        private static readonly Regex DiscordRegex = new Regex(@"\b[MNO][a-zA-Z0-9_-]{23,25}\.[a-zA-Z0-9_-]{6}\.[a-zA-Z0-9_-]{27,38}\b", RegexOptions.Compiled);
        private static readonly Regex TelegramRegex = new Regex(@"\b\d{8,10}:[a-zA-Z0-9_-]{35}\b", RegexOptions.Compiled);
        private static readonly Regex TwilioRegex = new Regex(@"\b(?:SK|AC)[a-f0-9]{32}\b", RegexOptions.Compiled);
        private static readonly Regex SendGridRegex = new Regex(@"\bSG\.[a-zA-Z0-9_-]{22}\.[a-zA-Z0-9_-]{43}\b", RegexOptions.Compiled);

        // 6. JWT & Refresh Token'lar
        private static readonly Regex JwtRegex = new Regex(@"\beyJ[a-zA-Z0-9_-]+\.eyJ[a-zA-Z0-9_-]+\.[a-zA-Z0-9_-]+\b", RegexOptions.Compiled);
        private static readonly Regex RefreshTokenPrefixRegex = new Regex(@"\brt_(?:live|test)_[0-9a-zA-Z]{16,99}\b", RegexOptions.Compiled);

        // 7. HTTP Cookie / Header / Query Parametre Token Değerleri (refresh_token=..., access_token=...)
        private static readonly Regex CookieOrHeaderTokenRegex = new Regex(@"(?i)\b(refresh_token|access_token|session_token|auth_token|bearer|token|session|jwt|api_key)=([^;\s&""']+)", RegexOptions.Compiled);

        // 8. Kod İçi & JSON İçi Şifre / Parola Alanları ("password": "...", password = "...")
        private static readonly Regex PasswordFieldRegex = new Regex(@"(?i)([""']?(?:password|passwd|pwd|secret|api_key|apikey|client_secret|private_key|sifre|parola)[""']?\s*[:=]\s*[""'])([^""'\r\n]+)([""'])", RegexOptions.Compiled);

        // 9. OTP / TOTP / 2FA Doğrulama Kodları ("totp_code": "481920", otp: 123456)
        private static readonly Regex OtpFieldRegex = new Regex(@"(?i)([""']?(?:totp|totp_code|otp|otp_code|2fa|2fa_code|verification_code|pin|sms_code)[""']?\s*[:=]\s*[""']?)(\d{4,8})([""']?)", RegexOptions.Compiled);

        /// <summary>
        /// Metindeki tüm API anahtarlarını, token'ları, şifreleri ve sırları maskeler
        /// </summary>
        public static string Mask(string text, MaskContext ctx)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string result = text;

            // 1. PEM Private Keys & GCP SA
            result = PemKeyRegex.Replace(result, delegate(Match m) { return "[PRIVATE_KEY_" + ctx.Next() + "]"; });
            result = GcpSaRegex.Replace(result, delegate(Match m) { return "[GCP_SERVICE_ACCOUNT_" + ctx.Next() + "]"; });

            // 2. JWT & Refresh Token
            result = JwtRegex.Replace(result, delegate(Match m) { return "[JWT_TOKEN_" + ctx.Next() + "]"; });
            result = RefreshTokenPrefixRegex.Replace(result, delegate(Match m) { return "[REFRESH_TOKEN_" + ctx.Next() + "]"; });

            // 3. AI & LLM Providers
            result = OpenAiRegex.Replace(result, delegate(Match m) { return "[API_KEY_OPENAI_" + ctx.Next() + "]"; });
            result = AnthropicRegex.Replace(result, delegate(Match m) { return "[API_KEY_ANTHROPIC_" + ctx.Next() + "]"; });
            result = GeminiRegex.Replace(result, delegate(Match m) { return "[API_KEY_GEMINI_" + ctx.Next() + "]"; });
            result = DeepSeekRegex.Replace(result, delegate(Match m) { return "[API_KEY_DEEPSEEK_" + ctx.Next() + "]"; });
            result = HuggingFaceRegex.Replace(result, delegate(Match m) { return "[API_KEY_HUGGINGFACE_" + ctx.Next() + "]"; });

            // 4. Cloud & Git Providers
            result = AwsAccessRegex.Replace(result, delegate(Match m) { return "[AWS_ACCESS_KEY_" + ctx.Next() + "]"; });
            result = AwsSecretRegex.Replace(result, delegate(Match m) { return m.Groups[1].Value + " = \"[AWS_SECRET_KEY_" + ctx.Next() + "]\""; });
            result = GitHubRegex.Replace(result, delegate(Match m) { return "[GITHUB_TOKEN_" + ctx.Next() + "]"; });
            result = GitLabRegex.Replace(result, delegate(Match m) { return "[GITLAB_TOKEN_" + ctx.Next() + "]"; });
            result = NpmRegex.Replace(result, delegate(Match m) { return "[NPM_TOKEN_" + ctx.Next() + "]"; });
            result = DockerRegex.Replace(result, delegate(Match m) { return "[DOCKER_TOKEN_" + ctx.Next() + "]"; });

            // 5. Payment & Service IDs (Stripe Keys & IDs, Slack, Discord, Telegram, Twilio, SendGrid)
            result = StripeKeyRegex.Replace(result, delegate(Match m) { return "[STRIPE_KEY_" + ctx.Next() + "]"; });
            result = StripeIdRegex.Replace(result, delegate(Match m) { return "[STRIPE_ID_" + ctx.Next() + "]"; });
            result = SlackRegex.Replace(result, delegate(Match m) { return "[SLACK_TOKEN_" + ctx.Next() + "]"; });
            result = DiscordRegex.Replace(result, delegate(Match m) { return "[DISCORD_TOKEN_" + ctx.Next() + "]"; });
            result = TelegramRegex.Replace(result, delegate(Match m) { return "[TELEGRAM_BOT_TOKEN_" + ctx.Next() + "]"; });
            result = TwilioRegex.Replace(result, delegate(Match m) { return "[TWILIO_KEY_" + ctx.Next() + "]"; });
            result = SendGridRegex.Replace(result, delegate(Match m) { return "[SENDGRID_KEY_" + ctx.Next() + "]"; });

            // 6. HTTP Cookie / Header Tokens
            result = CookieOrHeaderTokenRegex.Replace(result, delegate(Match m)
            {
                return m.Groups[1].Value + "=[TOKEN_" + ctx.Next() + "]";
            });

            // 7. Password / Secret Fields (JSON & Code uyumlu)
            result = PasswordFieldRegex.Replace(result, delegate(Match m)
            {
                return m.Groups[1].Value + "[SECRET_" + ctx.Next() + "]" + m.Groups[3].Value;
            });

            // 8. OTP / TOTP / 2FA Codes
            result = OtpFieldRegex.Replace(result, delegate(Match m)
            {
                return m.Groups[1].Value + "[OTP_KODU_" + ctx.Next() + "]" + m.Groups[3].Value;
            });

            return result;
        }
    }
}
