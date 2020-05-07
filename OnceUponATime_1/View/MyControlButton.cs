using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public sealed class MyControlButton : UserControl
    {
        private readonly PictureBox _button;
        private bool _mouseEntered;
        private bool _mousePressed;
        public MyControlButton(Image image)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;

            Size = new Size(30, 30);
            _button = new PictureBox
            {
                Size = Size,
                Image = image,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Parent.BackColor);

            var rect = new Rectangle(0, 0, Width, Height);
            graphics.DrawImage(_button.Image, rect);

            if (_mouseEntered)
            {
                graphics.DrawRectangle(new Pen(Color.FromArgb(60, ColorTranslator.FromHtml("#8C64BF"))), rect);
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, ColorTranslator.FromHtml("#8C64BF"))), rect);
            }

            if (_mousePressed)
            {
                graphics.DrawRectangle(new Pen(Color.FromArgb(60, Color.Black)), rect);
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, Color.Black)), rect);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _mouseEntered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _mouseEntered = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _mousePressed = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _mousePressed = false;
            Invalidate();
        }
    }
}
