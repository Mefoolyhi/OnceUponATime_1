using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    class MyStates : Control
    {
        public Label Hearts { get; set; }
        public Label Keys { get; set; }
        public PictureBox States { get; }
        private Color statesColor;

        public MyStates()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;

            Size = new Size(430, 90);
            ForeColor = ColorTranslator.FromHtml("#1D132B");
            statesColor = ColorTranslator.FromHtml("#AF89F0");
            Font = new Font("Palatino Linotype", 22, FontStyle.Bold);

            Hearts = new Label
            {
                BackColor = statesColor,
                Location = new Point(Width / 3 - 15, Height / 4),
                Size = new Size (90, 40)
            };

            Keys = new Label
            {
                BackColor = statesColor,
                Location = new Point(2 * Width / 3 + 25, Height / 4),
                Size = new Size(90, 40)
            };

            States = new PictureBox
            {
                Size = this.Size,
                Image = Loader.LoadImagePNG("game images", "states"),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            Controls.Add(Hearts);
            Controls.Add(Keys);
            Controls.Add(States);
        }
    }
}