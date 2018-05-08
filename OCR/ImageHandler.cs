using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace OCR
{
    public class ImageHandler
    {
        public Bitmap _currentBitmap;
        public Bitmap __tempBitmap;
        public Color[,] test_color;

        public ImageHandler()
        {

        }

        public void SetBrightness(int brightness)
        {
            Bitmap temp = (Bitmap)_currentBitmap;
            Bitmap bmap = (Bitmap)temp.Clone();
            if (brightness < -255) brightness = -255;
            if (brightness > 255) brightness = 255;
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    int cR = c.R + brightness;
                    int cG = c.G + brightness;
                    int cB = c.B + brightness;

                    if (cR < 0) cR = 1;
                    if (cR > 255) cR = 255;

                    if (cG < 0) cG = 1;
                    if (cG > 255) cG = 255;

                    if (cB < 0) cB = 1;
                    if (cB > 255) cB = 255;

                    bmap.SetPixel(i, j, Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
                }
            }
            _currentBitmap = (Bitmap)bmap.Clone();
        }

        public int[] CheckRegion(int limit, int max_limit)
        {
            Color c;
            int[] result = new int[2];
            int col_max = 0;
            int col_length = 0;
            bool start = false;
            bool end = false;
            int s_pos = _currentBitmap.Height - 1;
            int e_pos = 0;
            int col_e_pos = _currentBitmap.Width - 1;

            for (int j = _currentBitmap.Height - 1; j >= 0; j--)
            {
                col_max = 0;
                col_length = 0;
                for (int i = _currentBitmap.Width - 1; i >= 0; i -= 3)
                {
                    c = _currentBitmap.GetPixel(i, j);
                    byte gray = (byte)c.R;
                    if (gray > limit)
                    {
                        if (col_length > col_max) col_max = col_length;
                        col_length = 0;
                    }
                    else
                    {
                        col_length++;
                    }
                }
                if (col_length > col_max) col_max = col_length;
                if (col_max > max_limit)
                {
                    if (end)
                    {
                        e_pos = j;
                        break;
                    }
                    else
                    {
                        start = true;
                    }
                }
                else
                {
                    if (start)
                    {
                        start = false;
                        s_pos = j;
                        end = true;
                    }
                }
            }

            result[0] = s_pos;
            result[1] = e_pos;

            return result;
        }

        public bool CheckWide(int s_pos, int e_pos, int limit, int max_limit)
        {
            bool whiteCheck = false;
            int col_max = 0;
            int col_length = 0;
            bool col_s_check = true;
            int col_s_pos = 0;

            Color c;

            for (int i = 0; i < _currentBitmap.Width; i++)
            {
                whiteCheck = false;
                for (int j = 0; j < s_pos - e_pos; j++)
                {
                    c = _currentBitmap.GetPixel(i, j + e_pos);
                    byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);
                    if (gray > limit) whiteCheck = true;
                }
                if (whiteCheck)
                {
                    col_s_check = false;
                    col_length++;
                }
                else
                {
                    if (col_s_check) col_s_pos = i;
                    if (col_length > col_max) col_max = col_length;
                    col_length = 0;
                }
            }
            if (col_length > col_max) col_max = col_length;
            return (col_max < max_limit);
        }



        public void AdjustRegion(bool check_correct, int s_pos, int e_pos, int limit)
        {
            if (check_correct)
            {
                Color c;
                bool whiteCheck = false;
                bool col_s_check = true;
                int col_s_pos = 0;

                for (int i = 0; i < __tempBitmap.Width; i++)
                {
                    whiteCheck = false;

                    for (int j = 0; j < s_pos - e_pos; j++)
                    {
                        c = __tempBitmap.GetPixel(i, j + e_pos);
                        byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);
                        if (gray > limit) whiteCheck = true;
                    }
                    if (whiteCheck)
                    {
                        col_s_check = false;
                        break;
                    }
                    else
                    {
                        if (col_s_check) col_s_pos = i;
                    }
                }
                int new_with = (int)_currentBitmap.Width - col_s_pos;
                if (new_with > 10)
                {
                    _currentBitmap = new Bitmap(new_with, s_pos - e_pos);

                    for (int i = 0; i < new_with; i++)
                    {
                        whiteCheck = false;

                        for (int j = 0; j < s_pos - e_pos; j++)
                        {
                            c = __tempBitmap.GetPixel(i + col_s_pos, j + e_pos);
                            byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);
                            if (gray > limit) gray = 255;
                            else gray = 0;
                            //_currentBitmap.SetPixel(i, j, Color.FromArgb((byte)gray, (byte)gray, (byte)gray)); 
                            _currentBitmap.SetPixel(i, j, c);
                        }
                    }
                }
                else
                {
                    _currentBitmap = new Bitmap(1, 1);
                }
            }
            else
            {
                _currentBitmap = new Bitmap(1, 1);
            }
            __tempBitmap.Dispose();

        }

        public static Boolean CheckPixelColorFromBitmap(Bitmap bmp1, int left1, int top1, Bitmap bmp2, int left2, int top2)
        {
            if ((left1 < 0) || (top1 < 0) || (left1 > bmp1.Width) || (top1 > bmp1.Height)) return false;
            if ((left2 < 0) || (top2 < 0) || (left2 > bmp2.Width) || (top2 > bmp2.Height)) return false;
            Color c1 = bmp1.GetPixel(left1, top1);
            Color c2 = bmp2.GetPixel(left2, top2);

            int color_diff = Math.Abs(c1.R - c2.R) * Math.Abs(c1.R - c2.R) + Math.Abs(c1.G - c2.G) * Math.Abs(c1.G - c2.G) + Math.Abs(c1.B - c2.B) * Math.Abs(c1.B - c2.B);
            //Debug.Print("Color Diff : " + color_diff.ToString());
            return (color_diff < 4800);
        }

        public Boolean CheckPixelColorOver(int left, int top, int R, int G, int B)
        {
            if ((left < 0) || (top < 0) || (left > _currentBitmap.Width) || (top > _currentBitmap.Height)) return false;
            Color c = _currentBitmap.GetPixel(left, top);

            int color_diff = 0;
            if (c.R < R) color_diff += Math.Abs(c.R - R) * Math.Abs(c.R - R);
            if (c.G < G) color_diff += Math.Abs(c.G - G) * Math.Abs(c.G - G);
            if (c.B < B) color_diff += Math.Abs(c.B - B) * Math.Abs(c.B - B);
            //Debug.Print("Color Diff : " + color_diff.ToString());
            return (color_diff < 400);
        }
        public Boolean CheckPixelColor(Bitmap __bitmap, int R, int G, int B, int diff_left = 0, int diff_top = 0)
        {
            int left = (int)(__bitmap.Width / 2);
            int top = (int)(__bitmap.Height / 2);
            Color c = __bitmap.GetPixel(left + diff_left, top + diff_top);

            int color_diff = Math.Abs(c.R - R) * Math.Abs(c.R - R) + Math.Abs(c.G - G) * Math.Abs(c.G - G) + Math.Abs(c.B - B) * Math.Abs(c.B - B);
            //Debug.Print("Color Diff : " + color_diff.ToString());
            return (color_diff < 1200);
        }
        public Boolean CheckPixelColor2(Bitmap __bitmap, int R, int G, int B)
        {
            int left = (int)(__bitmap.Width / 2);
            int top = (int)(__bitmap.Height / 2);
            Color c = __bitmap.GetPixel(left, top);

            int color_diff = Math.Abs(c.R - R) * Math.Abs(c.R - R) + Math.Abs(c.G - G) * Math.Abs(c.G - G) + Math.Abs(c.B - B) * Math.Abs(c.B - B);
            //Debug.Print("Color Diff : " + color_diff.ToString());
            if (color_diff < 1200) return true;
            color_diff = 100000;
            if ((c.R > c.G) && (c.G > c.B))
            {
                color_diff = Math.Abs(c.R - c.B * 2) * Math.Abs(c.R - c.B * 2) + Math.Abs(c.G * 2 - c.B * 3) * Math.Abs(c.G * 2 - c.B * 3);
            }
            if (color_diff < 9000) return true;

            return false;
        }

        public Boolean CheckPixelColor(int left, int top, int R, int G, int B)
        {
            if ((left < 0) || (top < 0) || (left > _currentBitmap.Width) || (top > _currentBitmap.Height)) return false;
            Color c = _currentBitmap.GetPixel(left, top);

            int color_diff = Math.Abs(c.R - R) * Math.Abs(c.R - R) + Math.Abs(c.G - G) * Math.Abs(c.G - G) + Math.Abs(c.B - B) * Math.Abs(c.B - B);
            //Debug.Print("Color Diff : " + color_diff.ToString());
            return (color_diff < 2000);
        }

        public void PreProcess_for_OOi()
        {
            Color c;
            int i;

            for (i = 0; i < _currentBitmap.Width; i++)
            {
                c = _currentBitmap.GetPixel(i, _currentBitmap.Height - 2);
                if (c.R + c.G + c.B > 20)
                {
                    _currentBitmap = new Bitmap(1, 1);
                    return;
                }
            }

            int e_pos = 0;

            i = (int)(_currentBitmap.Width / 2);

            for (int j = _currentBitmap.Height - 2; j > 0; j--)
            {
                if (CheckPixelColorOver(i, j, 52, 52, 52) && (e_pos == 0))
                {
                    e_pos = j + 1;
                    break;
                }
            }

            int s_pos = e_pos - 20;

            __tempBitmap = new Bitmap(150, 20);
            for (i = 280; i < __tempBitmap.Width + 200; i++)
            {
                for (int j = 0; j < __tempBitmap.Height; j++)
                {
                    if (CheckPixelColorOver(i, j + s_pos, 130, 130, 130))
                    {
                        c = _currentBitmap.GetPixel(i, j + s_pos);
                        __tempBitmap.SetPixel(i - 280, j, c);
                        //__tempBitmap.SetPixel(i - 100, j, Color.White);
                    }
                    else
                    {
                        __tempBitmap.SetPixel(i - 280, j, Color.Black);
                    }
                }
            }

            _currentBitmap = (Bitmap)__tempBitmap.Clone();
            __tempBitmap.Dispose();
        }

        public void PreProcess_for_Kanazawa()
        {
            Color c;
            int i;

            for (i = 0; i < _currentBitmap.Width; i++)
            {
                c = _currentBitmap.GetPixel(i, _currentBitmap.Height - 2);
                if (c.R + c.G + c.B > 30)
                {
                    _currentBitmap = new Bitmap(1, 1);
                    return;
                }
            }

            int e_pos = 0;

            i = (int)(_currentBitmap.Width / 2);

            for (int j = _currentBitmap.Height - 2; j > 0; j--)
            {
                if (CheckPixelColor(i, j, 15, 17, 141) && (e_pos == 0))
                {
                    e_pos = j + 1;
                    break;
                }
            }

            int s_pos = e_pos - 59;
            int m_pos = e_pos - 45;

            __tempBitmap = new Bitmap(_currentBitmap.Width - 100, 14);
            for (i = 120; i < _currentBitmap.Width; i++)
            {
                for (int j = 0; j < __tempBitmap.Height; j++)
                {
                    if (CheckPixelColorOver(i, j + s_pos, 150, 150, 150))
                    {
                        c = _currentBitmap.GetPixel(i, j + s_pos);
                        __tempBitmap.SetPixel(i - 120, j, c);
                        //__tempBitmap.SetPixel(i - 100, j, Color.White);
                    }
                    else
                    {
                        __tempBitmap.SetPixel(i - 120, j, Color.Black);
                    }
                }
            }

            _currentBitmap = (Bitmap)__tempBitmap.Clone();
            __tempBitmap.Dispose();
        }

        public void PreProcess_for_Sonoda()
        {
            Color c;
            int i;

            for (i = 0; i < _currentBitmap.Width; i++)
            {
                c = _currentBitmap.GetPixel(i, _currentBitmap.Height - 2);
                if (c.R + c.G + c.B > 30)
                {
                    _currentBitmap = new Bitmap(1, 1);
                    return;
                }
            }

            int e_pos = 0;

            i = (int)(_currentBitmap.Width / 2);

            for (int j = _currentBitmap.Height - 2; j > 0; j--)
            {
                if (CheckPixelColor(i, j, 15, 17, 141) && (e_pos == 0))
                {
                    e_pos = j + 1;
                    break;
                }
            }

            int s_pos = e_pos - 59;
            int m_pos = e_pos - 45;

            __tempBitmap = new Bitmap(_currentBitmap.Width - 100, 14);
            for (i = 100; i < _currentBitmap.Width; i++)
            {
                for (int j = 0; j < __tempBitmap.Height; j++)
                {
                    if (CheckPixelColorOver(i, j + s_pos, 150, 150, 150))
                    {
                        c = _currentBitmap.GetPixel(i, j + s_pos);
                        __tempBitmap.SetPixel(i - 100, j, c);
                        //__tempBitmap.SetPixel(i - 100, j, Color.White);
                    }
                    else
                    {
                        __tempBitmap.SetPixel(i - 100, j, Color.Black);
                    }
                }
            }

            _currentBitmap = (Bitmap)__tempBitmap.Clone();
            __tempBitmap.Dispose();
        }

        public void PreProcess_for_Saga()
        {
            Color c;
            int i = 0;

            for (i = 0; i < _currentBitmap.Width; i++)
            {
                c = _currentBitmap.GetPixel(i, _currentBitmap.Height - 2);
                if ((c.R + c.G + c.B > 120))
                {
                    _currentBitmap = new Bitmap(1, 1);
                    return;
                }
            }

            int e_pos = _currentBitmap.Height;
            int s_pos = e_pos - 27;
            int s_width = _currentBitmap.Width - 10;
            int s_left = 10;

            __tempBitmap = new Bitmap(s_width, 25);
            test_color = new Color[__tempBitmap.Width, __tempBitmap.Height];

            for (i = 0; i < __tempBitmap.Width; i++)
            {
                for (int j = 0; j < __tempBitmap.Height; j++)
                {
                    if (CheckPixelColorOver(i + s_left, j + s_pos, 80, 80, 80))
                    {
                        __tempBitmap.SetPixel(i, j, _currentBitmap.GetPixel(i + s_left, j + s_pos));
                        test_color[i, j] = (Color)_currentBitmap.GetPixel(i + s_left, j + s_pos);
                    }
                    else
                    {
                        __tempBitmap.SetPixel(i, j, Color.Black);
                        test_color[i, j] = Color.Black;
                    }
                }
            }

            _currentBitmap = (Bitmap)__tempBitmap.Clone();
            __tempBitmap.Dispose();
        }

        public void PreProcess_for_Kawasaki(int page_type)
        {
            Color c;
            int i = 0;

            for (i = 0; i < _currentBitmap.Width; i++)
            {
                c = _currentBitmap.GetPixel(i, _currentBitmap.Height - 2);
                if ((c.R > 10) || (c.G > 10) || (c.B > 10))
                {
                    _currentBitmap = new Bitmap(1, 1);
                    return;
                }
            }

            int e_pos = 0;

            i = (int)(_currentBitmap.Width / 2);

            for (int j = _currentBitmap.Height - 2; j > 0; j--)
            {
                if (!CheckPixelColor(i, j, 0, 0, 0) && (e_pos == 0))
                {
                    e_pos = j + 1;
                    break;
                }
            }

            int s_height = 43;
            if (page_type == 1) s_height = 64;
            else if (page_type == 5) s_height = 22;

            int s_pos = e_pos - s_height;
            int s_width = 94;
            int s_left = 76;
            if (page_type == 1)
            {
                s_width = 80;
                s_left = 260;
            }

            __tempBitmap = new Bitmap(s_width, s_height);
            for (i = 0; i < __tempBitmap.Width; i++)
            {
                for (int j = 0; j < __tempBitmap.Height; j++)
                {
                    if (CheckPixelColorOver(i + s_left, j + s_pos, 100, 100, 100))
                    {
                        //__tempBitmap.SetPixel(i, j, Color.White);
                        c = _currentBitmap.GetPixel(i + s_left, j + s_pos);
                        __tempBitmap.SetPixel(i, j, c);
                    }
                    else
                    {
                        __tempBitmap.SetPixel(i, j, Color.Black);
                    }
                }
            }

            _currentBitmap = (Bitmap)__tempBitmap.Clone();
            __tempBitmap.Dispose();
        }

        public void PreProcess_for_Nagoya()
        {
            Color c;
            int i = 0;

            for (i = 0; i < _currentBitmap.Width; i++)
            {
                c = _currentBitmap.GetPixel(i, _currentBitmap.Height - 2);
                if ((c.R > 20) || (c.G > 20) || (c.B > 80))
                {
                    _currentBitmap = new Bitmap(1, 1);
                    return;
                }
            }

            int e_pos = 0;

            i = (int)(_currentBitmap.Width / 2);

            for (int j = _currentBitmap.Height - 2; j > 0; j--)
            {
                if (CheckPixelColor(i, j, 140, 140, 140) && (e_pos == 0))
                {
                    e_pos = j + 1;
                    break;
                }
            }

            int s_pos = e_pos - 27;
            int s_width = 100;
            int s_left = 130;

            __tempBitmap = new Bitmap(s_width, 25);
            for (i = 0; i < __tempBitmap.Width; i++)
            {
                for (int j = 0; j < __tempBitmap.Height; j++)
                {
                    if (CheckPixelColorOver(i + s_left, j + s_pos, 200, 200, 200))
                    {
                        __tempBitmap.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        __tempBitmap.SetPixel(i, j, Color.Black);
                    }
                }
            }

            _currentBitmap = (Bitmap)__tempBitmap.Clone();
            __tempBitmap.Dispose();

        }

        public void PreProcess_for_Obihiro(int odds_type)
        {
            Color c;

            for (int i = 0; i < _currentBitmap.Width; i++)
            {
                c = _currentBitmap.GetPixel(i, _currentBitmap.Height - 2);
                if (c.R + c.G + c.B > 10)
                {
                    _currentBitmap = new Bitmap(1, 1);
                    return;
                }
            }



            int s_pos = 0;
            int e_pos = 0;
            bool check_black = true;



            for (int j = _currentBitmap.Height - 2; j > 0; j--)
            {
                check_black = true;
                for (int i = 50; i < 100; i++)
                {
                    if (i >= _currentBitmap.Width) continue;
                    c = _currentBitmap.GetPixel(i, j);
                    if (c.R + c.G + c.B > 10)
                    {
                        check_black = false;
                        break;
                    }
                }
                if (check_black)
                {
                    if (e_pos > 0)
                    {
                        s_pos = j;
                        break;
                    }
                }
                else
                {
                    if (e_pos == 0)
                        e_pos = j;
                }
            }

            __tempBitmap = new Bitmap(_currentBitmap.Width, e_pos - s_pos + 1);
            int white_diff = 0;
            int white_s = 0;
            int white_e = 0;

            for (int i = 0; i < __tempBitmap.Width; i++)
            {
                for (int j = s_pos; j <= e_pos; j++)
                {
                    c = _currentBitmap.GetPixel(i, j);
                    __tempBitmap.SetPixel(i, j - s_pos, c);
                }
            }

            for (int i = 0; i < __tempBitmap.Width; i++)
            {
                check_black = true;
                for (int j = 0; j < __tempBitmap.Height; j++)
                {
                    c = __tempBitmap.GetPixel(i, j);
                    if (c.R + c.G + c.B > 700) check_black = false;
                }
                if (check_black)
                {
                    if (white_s > 0 && white_e == 0)
                    {
                        white_e = i;
                        white_diff = white_e - white_s;
                        if (white_diff > 12)
                        {
                            for (int ii = white_s - 1; ii <= white_e + 1; ii++)
                            {
                                for (int j = 0; j < __tempBitmap.Height; j++)
                                {
                                    c = Color.Black;
                                    __tempBitmap.SetPixel(ii, j, c);
                                }
                            }
                        }
                        white_s = 0;
                        white_e = 0;
                    }
                }
                else
                {
                    if (white_s == 0) white_s = i;
                }
                if (odds_type == 1)
                {
                    if (i > 125 && i < 430)
                    {
                        for (int j = 0; j < __tempBitmap.Height; j++)
                        {
                            c = Color.Black;
                            __tempBitmap.SetPixel(i, j, c);
                        }
                    }
                }
                else if ((odds_type == 2) || (odds_type == 3))
                {
                    if (i > 0 && i < 350)
                    {
                        for (int j = 0; j < __tempBitmap.Height; j++)
                        {
                            c = Color.Black;
                            __tempBitmap.SetPixel(i, j, c);
                        }
                    }
                }
                else if (odds_type == 4)
                {
                    if (i > 205 && i < 485)
                    {
                        for (int j = 0; j < __tempBitmap.Height; j++)
                        {
                            c = Color.Black;
                            __tempBitmap.SetPixel(i, j, c);
                        }
                    }
                }
                else if ((odds_type == 5) || (odds_type == 6))
                {
                    if (i > 115 && i < 445)
                    {
                        for (int j = 0; j < __tempBitmap.Height; j++)
                        {
                            c = Color.Black;
                            __tempBitmap.SetPixel(i, j, c);
                        }
                    }
                }
            }

            _currentBitmap = (Bitmap)__tempBitmap.Clone();
            __tempBitmap.Dispose();
        }

        public int PreProcess_for_Urawa()
        {
            Color c;
            int i;

            for (i = 0; i < _currentBitmap.Width; i++)
            {
                c = _currentBitmap.GetPixel(i, _currentBitmap.Height - 2);
                if (c.R + c.G + c.B > 30)
                {
                    _currentBitmap = new Bitmap(1, 1);
                    return 0;
                }
            }

            int e_pos = 0;

            i = _currentBitmap.Width - 146;

            for (int j = _currentBitmap.Height - 2; j > 0; j--)
            {

                c = _currentBitmap.GetPixel(i, j);
                if ((c.R + c.G + c.B > 70) && (e_pos == 0))
                {
                    e_pos = j + 1;
                    break;
                }
            }
            int s_pos = e_pos - 40;
            int m_pos = e_pos - 20;
            int check_pos1 = e_pos - 30;
            int check_pos2 = e_pos - 10;
            int s_left = 284;
            int s_width = 75;
            int odds_type = 0;

            if (CheckPixelColor(366, check_pos1, 234, 223, 30) && CheckPixelColor(366, check_pos2, 219, 51, 21))
            {
                s_left = 372;
                odds_type = 1;
            }
            else if (CheckPixelColor(235, check_pos1, 19, 25, 242) && CheckPixelColor(235, check_pos2, 232, 56, 10))
            {
                odds_type = 2;
            }
            else if (CheckPixelColor(235, check_pos1, 112, 233, 74) && CheckPixelColor(235, check_pos2, 255, 122, 11))
            {
                odds_type = 3;
            }

            __tempBitmap = new Bitmap(s_width, 40);
            for (i = 0; i < s_width; i++)
            {
                for (int j = 0; j < 40; j++)
                {

                    if (CheckPixelColor(i + s_left, j + s_pos, 55, 55, 55))
                    {
                        __tempBitmap.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        c = _currentBitmap.GetPixel(i + s_left, j + s_pos);
                        __tempBitmap.SetPixel(i, j, c);
                    }

                }
            }

            _currentBitmap = (Bitmap)__tempBitmap.Clone();
            __tempBitmap.Dispose();

            return odds_type;
        }

        public void SetContrast()
        {
            __tempBitmap = (Bitmap)_currentBitmap.Clone();



            Color c;

            int max_val = 0;
            int min_val = 255;
            int row_count = 0;

            for (int i = 0; i < _currentBitmap.Width - 3; i += 3)
            {
                int max_val_row = 0;
                int min_val_row = 255;
                for (int j = 0; j < _currentBitmap.Height - 3; j += 3)
                {
                    int gray = 0;
                    for (int k = 0; k < 3; k++)
                        for (int l = 0; l < 3; l++)
                        {
                            c = _currentBitmap.GetPixel(i + k, j + l);
                            gray += (int)(.299 * c.R + .587 * c.G + .114 * c.B);
                        }
                    gray /= 9;
                    if (gray > max_val) max_val = gray;
                    if (gray < min_val) min_val = gray;
                    if (gray > max_val_row) max_val_row = gray;
                    if (gray < min_val_row) min_val_row = gray;
                }
                if ((max_val_row < 100) && (max_val_row - min_val_row < 20)) row_count++;
            }

            Debug.Print(row_count.ToString());



            if (row_count < 1)
            {
                _currentBitmap = new Bitmap(1, 1);
                __tempBitmap.Dispose();
                return;
            }

            int limit = (int)(max_val * 14 / 20);



            for (int i = 0; i < _currentBitmap.Width; i++)
                for (int j = 0; j < _currentBitmap.Height; j++)
                {
                    c = _currentBitmap.GetPixel(i, j);
                    double gray = (double)(.299 * c.R + .587 * c.G + .114 * c.B);
                    gray = (double)((gray - min_val) * 255 / (max_val - min_val));
                    if (gray > 255) gray = 255;
                    if (gray < 0) gray = 0;
                    _currentBitmap.SetPixel(i, j, Color.FromArgb((byte)gray, (byte)gray, (byte)gray));
                }

            bool check_correct = true;



            int[] check_region = CheckRegion(limit, (int)(_currentBitmap.Width / 3 - 1));
            int s_pos = check_region[0];
            int e_pos = check_region[1];
            //return;
            s_pos += 3;

            if ((s_pos - e_pos > 40) || (s_pos - e_pos < 10) || (s_pos * 2 < _currentBitmap.Height))
            {
                check_correct = false;
            }

            Debug.Print("S and E position: " + s_pos.ToString() + " == " + e_pos.ToString());

            if (check_correct) check_correct = CheckWide(s_pos, e_pos, limit, (int)(_currentBitmap.Width / 2));

            AdjustRegion(check_correct, s_pos, e_pos, limit);

            return;
        }

        public void SetContrast(double contrast)
        {
            Bitmap temp = (Bitmap)_currentBitmap;
            Bitmap bmap = (Bitmap)temp.Clone();
            if (contrast < -100) contrast = -100;
            if (contrast > 100) contrast = 100;
            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    double pR = c.R / 255.0;
                    pR -= 0.5;
                    pR *= contrast;
                    pR += 0.5;
                    pR *= 255;
                    if (pR < 0) pR = 0;
                    if (pR > 255) pR = 255;

                    double pG = c.G / 255.0;
                    pG -= 0.5;
                    pG *= contrast;
                    pG += 0.5;
                    pG *= 255;
                    if (pG < 0) pG = 0;
                    if (pG > 255) pG = 255;

                    double pB = c.B / 255.0;
                    pB -= 0.5;
                    pB *= contrast;
                    pB += 0.5;
                    pB *= 255;
                    if (pB < 0) pB = 0;
                    if (pB > 255) pB = 255;

                    bmap.SetPixel(i, j, Color.FromArgb((byte)pR, (byte)pG, (byte)pB));
                }
            }
            _currentBitmap = (Bitmap)bmap.Clone();
        }

        public void SetGrayscale()
        {
            Bitmap temp = (Bitmap)_currentBitmap;
            Bitmap bmap = (Bitmap)temp.Clone();
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);

                    bmap.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            _currentBitmap = (Bitmap)bmap.Clone();
        }

        public void SetInvert()
        {
            Bitmap temp = (Bitmap)_currentBitmap;
            Bitmap bmap = (Bitmap)temp.Clone();
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    bmap.SetPixel(i, j, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }
            _currentBitmap = (Bitmap)bmap.Clone();
        }

        public void Resize(int newWidth, int newHeight)
        {
            if (newWidth != 0 && newHeight != 0)
            {
                Bitmap temp = (Bitmap)_currentBitmap;
                Bitmap bmap = new Bitmap(newWidth, newHeight, temp.PixelFormat);

                double nWidthFactor = (double)temp.Width / (double)newWidth;
                double nHeightFactor = (double)temp.Height / (double)newHeight;

                double fx, fy, nx, ny;
                int cx, cy, fr_x, fr_y;
                Color color1 = new Color();
                Color color2 = new Color();
                Color color3 = new Color();
                Color color4 = new Color();
                byte nRed, nGreen, nBlue;

                byte bp1, bp2;

                for (int x = 0; x < bmap.Width; ++x)
                {
                    for (int y = 0; y < bmap.Height; ++y)
                    {

                        fr_x = (int)Math.Floor(x * nWidthFactor);
                        fr_y = (int)Math.Floor(y * nHeightFactor);
                        cx = fr_x + 1;
                        if (cx >= temp.Width) cx = fr_x;
                        cy = fr_y + 1;
                        if (cy >= temp.Height) cy = fr_y;
                        fx = x * nWidthFactor - fr_x;
                        fy = y * nHeightFactor - fr_y;
                        nx = 1.0 - fx;
                        ny = 1.0 - fy;

                        color1 = temp.GetPixel(fr_x, fr_y);
                        color2 = temp.GetPixel(cx, fr_y);
                        color3 = temp.GetPixel(fr_x, cy);
                        color4 = temp.GetPixel(cx, cy);

                        // Blue
                        bp1 = (byte)(nx * color1.B + fx * color2.B);

                        bp2 = (byte)(nx * color3.B + fx * color4.B);

                        nBlue = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        // Green
                        bp1 = (byte)(nx * color1.G + fx * color2.G);

                        bp2 = (byte)(nx * color3.G + fx * color4.G);

                        nGreen = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        // Red
                        bp1 = (byte)(nx * color1.R + fx * color2.R);

                        bp2 = (byte)(nx * color3.R + fx * color4.R);

                        nRed = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        bmap.SetPixel(x, y, System.Drawing.Color.FromArgb(255, nRed, nGreen, nBlue));
                    }
                }
                _currentBitmap = (Bitmap)bmap.Clone();
            }
        }

        public void Crop(int xPosition, int yPosition, int width, int height)
        {
            Bitmap temp = (Bitmap)_currentBitmap;
            Bitmap bmap = (Bitmap)temp.Clone();
            if (xPosition + width > _currentBitmap.Width)
                width = _currentBitmap.Width - xPosition;
            if (yPosition + height > _currentBitmap.Height)
                height = _currentBitmap.Height - yPosition;
            Rectangle rect = new Rectangle(xPosition, yPosition, width, height);
            _currentBitmap = (Bitmap)bmap.Clone(rect, bmap.PixelFormat);
        }

    }
}
