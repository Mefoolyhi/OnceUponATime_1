using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public class MyArrowButton : Control
    {
        public PictureBox Button;
        public MyArrowButton(bool isReverse)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;

            Size = new Size(150, 160);
            if (isReverse)
            {
                Button = new PictureBox
                {
                    Size = this.Size,
                    Image = Loader.LoadImage("game images", "left"),
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
            }
            else
            {
                Button = new PictureBox
                {
                    Size = this.Size,
                    Image = Loader.LoadImage("game images", "right"),
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
            }
            Controls.Add(Button);
        }
    }
}
