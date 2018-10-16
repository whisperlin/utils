using UnityEngine;
using System;
 

namespace Engine.IntUtil
{
	public struct IntVector2
    {
        public int x;
        public int y;

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static IntVector2 _zero = new IntVector2(0, 0);
        public static IntVector2 zero { get { return _zero; } }

        public static IntVector2 one { get { return  new IntVector2  (1, 1);  }  }
        public static IntVector2 operator -(IntVector2 a)
        {
            return new IntVector2( - a.x, - a.y);
        }

        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x + b.x, a.y + b.y);
        }

        public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x - b.x, a.y - b.y);
        }

        public static IntVector2 operator *(IntVector2 a, int b)
        {
            return new IntVector2(a.x * b, a.y * b);
        }

        public static IntVector2 operator *(int b, IntVector2 a)
        {
            return new IntVector2( b * a.x, b * a.y);
        }

        public static implicit operator IntVector3(IntVector2 a)
        {
            return new IntVector3(a.x, 0, a.y);
        }

        public static IntVector2 operator /(IntVector2 a, int b)
        {
            if (b == 0)
            {
                throw new Exception("分母不能等于0");
            }
            else
            {
                return new IntVector2(a.x / b, a.y / b);
            }
        }

        public bool Equals(IntVector2 a)
        {
            if (this.x == a.x && this.y == a.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }        
        public int SqrMgnitude()
        {
            return x * x + y * y;
        }

        //求长度，结果取整
        public int Magnitude()
        {
            return (int) IntMath.Sqrt(x * x + y * y);
        }

        public int Distance(IntVector2 a)
        {
            IntVector2 b = this - a;
            return b.Magnitude();
        }

        //求两向量的角度
        public static int Angle(IntVector2 from, IntVector2 to)
        {
            return 0;
            //return (int)IntMath.Round(IntMath.Acos((from.x * to.x + from.y * to.y) / (IntMath.Sqrt(from.x * from.x + from.y * from.y) * IntMath.Sqrt(to.x * to.x + to.y * to.y))) * 180 / IntMath.PI, 0);
        }

        //规范化为1
        public void Normalize()
        {
            int lenght = this.Magnitude();
            if (lenght > 0)
            {
                this.x = IntMath.Abs(this.x / lenght);
                this.y = IntMath.Abs(this.y / lenght);
            }
        }

        //规范化为指定长度
        public void Normalize(int len)
        {
            int lenght = (int)(this.Magnitude()/len);
            if (lenght > 0)
            {
                this.x = IntMath.Abs(this.x / lenght);
                this.y = IntMath.Abs(this.y / lenght);
            }
        }

        public static IntVector2 MoveTowards(IntVector2 current, IntVector2 target, int maxDistanceDelta)
        {
            IntVector2 v = target - current;
            if (v.Magnitude() == 0)
            {
                return target;
            }
            v = v * maxDistanceDelta / v.Magnitude();
            IntVector2 s = current + v;
            if (v.x > 0 && s.x > target.x)
            {
                s.x = target.x;
            }
            if (v.x < 0 && s.x < target.x)
            {
                s.x = target.x;
            }
            if (v.y > 0 && s.y > target.y)
            {
                s.y = target.y;
            }
            if (v.y < 0 && s.y < target.y)
            {
                s.y = target.y;
            }
            return s;
        }

        

     

        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }
    }
}