using System.Drawing;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public class MyMenu : Control
    {
        public MyButton Continue;
        public MyButton Restart;
        public MyButton Exit;

        public MyMenu()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;
            Size = new Size(500, 450);

            Continue = new MyButton
            {
                Size = new Size(Width, Height /3),
                Font = new Font("Palatino Linotype", 32, FontStyle.Bold),
                Text = "Продолжить",
                Location = new Point(0, 0)
        };

            Restart = new MyButton
            {
                Size = new Size(Width, Height / 3),
                Font = new Font("Palatino Linotype", 32, FontStyle.Bold),
                Text = "Пройти серию заново",
                Location = new Point(0, Height / 3)
            };

            Exit = new MyButton
            {
                Size = new Size(Width, Height / 3),
                Font = new Font("Palatino Linotype", 32, FontStyle.Bold),
                Text = "Выйти",
                Location = new Point(0, 2 * Height / 3)
            };

            Controls.Add(Continue);
            Controls.Add(Restart);
            Controls.Add(Exit);
        }
    }
}
