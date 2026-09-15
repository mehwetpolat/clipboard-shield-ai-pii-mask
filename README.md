# 🛡️ PII-Mask: Windows Masaüstü Pano ve Veri Gizlilik Kalkanı

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows%20%7C%20WinForms-0078D6.svg?logo=windows&logoColor=white" alt="Windows">
  <img src="https://img.shields.io/badge/.NET%20Framework-4.0%2B-512BD4.svg?logo=dotnet&logoColor=white" alt=".NET">
  <img src="https://img.shields.io/badge/C%23-Native-239120.svg?logo=csharp&logoColor=white" alt="C# Native">
  <img src="https://img.shields.io/badge/External%20Dependencies-Zero-success.svg" alt="Zero Dependencies">
  <img src="https://img.shields.io/badge/Privacy-100%25%20Offline%20%2F%20Local-blueviolet.svg" alt="100% Local">
  <img src="https://img.shields.io/badge/Compliance-KVKK%20%7C%20GDPR%20%7C%20PCI--DSS-orange.svg" alt="Compliance">
</p>

**PII-Mask**, kullanıcıların panolarına (`Ctrl+C`) kopyaladıkları metinlerdeki kişisel verileri (PII), geliştirici sırlarını (API Key & Secret, token'lar, private key'ler), ulusal kimlik numaralarını ve finansal bilgileri **%100 yerel algoritmalarla** anında tespit eden, doğrulayan ve maskeleyen açık kaynaklı bir **Windows Masaüstü Gizlilik Kalkanı** uygulamasıdır.

Hiçbir harici sunucuya, buluta veya ağa bağlanmaz; tüm işlemler bilgisayarınızın yerel belleğinde (RAM) matematiksel sağlama formülleriyle milisaniyeler içinde çalışır.

---

## 💡 Geliştirici Senaryosu: Yapay Zeka ile Güvenli Kodlama

Yapay zeka araçlarını (**ChatGPT, Claude, Cursor, v0, Bolt**) kullanarak hızlıca uygulama geliştirirken en sık yaptığımız şey bir JSON verisini, API çıktısını veya kod parçasını kopyalayıp yapay zekaya yapıştırmaktır:

> 💬 *"Bu API verisine göre bana şık bir ödeme ve kullanıcı profili sayfası yapsana."*

Ancak kopyaladığımız verinin içinde farkında olmadan **şirketin API anahtarları, gerçek müşteri isimleri, telefonlar, T.C. kimlik numaraları veya kredi kartı bilgileri** olabilir.

**PII-Mask** arka planda açıkken ekstra hiçbir şey yapmanıza gerek kalmaz:
Siz normal şekilde `Ctrl+C` ile kopyalarsınız, PII-Mask sadece gizli/hassas kısımları temizler; tasarımın ve kodun ihtiyaç duyduğu ürünleri, fiyatları ve JSON yapısını aynen korur!

---

### 🔴 1. Kopyaladığınız Ham Veri (`Ctrl+C`):

```json
{
  "cart_total": 2499.50,
  "currency": "TRY",
  "product": "Kablosuz Mekanik Klavye",
  "customer": {
    "full_name": "Mehmet Polat",
    "email": "mehmet.polat@sirket.com.tr",
    "phone": "+90 532 987 65 43",
    "tckn": "10000000146"
  },
  "payment": {
    "card_number": "4539 1488 0343 6467",
    "cvv": "382",
    "totp_code": "481920",
    "stripe_customer_id": "cus_N9f82K1x89LmPq0A"
  },
  "api_token": "sk-proj-99887766554433221100aabbccddeeffgghh"
}
```

---

### 🟢 2. Yapay Zekaya Yapıştırılan Güvenli Veri (`Ctrl+V`):

```json
{
  "cart_total": 2499.50,
  "currency": "TRY",
  "product": "Kablosuz Mekanik Klavye",
  "customer": {
    "full_name": "[KULLANICI_1]",
    "email": "[EPOSTA_2]",
    "phone": "[TELEFON_3]",
    "tckn": "[TCKN_4]"
  },
  "payment": {
    "card_number": "[KREDI_KARTI_5]",
    "cvv": "[CVV_6]",
    "totp_code": "[OTP_KODU_7]",
    "stripe_customer_id": "[STRIPE_ID_8]"
  },
  "api_token": "[API_KEY_OPENAI_9]"
}
```

---

### ✨ Neden Çok Rahat?
* 🚀 **Arayüz ve Kod Yapısı Bozulmaz:** Yapay zeka ürün adını, fiyatını ve sayfanın tasarım mantığını eksiksiz anlar ve doğru kodu yazar.
* 🛡️ **Sıfır Veri Sızıntısı:** Gerçek kimlikler, telefonlar, kartlar ve API anahtarları yapay zeka sunucularına **asla gitmez**.
* ⚡ **Ekstra Çaba Yok:** Siz sadece kopyalar (`Ctrl+C`) ve yapıştırırsınız (`Ctrl+V`), PII-Mask aradaki filtrelemeyi 0 milisaniyede halleder.

