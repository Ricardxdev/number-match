using Terminal.Gui;

namespace NumberMatchGame
{
    public class Number
    {
        public int X;
        public int Y;
        public int Value;
        public bool IsSelected;
        public bool IsMatched;

        public Number(int x, int y, int value)
        {
            X = x;
            Y = y;
            Value = value;
        }
    }
    public class NumberButton : Button
    {
        public Number Number { get; private set; }

        public NumberButton(Number number) : base(number.Value.ToString())
        {
            Number = number;
            Text = number.Value.ToString();
            X = number.X * 5;
            Y = number.Y * 2;
            Width = 4;
            Height = 1;
        }

        public void Refresh(int x, int y) {
            Number.X = x;
            Number.Y = y;
            X = x * 5;
            Y = y * 2;

            this.SetNeedsDisplay();
        }
    }
}