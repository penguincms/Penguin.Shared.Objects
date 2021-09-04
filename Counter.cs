namespace Penguin.Shared.Objects
{
    public class Counter
    {
        private readonly object _lock = new object();

        public int Increment()
        {
            lock (_lock)
            {
                _Value++;
                return _Value;
            }

        }

        public static implicit operator Counter(int v)
        {
            return new Counter(v);
        }

        private int _Value;

        public int Value => Value;

        public Counter(int value)
        {
            _Value = value;
        }
    }
}
