namespace Penguin.Shared
{
    public class Counter
    {
        private readonly object numLock = new();
        private long value;
        public long Next => Increment();

        public static implicit operator Counter(long i)
        {
            return new Counter()
            {
                value = i
            };
        }

        /// <summary>
        /// Increments the counter by 1 in a thread safe manner
        /// </summary>
        /// <returns></returns>
        public long Increment()
        {
            long nv;
            lock (numLock)
            {
                //Not thread safe. Lol
                value++;
                nv = value;
            }

            return nv;
        }

        public override string ToString()
        {
            return $"{value}";
        }

        public Counter ToCounter()
        {
            throw new System.NotImplementedException();
        }
    }
}