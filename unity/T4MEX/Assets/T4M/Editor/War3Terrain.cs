using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

/**
 * 魔兽3地表
 * @author hackhuang
 */
public class War3Terrain
{
#if WAR3_TERRAIN
    enum ChannelType
    {
        RED = 0, GREEN, BLUE, ALPHA,
    }

    static int[] iEdgeColor = new int[4] { 1, 2, 4, 8 };

    static float MinusPixelColor(int iPoint, int layer, float v, int EdgeColor)
    {
        if (v != 0 && T4MSC.IsPointBrush(iPoint, layer))
            return Mathf.Max(Mathf.Floor(v) - EdgeColor, 0);

        return v;
    }

    public static void BrushTerrain_New(ref Color[] terrainBay, ref Color[] terrainBay_sec, int x, int y, int width, int height)
    {
        T4MSC.CreateT4MBrushPoint();

        int CurrentLayer = T4MSC.T4MselTexture;

        for (int i = 0; i < height - 1 && true; i++)
        {
            for (int j = 0; j < width - 1; j++)
            {
                int startIndex = i * width + j;

                int[] IndexList = new int[4] { startIndex, startIndex + 1, startIndex + width, startIndex + width + 1 };

                int iPoint = (y + i) * (T4MSC.T4MMaskTex.width + 1) + (x + j);
                for (int col = 0; col < 4; col++)
                {
                    int index = IndexList[col];
                    Color current = terrainBay[index] * 255;
                    Color currentSec = terrainBay_sec[index] * 255;
                    int r = (int)Mathf.Floor(current.r);
                    int g = (int)Mathf.Floor(current.g);
                    int b = (int)Mathf.Floor(current.b);
                    int a = (int)Mathf.Floor(current.a);

                    int r1 = (int)Mathf.Floor(currentSec.r);
                    int g1 = (int)Mathf.Floor(currentSec.g);
                    int b1 = (int)Mathf.Floor(currentSec.b);
                    int a1 = (int)Mathf.Floor(currentSec.a);

                    int EdgeCol = iEdgeColor[col];
                    if (T4MSC.T4MselTexture == 0)
                    {
                        current.g = MinusPixelColor(iPoint, 1, g, EdgeCol);
                        current.b = MinusPixelColor(iPoint, 2, b, EdgeCol);
                        current.a = MinusPixelColor(iPoint, 3, a, EdgeCol);

                        currentSec.r = MinusPixelColor(iPoint, 4, r1, EdgeCol);
                        currentSec.g = MinusPixelColor(iPoint, 5, g1, EdgeCol);
                        currentSec.b = MinusPixelColor(iPoint, 6, b1, EdgeCol);
                        currentSec.a = MinusPixelColor(iPoint, 7, a1, EdgeCol);

                        if (!T4MSC.IsPointBrush(iPoint, CurrentLayer))
                            current.r = (Mathf.Floor(current.r) + EdgeCol) % 16;
                    }
                    else if (T4MSC.T4MselTexture == 1)
                    {
                        current.b = MinusPixelColor(iPoint, 2, b, EdgeCol);
                        current.a = MinusPixelColor(iPoint, 3, a, EdgeCol);

                        currentSec.r = MinusPixelColor(iPoint, 4, r1, EdgeCol);
                        currentSec.g = MinusPixelColor(iPoint, 5, g1, EdgeCol);
                        currentSec.b = MinusPixelColor(iPoint, 6, b1, EdgeCol);
                        currentSec.a = MinusPixelColor(iPoint, 7, a1, EdgeCol);

                        if (!T4MSC.IsPointBrush(iPoint, CurrentLayer))
                        {
                            if (T4MSC.ClearPerviousLayer)
                            {
                                current.r = MinusPixelColor(iPoint, 0, r, EdgeCol);
                            }

                            current.g = (Mathf.Floor(current.g) + EdgeCol) % 16;
                        }
                    }
                    else if (T4MSC.T4MselTexture == 2)
                    {
                        current.a = MinusPixelColor(iPoint, 3, a, EdgeCol);

                        currentSec.r = MinusPixelColor(iPoint, 4, r1, EdgeCol);
                        currentSec.g = MinusPixelColor(iPoint, 5, g1, EdgeCol);
                        currentSec.b = MinusPixelColor(iPoint, 6, b1, EdgeCol);
                        currentSec.a = MinusPixelColor(iPoint, 7, a1, EdgeCol);

                        if (!T4MSC.IsPointBrush(iPoint, CurrentLayer))
                        {
                            if (T4MSC.ClearPerviousLayer)
                            {
                                current.r = MinusPixelColor(iPoint, 0, r, EdgeCol);
                                current.g = MinusPixelColor(iPoint, 1, g, EdgeCol);
                            }

                            current.b = (Mathf.Floor(current.b) + EdgeCol) % 16;
                        }
                    }
                    else if (T4MSC.T4MselTexture == 3)
                    {
                        currentSec.r = MinusPixelColor(iPoint, 4, r1, EdgeCol);
                        currentSec.g = MinusPixelColor(iPoint, 5, g1, EdgeCol);
                        currentSec.b = MinusPixelColor(iPoint, 6, b1, EdgeCol);
                        currentSec.a = MinusPixelColor(iPoint, 7, a1, EdgeCol);

                        if (!T4MSC.IsPointBrush(iPoint, CurrentLayer))
                        {
                            current.a = (Mathf.Floor(current.a) + EdgeCol);
                            if (T4MSC.ClearPerviousLayer)
                            {
                                current.r = MinusPixelColor(iPoint, 0, r, EdgeCol);
                                current.g = MinusPixelColor(iPoint, 1, g, EdgeCol);
                                current.b = MinusPixelColor(iPoint, 2, b, EdgeCol);
                            }
                        }
                    }
                    else if (T4MSC.T4MselTexture == 4)
                    {
                        currentSec.g = MinusPixelColor(iPoint, 5, g1, EdgeCol);
                        currentSec.b = MinusPixelColor(iPoint, 6, b1, EdgeCol);
                        currentSec.a = MinusPixelColor(iPoint, 7, a1, EdgeCol);

                        if (!T4MSC.IsPointBrush(iPoint, CurrentLayer))
                        {
                            currentSec.r = (Mathf.Floor(currentSec.r) + EdgeCol);
                            if (T4MSC.ClearPerviousLayer)
                            {
                                current.r = MinusPixelColor(iPoint, 0, r, EdgeCol);
                                current.g = MinusPixelColor(iPoint, 1, g, EdgeCol);
                                current.b = MinusPixelColor(iPoint, 2, b, EdgeCol);
                                current.a = MinusPixelColor(iPoint, 3, a, EdgeCol);
                            }
                        }
                    }
                    else if (T4MSC.T4MselTexture == 5)
                    {
                        currentSec.b = MinusPixelColor(iPoint, 6, b1, EdgeCol);
                        currentSec.a = MinusPixelColor(iPoint, 7, a1, EdgeCol);

                        if (!T4MSC.IsPointBrush(iPoint, CurrentLayer))
                        {
                            currentSec.g = (Mathf.Floor(currentSec.g) + EdgeCol);
                            if (T4MSC.ClearPerviousLayer)
                            {
                                current.r = MinusPixelColor(iPoint, 0, r, EdgeCol);
                                current.g = MinusPixelColor(iPoint, 1, g, EdgeCol);
                                current.b = MinusPixelColor(iPoint, 2, b, EdgeCol);
                                current.a = MinusPixelColor(iPoint, 3, a, EdgeCol);

                                currentSec.r = MinusPixelColor(iPoint, 4, r1, EdgeCol);
                            }
                        }
                    }
                    else if (T4MSC.T4MselTexture == 6)
                    {
                        currentSec.a = MinusPixelColor(iPoint, 7, a1, EdgeCol);

                        if (!T4MSC.IsPointBrush(iPoint, CurrentLayer))
                        {
                            currentSec.b = (Mathf.Floor(currentSec.b) + EdgeCol);
                            if (T4MSC.ClearPerviousLayer)
                            {
                                current.r = MinusPixelColor(iPoint, 0, r, EdgeCol);
                                current.g = MinusPixelColor(iPoint, 1, g, EdgeCol);
                                current.b = MinusPixelColor(iPoint, 2, b, EdgeCol);
                                current.a = MinusPixelColor(iPoint, 3, a, EdgeCol);

                                currentSec.r = MinusPixelColor(iPoint, 4, r1, EdgeCol);
                                currentSec.g = MinusPixelColor(iPoint, 5, g1, EdgeCol);
                            }
                        }
                    }
                    else if (T4MSC.T4MselTexture == 7)
                    {
                        if (!T4MSC.IsPointBrush(iPoint, CurrentLayer))
                        {
                            currentSec.a = (Mathf.Floor(currentSec.a) + EdgeCol);
                            if (T4MSC.ClearPerviousLayer)
                            {
                                current.r = MinusPixelColor(iPoint, 0, r, EdgeCol);
                                current.g = MinusPixelColor(iPoint, 1, g, EdgeCol);
                                current.b = MinusPixelColor(iPoint, 2, b, EdgeCol);
                                current.a = MinusPixelColor(iPoint, 3, a, EdgeCol);

                                currentSec.r = MinusPixelColor(iPoint, 4, r1, EdgeCol);
                                currentSec.g = MinusPixelColor(iPoint, 5, g1, EdgeCol);
                                currentSec.b = MinusPixelColor(iPoint, 6, g1, EdgeCol);
                            }
                        }
                    }

                    terrainBay[index] = current / 255;
                    terrainBay_sec[index] = currentSec / 255;
                }

                T4MSC.SetBrushPoint(iPoint, CurrentLayer, true);
                if (T4MSC.ClearPerviousLayer)
                {
                    for (int idx = 0; idx < 8; idx++)
                    {
                        if (idx != CurrentLayer)
                            T4MSC.SetBrushPoint(iPoint, idx, false);
                    }

                }
                else
                {
                    for (int idx = CurrentLayer + 1; idx < 8; idx++)
                    {
                        T4MSC.SetBrushPoint(iPoint, idx, false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 对地形刷新
    /// </summary>
    /// <param name="MaskColor"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public static void BrushTerrain(ref Color[] terrainBay, int width, int height)
    {
        for (int i = 0; i < height - 1 && true; i++)
        {
            for (int j = 0; j < width - 1; j++)
            {
                int index = i * width + j;

                int[] IndexList = new int[4] { index, index + 1, index + width, index + width + 1 };
                for (int col = 0; col < 4; col++)
                {
                    UpdateTileTexture(col, T4MSC.T4MselTexture, ref terrainBay[IndexList[col]]);
                }
            }
        }
    }

    static void UpdateTileTexture(int index, int useTexture, ref Color color)
    {
        bool hasSet = false;
        int channel = -1; int emptychannel = -1;
        for (channel = 0; channel < 4; channel++)
        {
            int intValue = GetColor(color, channel);

            int maskCode = intValue & 0x0f;
            if (maskCode == 0) break;

            int oTexture = intValue >> 4;
            int newValue = GetTheNBitHigh(maskCode, index);

            //说明这位置要进行覆盖
            if (maskCode == newValue) 
            {
                //说明是同一张图
                if (useTexture == oTexture) return;

                newValue = GetTheBitNLow(maskCode, index);
                intValue = (oTexture << 4) + newValue;
                if (newValue == 0)
                {
                    intValue = 0; emptychannel = channel;//后面要住上移 
                }

                SetColor(intValue, channel, ref color); continue;
            }

            //如果相同贴图则要加
            if (useTexture == oTexture)
            {
                intValue |= 1 << index; hasSet = true;

                SetColor(intValue, channel, ref color);
            }

            //说明后面要往上移
            if (emptychannel != -1)
            {
                SetColor(intValue, emptychannel, ref color);

                SetColor(0, channel, ref color); emptychannel = channel;
            }

            //其它没占你位不关我事
        }

        if (hasSet) return;

        //放最后进行覆盖
        int topchannel = emptychannel == -1 ? channel : emptychannel;

        SetColor((useTexture << 4) | (1 << index), topchannel, ref color);
    }

    /// <summary>
    /// 获取二进制n位为高位后的值
    /// </summary>
    /// <param name="x"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    static int GetTheNBitHigh(int x, int n)
    {
        return x | (1 << (n & 31));
    }

    /// <summary>
    /// 获取二进制n位为低位后的值
    /// </summary>
    /// <param name="x"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static int GetTheBitNLow(int x, int n)
    {
        return x & ~(1 << n);
    }

    static int GetColor(Color color, int channel)
    {
        float colorValue = 0;
        switch ((ChannelType)channel)
        {
            case ChannelType.RED: colorValue = color.r; break;
            case ChannelType.GREEN: colorValue = color.g; break;
            case ChannelType.BLUE: colorValue = color.b; break;
            case ChannelType.ALPHA: colorValue = color.a; break;
        }

        return Mathf.FloorToInt(colorValue * 255);
    }

    static void SetColor(int value, int channel, ref Color color)
    {
        float colorValue = value / 255f;
        switch ((ChannelType)channel)
        {
            case ChannelType.RED: color.r = colorValue; break;
            case ChannelType.GREEN: color.g = colorValue; break;
            case ChannelType.BLUE: color.b = colorValue; break;
            case ChannelType.ALPHA: color.a = colorValue; break;
            default: Debug.LogError("fkkkkkkkkkkkkkkkkkkk" + channel); break;
        }
    }
#endif
}
