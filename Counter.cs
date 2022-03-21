namespace Penguin.Shared.Objects
{
    public class Counter
    {
        private readonly object _lock = new object();

        public int Increment()
        {
            lock (this._lock)
            {
                this._Value++;
                return this._Value;
            }

        }

        public static implicit operator Counter(int v) => new Counter(v);

        private int _Value;

        public int Value => this.Value;

        public Counter(int value)
        {
            this._Value = value;
        }
    }
}
