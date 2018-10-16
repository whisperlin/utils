using System;

namespace Engine.IntUtil
{
    public class IntRandom
    {
        private int pre;
        private int seed;

        public IntRandom()
        {
            this.seed = DateTime.Now.Second * DateTime.Now.Millisecond;
        }

        public IntRandom(int seed)
        {
            this.seed = seed;
        }

        public int Seed
        {
            get { return seed; }
            set { this.seed = value; }
        }

        //返回值大于等于零且小于 System.Int32.MaxValue 的随机数
        public int Next()
        {
            return IntMath.Abs(rand());
        }

        //返回值大于等于零且小于 maxValue 的随机数
        public int Next(int maxValue)
        {
            return Next() % maxValue;
        }

        //返回值大于等于minValue且小于 maxValue 的随机数
        public int Next(int minValue, int maxValue)
        {
            int r = Next();
            int mod = maxValue - minValue;
            return r % mod + minValue;
        }

        public int rand() {
            int ret = (seed * 7361238 + seed % 20037 * 1244 + pre * 12342 + 378211) * (seed + 134543);
            pre = seed;
            seed = ret;
            return ret;
        }
    }
}