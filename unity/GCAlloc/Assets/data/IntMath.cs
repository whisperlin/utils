using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.IntUtil
{
    class IntMath
    {
        public static int square(int x)
        {
            return x*x;
        }

        public static int Sqrt(int x)
        {
            int temp = 0;
            int v_bit = 15;
            int n = 0;
            int b = 0x8000;
            if(x <= 1){
                return x;
            }
            do{
                temp = ((n << 1) + b) << (v_bit--);
                if (x >= temp)
                {
                    n += b;
                    x -= temp;
                }
            }while ((b >>= 1)>0);
            return n; 
        }

        public static int Abs(int x)
        {
            if (x < 0)
            {
                return -x;
            }
            return x;
        }

        public static int Min(int a, int b)
        {
            if (a > b)
            {
                return b;
            }
            else
            {
                return a;
            }
        }

    }
}
