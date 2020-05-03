using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public class MyExitButton : Control
    {
        public PictureBox Button;
        public MyExitButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;

            Size = new Size(30, 30);
            Button = new PictureBox
            {
                Size = this.Size,
                Image = Loader.LoadImage("game images", "exit"),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Controls.Add(Button);
        }
    }
}
