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
        private PictureBox Button;
        private bool mouseEntered = false;
        private bool mousePressed = false;
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
                    Image = Loader.LoadImagePNG("game images", "left"),
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
            }
            else
            {
                Button = new PictureBox
                {
                    Size = this.Size,
                    Image = Loader.LoadImagePNG("game images", "right"),
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Parent.BackColor);

            var rect = new Rectangle(0, 0, Width, Height);
            graphics.DrawImage(Button.Image, rect);

            if (mouseEntered)
            {
                graphics.DrawRectangle(new Pen(Color.FromArgb(60, ColorTranslator.FromHtml("#8C64BF"))), rect);
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, ColorTranslator.FromHtml("#8C64BF"))), rect);
            }

            if (mousePressed)
            {
                graphics.DrawRectangle(new Pen(Color.FromArgb(60, Color.Black)), rect);
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, Color.Black)), rect);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            mouseEntered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            mouseEntered = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mousePressed = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mousePressed = false;
            Invalidate();
        }
    }
}
