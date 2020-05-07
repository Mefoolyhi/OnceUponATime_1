using System.Drawing;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public sealed class MyMenu : UserControl
    {
        public readonly MyButton FirstButton;
        public readonly MyButton SecondButton;
        public readonly MyButton ThirdButton;

        public MyMenu(string first, string second, string third)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;
            Size = new Size(500, 450);

            FirstButton = new MyButton
            {
                Size = new Size(Width, Height /3),
                Font = new Font("Palatino Linotype", 32, FontStyle.Bold),
                Text = first,
                Location = new Point(0, 0)
        };

            SecondButton = new MyButton
            {
                Size = new Size(Width, Height / 3),
                Font = new Font("Palatino Linotype", 32, FontStyle.Bold),
                Text = second,
                Location = new Point(0, Height / 3)
            };

            ThirdButton = new MyButton
            {
                Size = new Size(Width, Height / 3),
                Font = new Font("Palatino Linotype", 32, FontStyle.Bold),
                Text = third,
                Location = new Point(0, 2 * Height / 3)
            };

            Controls.Add(FirstButton);
            Controls.Add(SecondButton);
            Controls.Add(ThirdButton);
        }
    }
}
