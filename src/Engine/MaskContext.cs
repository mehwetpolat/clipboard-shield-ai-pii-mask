namespace PIIMask.App.Engine
{
    /// <summary>
    /// Maskeleme işlemi boyunca sayaç ve durum takibi sağlayan bağlam nesnesi
    /// </summary>
    public class MaskContext
    {
        private int _counter = 1;

        public int Next()
        {
            return _counter++;
        }

        public void Reset()
        {
            _counter = 1;
        }
    }
}
