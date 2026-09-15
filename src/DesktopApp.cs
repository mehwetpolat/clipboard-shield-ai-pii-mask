using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using PIIMask.App.Engine;

namespace PIIMask.App
{
    /// <summary>
    /// Modern, Yüksek Kontrastlı Filtre Butonu (Toggle Pill)
    /// </summary>
    public class FilterPillButton : Button
    {
        private bool _isChecked = true;
        private string _baseTitle = "";
        private string _filterKey = "";

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                UpdateAppearance();
            }
        }

        public string FilterKey
        {
            get { return _filterKey; }
        }

        public FilterPillButton(string title, string filterKey, bool initialChecked)
        {
            _baseTitle = title;
            _filterKey = filterKey;
            _isChecked = initialChecked;

            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 1;
            this.Cursor = Cursors.Hand;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.Size = new Size(174, 38);
            this.Margin = new Padding(4, 4, 4, 4);

            UpdateAppearance();

            this.Click += delegate(object s, EventArgs e)
            {
                IsChecked = !IsChecked;
            };
        }

        public FilterPillButton(string title, string filterKey) : this(title, filterKey, true)
        {
        }

        public void UpdateAppearance()
        {
            if (_isChecked)
            {
                this.BackColor = Color.FromArgb(16, 185, 129); // Zümrüt Yeşili (Aktif)
                this.ForeColor = Color.White;
                this.FlatAppearance.BorderColor = Color.FromArgb(52, 211, 153);
                this.Text = "✔ " + _baseTitle;
            }
            else
            {
                this.BackColor = Color.FromArgb(30, 41, 59); // Koyu Slate (Pasif)
                this.ForeColor = Color.FromArgb(148, 163, 184);
                this.FlatAppearance.BorderColor = Color.FromArgb(71, 85, 105);
                this.Text = "✖ " + _baseTitle;
            }
        }
    }

    /// <summary>
    /// Masaüstü Kullanıcı Arayüzü ve Pano Dinleyici Formu
    /// </summary>
    public class DesktopForm : Form
    {
        private NotifyIcon _notifyIcon;
        private Label _lblStatus;
        private Label _lblStatusBadge;
        private Button _btnToggle;
        private TextBox _txtLogs;
        private TextBox _txtTestInput;
        private TextBox _txtTestOutput;

        // Modern Filtre Butonları
        private FilterPillButton _pillTCKN;
        private FilterPillButton _pillCard;
        private FilterPillButton _pillAPIKey;
        private FilterPillButton _pillEmail;
        private FilterPillButton _pillPhone;
        private FilterPillButton _pillDBIP;
        private FilterPillButton _pillName;

        private TabControl _tabControl;
        private TabPage _tabDashboard;
        private TabPage _tabManualTest;
        private TabPage _tabHowToUse;
        private bool _isEnabled = true;
        private string _lastClipboardText = "";
        private Timer _clipboardTimer;

        public DesktopForm()
        {
            InitializeLayout();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.BringToFront();
            this.Activate();
            AppendLog("🛡️ Yerel Pano Koruma Kalkanı Devrede. Pano anlık olarak izleniyor.");
        }

        private void InitializeLayout()
        {
            this.Text = "🛡️ PII-Mask | Masaüstü Pano & Veri Gizlilik Kalkanı";
            this.Size = new Size(840, 660);
            this.MinimumSize = new Size(840, 660);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(11, 15, 25);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Pencere ve Görev Çubuğu İkonu
            try
            {
                this.Icon = AppIconHelper.GetAppIcon();
            }
            catch { }

            // Tab Control
            _tabControl = new TabControl
            {
                Location = new Point(16, 16),
                Size = new Size(792, 585),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            _tabDashboard = new TabPage
            {
                Text = "  🛡️ Kalkan & Canlı Pano  ",
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                Padding = new Padding(12)
            };

            _tabManualTest = new TabPage
            {
                Text = "  🧪 Manuel Metin Testi  ",
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                Padding = new Padding(12)
            };

            _tabHowToUse = new TabPage
            {
                Text = "  📖 Nasıl Kullanılır?  ",
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                Padding = new Padding(12)
            };

            SetupDashboardTab();
            SetupManualTestTab();
            SetupHowToUseTab();

            _tabControl.TabPages.Add(_tabDashboard);
            _tabControl.TabPages.Add(_tabManualTest);
            _tabControl.TabPages.Add(_tabHowToUse);
            this.Controls.Add(_tabControl);

            // System Tray NotifyIcon
            try
            {
                _notifyIcon = new NotifyIcon
                {
                    Icon = AppIconHelper.GetAppIcon(),
                    Text = "PII-Mask Gizlilik Kalkanı",
                    Visible = true
                };
                _notifyIcon.DoubleClick += delegate(object s, EventArgs e) { this.Show(); this.WindowState = FormWindowState.Normal; this.Activate(); };

                ContextMenu menu = new ContextMenu();
                menu.MenuItems.Add(new MenuItem("🌐 Pencereyi Göster", delegate(object s, EventArgs e) { this.Show(); this.WindowState = FormWindowState.Normal; this.Activate(); }));
                menu.MenuItems.Add(new MenuItem("❌ Çıkış", ExitApplication));
                _notifyIcon.ContextMenu = menu;
            }
            catch { }

            // Timer for Realtime Clipboard Checking (300ms ultra-fast response)
            _clipboardTimer = new Timer();
            _clipboardTimer.Interval = 300;
            _clipboardTimer.Tick += CheckClipboardContent;
            _clipboardTimer.Start();
        }

        private void SetupDashboardTab()
        {
            // Header Banner Card
            Panel pnlHeader = new Panel
            {
                Location = new Point(12, 12),
                Size = new Size(760, 75),
                BackColor = Color.FromArgb(30, 41, 59)
            };

            Label lblAppTitle = new Label
            {
                Text = "🛡️ PII-Mask Gizlilik Kalkanı",
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                Location = new Point(16, 12),
                AutoSize = true,
                ForeColor = Color.FromArgb(96, 165, 250)
            };
            pnlHeader.Controls.Add(lblAppTitle);

            _lblStatusBadge = new Label
            {
                Text = "🟢 AKTİF",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(295, 16),
                Size = new Size(75, 22),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(6, 78, 59),
                ForeColor = Color.FromArgb(52, 211, 153)
            };
            pnlHeader.Controls.Add(_lblStatusBadge);

            _lblStatus = new Label
            {
                Text = "Kopyalanan tüm hassas veriler (TCKN, Kart, API Key & Secret) anında yerel RAM üzerinde maskeleniyor.",
                Font = new Font("Segoe UI", 9F),
                Location = new Point(17, 43),
                AutoSize = true,
                ForeColor = Color.FromArgb(203, 213, 225)
            };
            pnlHeader.Controls.Add(_lblStatus);

            _btnToggle = new Button
            {
                Text = "🔴 Kalkanı Durdur",
                Location = new Point(595, 16),
                Size = new Size(150, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnToggle.FlatAppearance.BorderSize = 0;
            _btnToggle.Click += ToggleProtectionState;
            pnlHeader.Controls.Add(_btnToggle);

            _tabDashboard.Controls.Add(pnlHeader);

            // Group: Filter Settings
            GroupBox grpFilters = new GroupBox
            {
                Text = " 🔍 Aktif Koruma Filtreleri (Tıklayarak Açıp Kapatabilirsiniz) ",
                Location = new Point(12, 95),
                Size = new Size(760, 130),
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            FlowLayoutPanel flowFilters = new FlowLayoutPanel
            {
                Location = new Point(10, 24),
                Size = new Size(740, 98),
                AutoScroll = false,
                WrapContents = true
            };

            _pillTCKN = new FilterPillButton("TCKN & Kimlik No", "TCKN");
            _pillCard = new FilterPillButton("Kredi Kartı & IBAN", "KREDI_KARTI");
            _pillAPIKey = new FilterPillButton("API Key & Secret", "API_ANAHTARI");
            _pillEmail = new FilterPillButton("E-Posta", "EPOSTA");
            _pillPhone = new FilterPillButton("Telefon Numarası", "TELEFON");
            _pillDBIP = new FilterPillButton("Database URL & IP", "VERITABANI_BAGLANTISI");
            _pillName = new FilterPillButton("İsim & Soyisim", "KULLANICI");

            flowFilters.Controls.Add(_pillTCKN);
            flowFilters.Controls.Add(_pillCard);
            flowFilters.Controls.Add(_pillAPIKey);
            flowFilters.Controls.Add(_pillEmail);
            flowFilters.Controls.Add(_pillPhone);
            flowFilters.Controls.Add(_pillDBIP);
            flowFilters.Controls.Add(_pillName);

            grpFilters.Controls.Add(flowFilters);
            _tabDashboard.Controls.Add(grpFilters);

            // Group: Live Logs
            GroupBox grpLogs = new GroupBox
            {
                Text = " 📜 Canlı Pano & Kalkan Günlüğü ",
                Location = new Point(12, 235),
                Size = new Size(760, 275),
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            _txtLogs = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(12, 24),
                Size = new Size(736, 238),
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.FromArgb(226, 232, 240),
                Font = new Font("Consolas", 9.2F),
                BorderStyle = BorderStyle.FixedSingle
            };
            grpLogs.Controls.Add(_txtLogs);
            _tabDashboard.Controls.Add(grpLogs);

            // Bottom Footer
            Label lblFooter = new Label
            {
                Text = "🔒 %100 Çevrimdışı & Yerel Algoritmik Koruma | 0 Ağ İsteği | 50+ Algoritma ve Kural Aktif",
                Location = new Point(14, 520),
                AutoSize = true,
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Segoe UI", 8.5F)
            };
            _tabDashboard.Controls.Add(lblFooter);
        }

        private void SetupManualTestTab()
        {
            // Group: Interactive Test
            GroupBox grpTest = new GroupBox
            {
                Text = " 🧪 Manuel Metin Maskeleme & Doğrulama Testi ",
                Location = new Point(12, 12),
                Size = new Size(760, 520),
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            Label lblIn = new Label { Text = "Orijinal Girdi Metni (TCKN, Kart, Tel, API Key vb.):", Location = new Point(15, 25), AutoSize = true, ForeColor = Color.White };
            grpTest.Controls.Add(lblIn);

            _txtTestInput = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(15, 48),
                Size = new Size(355, 360),
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                Font = new Font("Consolas", 9.5F),
                BorderStyle = BorderStyle.FixedSingle
            };
            grpTest.Controls.Add(_txtTestInput);

            Label lblOut = new Label { Text = "Maskelenmiş Çıktı:", Location = new Point(385, 25), AutoSize = true, ForeColor = Color.White };
            grpTest.Controls.Add(lblOut);

            _txtTestOutput = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(385, 48),
                Size = new Size(355, 360),
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.FromArgb(52, 211, 153),
                Font = new Font("Consolas", 9.5F),
                BorderStyle = BorderStyle.FixedSingle
            };
            grpTest.Controls.Add(_txtTestOutput);

            Button btnTest = new Button
            {
                Text = "⚡ Maskele & Doğrula",
                Location = new Point(15, 420),
                Size = new Size(180, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTest.FlatAppearance.BorderSize = 0;
            btnTest.Click += RunManualTestMask;
            grpTest.Controls.Add(btnTest);

            Button btnLoadSample = new Button
            {
                Text = "📝 Örnek Veri Yükle",
                Location = new Point(205, 420),
                Size = new Size(165, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(71, 85, 105),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLoadSample.FlatAppearance.BorderSize = 0;
            btnLoadSample.Click += LoadSampleData;
            grpTest.Controls.Add(btnLoadSample);

            Button btnCopyOutput = new Button
            {
                Text = "📋 Çıktıyı Kopyala",
                Location = new Point(385, 420),
                Size = new Size(165, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCopyOutput.FlatAppearance.BorderSize = 0;
            btnCopyOutput.Click += delegate(object s, EventArgs e)
            {
                if (!string.IsNullOrEmpty(_txtTestOutput.Text))
                {
                    SafeSetClipboardText(_txtTestOutput.Text);
                    AppendLog("📋 Test çıktısı panoya kopyalandı.");
                }
            };
            grpTest.Controls.Add(btnCopyOutput);

            _tabManualTest.Controls.Add(grpTest);
        }

        private void SetupHowToUseTab()
        {
            GroupBox grpGuide = new GroupBox
            {
                Text = " 📖 PII-Mask Kullanım Kılavuzu & Detaylı Özellik Açıklamaları ",
                Location = new Point(12, 12),
                Size = new Size(760, 520),
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            RichTextBox rtbGuide = new RichTextBox
            {
                Location = new Point(15, 25),
                Size = new Size(730, 480),
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.FromArgb(226, 232, 240),
                Font = new Font("Segoe UI", 9.5F),
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            PopulateHowToUseGuide(rtbGuide);

            grpGuide.Controls.Add(rtbGuide);
            _tabHowToUse.Controls.Add(grpGuide);
        }

        private void PopulateHowToUseGuide(RichTextBox rtb)
        {
            rtb.Clear();

            AppendGuideHeading(rtb, "🛡️ 1. PII-Mask Nedir & Ne İşe Yarar?");
            AppendGuideBody(rtb, 
                "PII-Mask; geliştiricilerin, yapay zeka araçlarıyla (ChatGPT, Claude, Cursor, v0, Lovable vb.) çalışan " +
                "vibecoder'ların ve veri analistlerinin panoya kopyaladığı hassas müşteri veya sistem verilerini internete göndermeden " +
                "önce yerel RAM belleği üzerinde anında ve %100 çevrimdışı olarak maskeleyen bir güvenlik kalkanıdır.\r\n" +
                "• 0 Ağ İsteği: Hiçbir sunucuya bağlanmaz, internet bağlantısı gerektirmez.\r\n" +
                "• Mikro-Saniye Hızında Yerel Koruma: Verileriniz bilgisayarınızdan asla dışarı çıkmaz.");

            AppendGuideHeading(rtb, "⚡ 2. Kalkan & Canlı Pano Ekranı");
            AppendGuideBody(rtb,
                "• Otomatik Pano Koruması: Uygulama açıkken panonuza bir metin kopyaladığınızda (Ctrl+C), PII-Mask 50'den fazla " +
                "algoritmik kural ile metni anında tarar. Hassas veri tespit edildiğinde otomatik olarak '[MASKE_ETİKETİ]' formatına " +
                "dönüştürülerek güvenli hali panonuza geri yazılır.\r\n" +
                "• Kalkan Durumu (🟢 Aktif / 🔴 Kalkanı Durdur): Ana ekrandaki açma/kapama butonuyla korumayı tek tıkla " +
                "geçici olarak durdurabilir veya tekrar başlatabilirsiniz.\r\n" +
                "• Canlı Pano Günlüğü: RAM üzerinde gerçekleşen her maskeleme ve koruma işlemini zaman damgalı log olarak listeler.");

            AppendGuideHeading(rtb, "🔍 3. Aktif Koruma Filtreleri (Butonlar & Açma/Kapatma)");
            AppendGuideBody(rtb,
                "Filtre butonlarına tıklayarak istediğiniz koruma kuralını bağımsız olarak açıp kapatabilirsiniz:\r\n" +
                "• ✔ [Yeşil - Aktif]: Bu veri türü kopyalandığında otomatik olarak taranır ve maskelenir.\r\n" +
                "• ✖ [Koyu - Pasif]: Bu veri türü taranmaz, orijinal haliyle panoda korunur.\r\n\r\n" +
                "Tüm Filtrelerin Kapsamı ve Örnekleri:\r\n" +
                "1. TCKN & Kimlik No: 11 haneli T.C. Kimlik Numaralarını çift modülo algoritmasıyla doğrular ([TCKN_1]). Yabancı SSN, VKN, NINO verilerini de maskeler.\r\n" +
                "2. Kredi Kartı & IBAN: Luhn algoritmalı 16 haneli kart numaralarını ([KREDI_KARTI_1]), TR ve Uluslararası IBAN formatlarını ([IBAN_1]), 3-4 haneli CVV/CVC güvenlik kodlarını ([CVV]) ve Kart Son Kullanma Tarihlerini ([SON_KULLANMA_TARIHI]) maskeler.\r\n" +
                "3. API Key & Secret: OpenAI (sk-proj-...), Anthropic Claude, Google Gemini, GitHub, AWS Access/Secret Key, Stripe anahtarlarını, Bearer JWT token'larını, şifreleri (\"password\": \"...\") ve 6 haneli OTP/TOTP kodlarını ([API_ANAHTARI_1], [GIZLI_PAROLA], [OTP_KODU]) maskeler.\r\n" +
                "4. E-Posta: RFC 5322 uyumlu tüm kişisel ve kurumsal e-posta adreslerini ([EPOSTA_1]) maskeler.\r\n" +
                "5. Telefon Numarası: Türkiye cep telefonlarını (05xx, +90), sabit hatları ve uluslararası numaraları ([TELEFON_1]) maskeler.\r\n" +
                "6. Database URL & IP: PostgreSQL, MySQL, MongoDB, Redis gibi veritabanı bağlantı adreslerini ([VERITABANI_BAGLANTISI_1]) ve IPv4/IPv6 adreslerini ([IP_ADRESI_1]) maskeler.\r\n" +
                "7. İsim & Soyisim: JSON ve kodlar içerisindeki 'full_name', 'first_name', 'customer_name', 'ad_soyad' alanlarını ve 'Sayın [Ad Soyad]' hitaplarını ([KULLANICI_1]) JSON yapısını bozmadan maskeler.");

            AppendGuideHeading(rtb, "🧪 4. Manuel Metin Testi Ekranı");
            AppendGuideBody(rtb,
                "• Panoyu kullanmadan, elinizdeki bir JSON isteğini (Request), yanıtını (Response) veya kod bloğunu sol kutuya yapıştırıp '⚡ Maskele & Doğrula' butonuna basarak anında test edebilirsiniz.\r\n" +
                "• '📝 Örnek Veri Yükle' butonu ile hızlıca gerçekçi test verisi yükleyebilir, '📋 Çıktıyı Kopyala' butonu ile maskelenmiş sonucu alabilirsiniz.");

            AppendGuideHeading(rtb, "📌 5. Sistem Tepsisi (Tray) & Arka Plan Koruması");
            AppendGuideBody(rtb,
                "• Pencereyi kapattığınızda veya simge durumuna küçülttüğünüzde PII-Mask görev çubuğu bildirim alanında (saat yanında) sessizce çalışmaya devam eder.\r\n" +
                "• Kalkan simgesine çift tıklayarak pencereyi dilediğiniz an tekrar açabilir, sağ tıklayarak hızlı menüden çıkış yapabilirsiniz.");

            rtb.SelectionStart = 0;
            rtb.ScrollToCaret();
        }

        private void AppendGuideHeading(RichTextBox rtb, string heading)
        {
            rtb.SelectionFont = new Font("Segoe UI", 11.5F, FontStyle.Bold);
            rtb.SelectionColor = Color.FromArgb(56, 189, 248); // Canlı Cyan / Sky Blue
            rtb.AppendText(heading + "\r\n\r\n");
        }

        private void AppendGuideBody(RichTextBox rtb, string text)
        {
            rtb.SelectionFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            rtb.SelectionColor = Color.FromArgb(226, 232, 240); // Açık Slate Gri
            rtb.AppendText(text + "\r\n\r\n");
        }

        private void LoadSampleData(object sender, EventArgs e)
        {
            _txtTestInput.Text = "Sayın Ahmet Yılmaz (TCKN: 10000000146),\r\n" +
                                 "0532 123 45 67 numaralı telefonunuz ve 4539 1488 0343 6467 numaralı kartınız ile işlem yapılmıştır.\r\n" +
                                 "E-posta: ahmet.yilmaz@gmail.com\r\n" +
                                 "API Key: sk-proj-1234567890abcdef1234567890abcdef1234567890abcdef1234\r\n" +
                                 "DB URL: postgresql://postgres:SecretPass2026!@192.168.1.45:5432/main_db";
            AppendLog("📝 Örnek test verisi yüklendi.");
        }

        private FilterConfig GetCurrentFilterConfig()
        {
            FilterConfig cfg = new FilterConfig();
            cfg.TCKN = _pillTCKN.IsChecked;
            cfg.CreditCard = _pillCard.IsChecked;
            cfg.APIKey = _pillAPIKey.IsChecked;
            cfg.Email = _pillEmail.IsChecked;
            cfg.Phone = _pillPhone.IsChecked;
            cfg.DatabaseURL = _pillDBIP.IsChecked;
            cfg.IPAddress = _pillDBIP.IsChecked;
            cfg.UserName = _pillName.IsChecked;
            return cfg;
        }

        private void ToggleProtectionState(object sender, EventArgs e)
        {
            _isEnabled = !_isEnabled;
            if (_isEnabled)
            {
                _lblStatusBadge.Text = "🟢 AKTİF";
                _lblStatusBadge.BackColor = Color.FromArgb(6, 78, 59);
                _lblStatusBadge.ForeColor = Color.FromArgb(52, 211, 153);
                _btnToggle.Text = "🔴 Kalkanı Durdur";
                _btnToggle.BackColor = Color.FromArgb(239, 68, 68);
                AppendLog("🛡️ Güvenlik Kalkanı AKTİF edildi.");
            }
            else
            {
                _lblStatusBadge.Text = "🔴 PASİF";
                _lblStatusBadge.BackColor = Color.FromArgb(127, 29, 29);
                _lblStatusBadge.ForeColor = Color.FromArgb(248, 113, 113);
                _btnToggle.Text = "🟢 Kalkanı Başlat";
                _btnToggle.BackColor = Color.FromArgb(16, 185, 129);
                AppendLog("🔴 Güvenlik Kalkanı PASİF edildi.");
            }
        }

        private void RunManualTestMask(object sender, EventArgs e)
        {
            string input = _txtTestInput.Text;
            if (string.IsNullOrEmpty(input)) return;

            string masked = MaskEngine.ProcessText(input, GetCurrentFilterConfig());
            _txtTestOutput.Text = masked;
            AppendLog("⚡ Manuel maskeleme tamamlandı.");
        }

        private void CheckClipboardContent(object sender, EventArgs e)
        {
            if (!_isEnabled) return;

            string current = SafeGetClipboardText();
            if (!string.IsNullOrEmpty(current) && current != _lastClipboardText)
            {
                _lastClipboardText = current;
                string masked = MaskEngine.ProcessText(current, GetCurrentFilterConfig());

                if (!string.IsNullOrEmpty(masked) && masked != current)
                {
                    if (SafeSetClipboardText(masked))
                    {
                        _lastClipboardText = masked;
                        AppendLog("⚡ Pano Korundu: Hassas veriler yerel RAM üzerinde güvenle maskelendi.");
                    }
                }
            }
        }

        private string SafeGetClipboardText()
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        return Clipboard.GetText();
                    }
                }
                catch
                {
                    System.Threading.Thread.Sleep(20);
                }
            }
            return null;
        }

        private bool SafeSetClipboardText(string text)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    Clipboard.SetText(text);
                    return true;
                }
                catch
                {
                    System.Threading.Thread.Sleep(30);
                }
            }
            return false;
        }

        private void AppendLog(string msg)
        {
            if (_txtLogs.InvokeRequired)
            {
                _txtLogs.Invoke(new Action<string>(AppendLog), msg);
            }
            else
            {
                _txtLogs.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + msg + "\r\n");
            }
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            _clipboardTimer.Stop();
            if (_notifyIcon != null) _notifyIcon.Visible = false;
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                _clipboardTimer.Stop();
                if (_notifyIcon != null)
                {
                    _notifyIcon.Visible = false;
                    _notifyIcon.Dispose();
                }
            }
            catch { }

            base.OnFormClosing(e);
            Environment.Exit(0);
        }
    }

    /// <summary>
    /// Yüksek çözünürlüklü Vektörel Kalkan ve Güvenlik İkonu Yöneticisi
    /// </summary>
    public static class AppIconHelper
    {
        private static Icon _cachedIcon;

        public static Icon GetAppIcon()
        {
            if (_cachedIcon != null) return _cachedIcon;

            // 1. Assets klasöründeki ikon dosyasını kontrol et
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "app.ico");
                if (!File.Exists(iconPath))
                {
                    iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "assets", "app.ico");
                }

                if (File.Exists(iconPath))
                {
                    _cachedIcon = new Icon(iconPath);
                    return _cachedIcon;
                }
            }
            catch { }

            // 2. Bellekte dinamik yüksek çözünürlüklü vektörel kalkan ikonu üret
            try
            {
                using (Bitmap bmp = new Bitmap(64, 64, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.Clear(Color.Transparent);

                        PointF[] points = new PointF[]
                        {
                            new PointF(32, 4),
                            new PointF(56, 12),
                            new PointF(56, 35),
                            new PointF(32, 60),
                            new PointF(8, 35),
                            new PointF(8, 12)
                        };

                        using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                        {
                            path.AddPolygon(points);
                            using (System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, 64, 64), Color.FromArgb(14, 165, 233), Color.FromArgb(15, 23, 42), 65f))
                            {
                                g.FillPath(brush, path);
                            }
                            using (Pen pen = new Pen(Color.FromArgb(56, 189, 248), 2.5f))
                            {
                                pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                                g.DrawPath(pen, path);
                            }

                            // Kilit & Güvenlik Sembolü
                            using (SolidBrush lockBrush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                            {
                                using (Pen lockPen = new Pen(Color.White, 2.5f))
                                {
                                    g.DrawArc(lockPen, 26, 20, 12, 12, 180, 180);
                                }
                                g.FillRectangle(lockBrush, 24, 28, 16, 13);
                                using (SolidBrush holeBrush = new SolidBrush(Color.FromArgb(15, 23, 42)))
                                {
                                    g.FillEllipse(holeBrush, 30, 31, 4, 4);
                                }
                            }
                        }
                    }
                    IntPtr hIcon = bmp.GetHicon();
                    _cachedIcon = Icon.FromHandle(hIcon);
                    return _cachedIcon;
                }
            }
            catch
            {
                return SystemIcons.Shield;
            }
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new DesktopForm());
            }
            catch (Exception ex)
            {
                File.WriteAllText("crash_log.txt", ex.ToString());
                MessageBox.Show("Uygulama Hatası: " + ex.Message, "PII-Mask Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
