using System;
using System.Data;

namespace ibanapp
{
    public class CursorPosition
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public CursorPosition(int left, int top)
        {
            Left = left;
            Top = top;
        }

        public void UpdatePosition(int left, int top)
        {
            Left = left;
            Top = top;
        }
    }
}
