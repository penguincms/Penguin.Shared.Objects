namespace Penguin.Shared
{
    public class Counter
    {
        private readonly object numLock = new object();
        private long value = 0;
        public long Next => this.Increment();

        public static implicit operator Counter(long i) => new Counter()
        {
            value = i
        };

        /// <summary>
        /// Increments the counter by 1 in a thread safe manner
        /// </summary>
        /// <returns></returns>
        public long Increment()
        {
            long nv;
            lock (this.numLock)
            {
                //Not thread safe. Lol
                this.value++;
                nv = this.value;
            }

            return nv;
        }

        public override string ToString() => $"{this.value}";
    }
}