---

## 🌟 Temel Özellikler

- **🔒 %100 Çevrimdışı ve Yerel Koruma:** Sıfır ağ iletişimi. Kopyalanan hiçbir veri cihaz dışına çıkmaz.
- **⚡ Canlı Pano Kalkanı (Real-time Clipboard Shield):** Metin kopyalandığında hassas veriler 0 ms gecikmeyle yerelde maskelenir.
- **🧮 50+ Algoritmik Doğrulama Motoru:**
  - **TCKN:** 11 basamaklı çift sağlama formülü.
  - **VKN:** Mod 9 / Mod 10 kurumsal kimlik formülü.
  - **Kredi Kartı (PAN):** Modulo 10 (Luhn) sağlama algoritması.
  - **IBAN:** ISO 7064 Modulo 97-10 sağlama algoritması.
  - **ABA Routing Transit:** 9 basamaklı ağırlıklı sağlama formülü.
  - **US SSN, US EIN, UK NINO, EU VAT:** Uluslararası resmi format kuralları.
  - **API Key & Secret:** OpenAI, Anthropic, Gemini, AWS, GCP, Azure, GitHub, Stripe, Private Keys, JWT vb.
- **🖥️ Kullanıcı Dostu Windows Masaüstü Arayüzü:** Sistem tepsisi (System Tray) entegrasyonu, canlı işlem günlüğü, tek tıkla filtre açma/kapama ve manuel metin test paneli.

---

## 🏗️ Çalışma Prensibi

```
  [Kullanıcı Metin Kopyalar (Ctrl+C)]
                 │
                 ▼
  ┌────────────────────────────────────────────────────────┐
  │ 🛡️ PII-MASK (Windows Masaüstü Kalkanı)                 │
  │                                                        │
  │  1. Pano Olayı Algılanır (300ms ultra hızlı tarama)    │
  │  2. Modüler Doğrulayıcılar & Matematiksel Formüller    │
  │  3. Hassas Veriler [TCKN_1], [API_KEY_1] ile Maskelenir│
  │  4. Pano Güvenli Metin ile Güncellenir                 │
  └────────────────────────────────────────────────────────┘
                 │
                 ▼
  [Kullanıcı Güvenle Yapıştırır (Ctrl+V)]
```

---

## 📊 Desteklenen 50+ Veri Türü ve Algoritma Tablosu

| Kategori | Varlık Türü | Format / Standart | Algoritmik Doğrulama | Maskeleme Formatı |
| :--- | :--- | :--- | :--- | :--- |
| **Ulusal Kimlikler** | T.C. Kimlik No (TCKN) | 11 Basamak, İlk hane != 0 | **TCKN Çift Sağlama Formülü** | `[TCKN_1]` |
| | T.C. Vergi Kimlik (VKN) | 10 Basamak Kurumsal No | **Mod 9 / Mod 10 Ağırlıklı Formül** | `[VKN_1]` |
| | US SSN & EIN | `\d{3}-\d{2}-\d{4}`, `\d{2}-\d{7}` | Grup Kuralları & Aralık Kontrolü | `[US_SSN_1]`, `[US_EIN_1]` |
| | UK NINO | `^[A-CEGHJ-PR-TW-Z]{2}\d{6}[A-D]$` | İngiltere NINO Standartları | `[UK_NINO_1]` |
| | EU VAT | `[A-Z]{2}[0-9A-Za-z]{2,12}` | Ülke Kodları & Vergi Formatı | `[EU_VAT_1]` |
| **Finansal Bilgiler** | Kredi / Banka Kartı (PAN) | Visa, Mastercard, Amex, Troy | **Modulo 10 (Luhn) Algoritması** | `[KREDI_KARTI_1]` |
| | IBAN | `[A-Z]{2}\d{2}[A-Z0-9]{4,30}` | **ISO 7064 Modulo 97-10** | `[IBAN_1]` |
| | ABA Routing Transit | 9 Basamaklı Banka Kodu | **Ağırlıklı Routing Checksum** | `[ABA_ROUTING_1]` |
| **API Key & Secret** | OpenAI Classic & Proje | `sk-[a-zA-Z0-9]{32,51}`, `sk-proj-...` | Prefix & Uzunluk Kontrolü | `[API_KEY_OPENAI_1]` |
| | Anthropic, Gemini, DeepSeek | `sk-ant-...`, `AIzaSy...`, `sk-[hex]` | Prefix & Format Kontrolü | `[API_KEY_ANTHROPIC_1]`, `[API_KEY_GEMINI_1]` |
| | AWS (IAM, STS, Secret) | `AKIA[0-9A-Z]{16}`, `ASIA...` | Format & Base64 Kontrolü | `[AWS_ACCESS_KEY_1]`, `[AWS_SECRET_KEY_1]` |
| | GCP & Azure | Service Account JSON, Azure Key | Yapısal JSON & Base64 | `[GCP_SERVICE_ACCOUNT_1]` |
| | GitHub & GitLab | `ghp_`, `github_pat_`, `glpat-` | Token Format Kontrolü | `[GITHUB_TOKEN_1]`, `[GITLAB_TOKEN_1]` |
| | Stripe, Slack, Discord | `sk_live_`, `xoxb-`, Bot Token | Servis Token Standartları | `[STRIPE_KEY_1]`, `[SLACK_TOKEN_1]` |
| | Private Keys & JWT | `-----BEGIN PRIVATE KEY-----`, JWT | PEM Blok & Base64URL | `[PRIVATE_KEY_1]`, `[JWT_TOKEN_1]` |
| **İletişim & Ağ** | E-Posta Adresleri | RFC 5322 Standardı | Regex & TLD Kontrolü | `[EPOSTA_1]` |
| | Telefon Numaraları | TR Yerel (05xx, +90), NANP (+1) | GSM Operatör Blokları | `[TELEFON_1]` |
| | IP Adresleri | IPv4 (0-255 oktet), IPv6 | Oktet Sınır Kontrolü | `[IP_ADRESI_1]`, `[IPV6_ADRESI_1]` |
| | MAC Adresi & UUID | 6 Çift Hex (`:` veya `-`), UUID v1-v5 | Format Doğrulaması | `[MAC_ADRESI_1]`, `[UUID_1]` |
| **Veritabanı & Sistem**| Connection URI | PostgreSQL, MySQL, Mongo, Redis, MSSQL | URI Şema Doğrulaması | `[DATABASE_URL_1]` |
| | Kod İçi Şifre Atamaları | `password = "..."`, `secret = "..."` | Anahtar Kelime & Değer Kontrolü | `[SECRET_1]` |

