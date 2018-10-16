using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
 
namespace Engine.IntUtil
{
    //[System.Serializable]
	public class IntVector3
    {
        public int x;
        public int y;
        public int z;

        public IntVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Set(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static IntVector3 right { get { return new IntVector3(1, 0, 0); } }
        public static IntVector3 left { get { return new IntVector3(-1, 0, 0); } }
        public static IntVector3 up  {  get  {  return new IntVector3(0, 1, 0);  } }
        public static IntVector3 down { get  {  return new IntVector3(0, -1, 0);  } }
        public static IntVector3 forward { get { return new IntVector3(0, 0, 1); } }
        public static IntVector3 back { get { return new IntVector3(0, 0, -1); } }
        public static IntVector3 zero { get  { return new IntVector3(0, 0, 0); } }

        public static IntVector3 c100 { get { return new IntVector3(100, 100, 100); } }

        /// <summary>
        /// x,z分量的4个斜对角方向
        /// </summary>
        public static IntVector3 diagonalLB { get { return new IntVector3(-1, 0, -1); } }
        public static IntVector3 diagonalLF { get { return new IntVector3(-1, 0, 1); } }
        public static IntVector3 diagonalRB { get { return new IntVector3(1, 0, -1); } }
        public static IntVector3 diagonalRF { get { return new IntVector3(1, 0, 1); } }

        public static IntVector3 operator -(IntVector3 a)
        {
            return new IntVector3(-a.x, -a.y, -a.z);
        }

        public static IntVector3 operator +(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static IntVector3 operator -(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static IntVector3 operator *(IntVector3 a, int b)
        {
            return new IntVector3(a.x * b, a.y * b, a.z * b);
        }

        public static IntVector3 operator *(int b, IntVector3 a)
        {
            return new IntVector3(b * a.x, b * a.y, b * a.z);
        }

        public static IntVector3 operator /(IntVector3 a, int b)
        {
            if (b == 0)
            {
                throw new Exception("分母不能等于0");
            }
            else
            {
                return new IntVector3(a.x / b, a.y / b, a.z / b);
            }
        }

        public static implicit operator Vector3(IntVector3 a)
        {
            return new Vector3(a.x / 100f, a.y / 100f, a.z / 100f);
        }

        public static implicit operator IntVector2(IntVector3 a)
        {
            return new IntVector2(a.x, a.z);
        }

        public static bool operator !=(IntVector3 lhs, IntVector3 rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static bool operator ==(IntVector3 lhs, IntVector3 rhs)
        {
            return lhs.Equals(rhs);
        }

        public bool Equals(IntVector3 a)
        {
            if (this.x == a.x && this.y == a.y && this.z == a.z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static IntVector3 Zero()
        {
            return new IntVector3(0, 0, 0);
        }

        public int Square()
        {
            return x * x + y * y + z * z;
        }

        //求长度，结果取整
        public int Magnitude()
        {
            return IntMath.Sqrt(Square());
        }

        public int Distance(IntVector3 a)
        {
            IntVector3 b = this - a;
            return b.Magnitude();
        }

        //求两向量的角度
        public static int Angle(IntVector3 from, IntVector3 to)
        {
            return 0;
            //return (int)IntMath.Round(IntMath.Acos((from.x * to.x + from.y * to.y + from.z * to.z) / 
            //    (IntMath.Sqrt(from.x * from.x + from.y * from.y + from.z * from.z) * IntMath.Sqrt(to.x * to.x + to.y * to.y + to.z * to.z))) * 180 / IntMath.PI, 0);
        }

        //规范化为指定长度
        public IntVector3 Normalize(int len)
        {
            if (len == 0)
            {
                x = 0;
                y = 0;
                z = 0;

                return this;
            }

            int lenght = this.Magnitude();
            if (lenght != 0)
            {
                this.x = this.x * len / lenght;
                this.y = this.y * len / lenght;
                this.z = this.z * len / lenght;
            }

            return this;
        }

        /// <summary>
        /// 插值接近b，最终a，b的分量可能相差1
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="precent">百分点</param>
        /// <returns></returns>
        public static IntVector3 Lerp(IntVector3 a, IntVector3 b, int precent)
        {
            if(precent<0||precent>100)
                throw new Exception("百分比不正确！");
            else
                return (b - a) * precent / 100 + a;
        }

        public static IntVector3 MoveTowards(IntVector3 current, IntVector3 target, int maxDistanceDelta)
        {
            IntVector3 v = target - current;
            if (v.Magnitude() == 0)
            {
                return target;
            }
            v = v * maxDistanceDelta / v.Magnitude();
            IntVector3 s = current + v;
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
            if (v.z > 0 && s.z > target.z)
            {
                s.z = target.z;
            }
            if (v.z < 0 && s.z < target.z)
            {
                s.z = target.z;
            }
            return s;
        }
       

        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + ")";
        }
    }
}