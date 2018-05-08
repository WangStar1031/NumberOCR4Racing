using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace OCR
{
    class NumberRecognition
    {
        private RectArea[] m_arrRects = new RectArea[100];
        private Bitmap[] m_arrBitmaps = new Bitmap[100];

        private Bitmap m_BtmTemp;
        private RectArea m_RectTemp;
        private int[,] arrNumbers = new int[,] { { 1, 2, 1, 2, 0, 2, 2, 0, 2, 2, 0, 2, 1, 2, 1 }, { 2, 2, 2, 0, 2, 2, 0, 2, 2, 0, 2, 1, 0, 2, 1 }, { 2, 2, 2, 0, 0, 2, 0, 2, 0, 2, 1, 0, 2, 2, 1 }, { 2, 2, 2, 0, 2, 1, 0, 1, 1, 0, 0, 2, 0, 2, 1 }, { 2, 0, 1, 2, 0, 1, 2, 2, 2, 0, 2, 2, 0, 0, 1 }, { 2, 2, 2, 2, 0, 0, 0, 1, 1, 0, 0, 2, 2, 2, 0 }, { 0, 2, 0, 2, 0, 0, 2, 2, 2, 2, 0, 2, 1, 2, 1 }, { 2, 2, 2, 0, 0, 2, 0, 2, 0, 0, 2, 0, 1, 0, 0 }, { 1, 2, 1, 2, 0, 2, 1, 2, 2, 2, 0, 2, 1, 2, 1 }, { 2, 2, 2, 2, 0, 2, 2, 2, 1, 0, 1, 2, 0, 2, 0 } };

        private int getNumber(int[] _vector)
        {
            int[] arrDiffs = new int[10];
            for (int i = 0; i < 10; i++)
            {
                int nAllDiff = 0;
                for (int j = 0; j < 15; j++)
                {
                    int curModelVal = arrNumbers[i, j];
                    int curValueVal = _vector[j];
                    int nDiff = Math.Abs((curModelVal - curValueVal) * (curModelVal - curValueVal) * (curModelVal - curValueVal));
                    nAllDiff += nDiff;
                }
                arrDiffs[i] = nAllDiff;
            }
            int nMinDiff = 120;
            int nRetVal = 0;
            for (int i = 0; i < 10; i++)
            {
                if (nMinDiff > arrDiffs[i])
                {
                    nMinDiff = arrDiffs[i];
                    nRetVal = i;
                }
            }
            //            if (nMinDiff > 20)
            //                return "";
            return nRetVal;
        }
        private void travelBitmap(int x, int y)
        {
            if (x < 0 || x >= m_BtmTemp.Width || y < 0 || y >= m_BtmTemp.Height)
                return;

            m_RectTemp.reCalcArea(x, y);
            m_BtmTemp.SetPixel(x, y, Color.FromArgb(0, 0, 0));

            Color c = m_BtmTemp.GetPixel(x - 1, y);
            if (c.R != 0)
                travelBitmap(x - 1, y);
            c = m_BtmTemp.GetPixel(x + 1, y);
            if (c.R != 0)
                travelBitmap(x + 1, y);
            c = m_BtmTemp.GetPixel(x, y + 1);
            if (c.R != 0)
                travelBitmap(x, y + 1);
            c = m_BtmTemp.GetPixel(x, y - 1);
            if (c.R != 0)
                travelBitmap(x, y - 1);
        }
        public String Processing(Color[,] test_color, int color_width, int color_height)
        {
            Bitmap temp = new Bitmap(color_width, color_height);
            Color c;
            int nMin = 255, nMax = 0;
            for (int i = 0; i < temp.Width; i++)
            {
                for (int j = 0; j < temp.Height; j++)
                {
                    c = test_color[i, j];
                    byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);
                    if (nMax < gray) nMax = gray;
                    if (nMin > gray) nMin = gray;
                    temp.SetPixel(i, j, c);
                }
            }
            int nMid = (nMin * 3 + nMax) / 4;

            for (int i = 0; i < temp.Width; i++)
            {
                for (int j = 0; j < temp.Height; j++)
                {
                    c = temp.GetPixel(i, j);
                    byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);
                    if (gray > nMid) gray = 255;
                    else gray = 0;
                    temp.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            m_BtmTemp = (Bitmap)temp.Clone();
            int nArrRectCount = -1;
            String strRealVal = "";
            for (int i = 0; i < m_BtmTemp.Width; i++)
            {
                for (int j = 0; j < m_BtmTemp.Height; j++)
                {
                    c = m_BtmTemp.GetPixel(i, j);
                    byte value = (byte)c.R;
                    if (value != 0)
                    {
                        m_RectTemp = new RectArea(i, j);
                        travelBitmap(i, j);
                        i = m_RectTemp.right + 1;
                        if (m_RectTemp.Height() * 7 / 10 < m_RectTemp.Width())
                        {
                            int nCount = (int)Math.Round((double)(m_RectTemp.Width() * 2 / m_RectTemp.Height()));
                            for (int t = 0; t < nCount; t++)
                            {
                                nArrRectCount++;
                                m_arrRects[nArrRectCount] = new RectArea();
                                m_arrRects[nArrRectCount].left = (m_RectTemp.left * (nCount - t) + m_RectTemp.right * t) / nCount;
                                m_arrRects[nArrRectCount].right = (m_RectTemp.left * (nCount - t - 1) + m_RectTemp.right * (t + 1)) / nCount;
                                m_arrRects[nArrRectCount].top = m_RectTemp.top;
                                m_arrRects[nArrRectCount].bottom = m_RectTemp.bottom;
                            }
                        }
                        else
                        {
                            nArrRectCount++;
                            m_arrRects[nArrRectCount] = new RectArea();
                            m_arrRects[nArrRectCount].left = m_RectTemp.left;
                            m_arrRects[nArrRectCount].top = m_RectTemp.top;
                            m_arrRects[nArrRectCount].right = m_RectTemp.right;
                            m_arrRects[nArrRectCount].bottom = m_RectTemp.bottom;
                        }
                        break;
                    }
                }
            }
            m_BtmTemp = (Bitmap)temp.Clone();
            String allMessage = "";
            for (int i = 0; i <= nArrRectCount; i++)
            {
                RectArea rect = m_arrRects[i];
                int nWidth = rect.right - rect.left + 1;
                int nHeight = rect.bottom - rect.top + 1;
                int nThreshold = nWidth * nHeight / 15;
                int[] vector = new int[15];
                for (int a = 0; a < 15; a++)
                {
                    vector[a] = 0;
                }
                for (int x = rect.left; x <= rect.right; x++)
                {
                    for (int y = rect.top; y <= rect.bottom; y++)
                    {
                        c = m_BtmTemp.GetPixel(x, y);
                        byte value = (byte)(c.R / 255);
                        int nXPos = (x - rect.left) * 3 / nWidth;
                        int nYPos = (y - rect.top) * 5 / nHeight;
                        vector[nYPos * 3 + nXPos] += value;
                    }
                }
                String message = "";
                for (int a = 0; a < 15; a++)
                {
                    if (vector[a] < nThreshold / 2) vector[a] = 0;
                    else if (vector[a] < nThreshold * 3 / 4) vector[a] = 1;
                    else vector[a] = 2;
                    message = String.Concat(message, vector[a].ToString(), ",");
                }
                int nCurChar;
                if (nWidth < nHeight / 3)
                    nCurChar = 1;
                else
                    nCurChar = getNumber(vector);
                allMessage = String.Concat(allMessage, message, "\n");
                strRealVal = String.Concat(strRealVal, nCurChar.ToString());
            }
            return strRealVal;
        }
    }
}
