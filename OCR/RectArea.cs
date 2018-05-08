namespace OCR
{
    class RectArea
    {
        public int left;
        public int top;
        public int bottom;
        public int right;
        public RectArea()
        {
            left = top = bottom = right = 0;
        }
        public RectArea(int x, int y)
        {
            left = right = x;
            top = bottom = y;
        }
        public void reCalcArea(int x, int y)
        {
            if (left > x)
            {
                left = x;
            }
            if (right < x)
            {
                right = x;
            }
            if (top > y)
            {
                top = y;
            }
            if (bottom < y)
            {
                bottom = y;
            }
        }
        public int Width()
        {
            return right - left;
        }
        public int Height()
        {
            return bottom - top;
        }
    }
}