---

## 📁 Proje Dizin Yapısı

```
PII-Mask/
├── assets/
│   └── app.ico                       # Yüksek çözünürlüklü uygulama ikonu
├── build.bat                         # Tek tıkla C# derleme betiği
├── README.md                         # Proje dokümantasyonu
├── release/
│   └── PII-Mask.exe                  # Hazır çalıştırılabilir masaüstü uygulaması
└── src/
    ├── DesktopApp.cs                 # WinForms UI, System Tray ve Pano Olay Dinleyicisi
    ├── Engine/                       # Maskeleme ve Durum Motoru
    │   ├── FilterConfig.cs           # Aktif filtre modeli
    │   ├── MaskContext.cs            # Sayaç ve etiketleme bağlamı
    │   └── MaskEngine.cs             # Merkezi filtreleme orkestratörü
    └── Validators/                   # Bağımsız Algoritmik Doğrulayıcılar
        ├── TcknValidator.cs          # TCKN Çift Sağlama formülü
        ├── CreditCardValidator.cs    # Luhn PAN, IBAN Mod 97-10, ABA Routing
        ├── ApiKeyValidator.cs        # 20+ API Key, Token, PEM Key, JWT
        ├── GlobalIdentityValidator.cs# VKN, US SSN, US EIN, UK NINO, EU VAT
        ├── EmailValidator.cs         # RFC 5322 E-Posta
        ├── PhoneValidator.cs         # TR GSM ve Uluslararası telefonlar
        └── NetworkValidator.cs       # Database URI, IPv4, IPv6, MAC, UUID
```

---

## 🔨 Derleme & Çalıştırma

### 1. Hazır Binary Çalıştırma:
`release\PII-Mask.exe` dosyasına çift tıklayarak uygulamayı doğrudan açabilirsiniz.

### 2. Kaynak Koddan Derleme:
Herhangi bir Windows bilgisayarda komut satırından veya `build.bat` dosyasını çalıştırarak derleyebilirsiniz:

```cmd
build.bat
```

Veya doğrudan C# derleyicisi ile:
```cmd
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /r:System.Windows.Forms.dll /r:System.Drawing.dll /out:release\PII-Mask.exe src\DesktopApp.cs src\Engine\*.cs src\Validators\*.cs
```

---

## 🔐 Uyumluluk ve Yasal Çerçeve

- **KVKK (Kişisel Verilerin Korunması Kanunu):** Panoya alınan kimlik ve iletişim verilerinin üçüncü taraf uygulamalara (AI/LLM web arayüzleri, mesajlaşma uygulamaları vb.) kontrolsüz yapıştırılmasını önler.
- **GDPR (General Data Protection Regulation):** Veri minimizasyonu ve yerel koruma sağlar.
- **PCI-DSS:** Kredi kartı PAN ve CVV verilerinin kopyalama sırasında sızmasını engeller.
