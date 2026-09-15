namespace PIIMask.App.Engine
{
    /// <summary>
    /// Kullanıcı arayüzünden seçilen aktif filtre yapılandırması
    /// </summary>
    public class FilterConfig
    {
        public bool TCKN = true;
        public bool CreditCard = true;
        public bool APIKey = true;
        public bool Email = true;
        public bool Phone = true;
        public bool DatabaseURL = true;
        public bool IPAddress = true;
        public bool UserName = true;
    }
}
