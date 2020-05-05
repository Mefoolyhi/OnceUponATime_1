using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public sealed class MyButton : Control
    {
        private readonly StringFormat _sf = new StringFormat();
        private bool _mouseEntered;
        private bool _mousePressed;

        private bool _roundingEnable;
        public bool RoundingEnable
        {
            get => _roundingEnable;
            set
            {
                _roundingEnable = value;
                Refresh();
            }
        }

        private int _roundingPercent = 100;
        public int RoundingPercent
        {
            set
            {
                if (value < 0 || value > 100) return;
                _roundingPercent = value;
                Refresh();
            }
        }

        public MyButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;

            Size = new Size(220, 60);
            BackColor = ColorTranslator.FromHtml("#8C64BF");
            ForeColor = ColorTranslator.FromHtml("#1D132B");
            Font = new Font("Palatino Linotype", 22, FontStyle.Bold);
            _sf.Alignment = StringAlignment.Center;
            _sf.LineAlignment = StringAlignment.Center;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Parent.BackColor);

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            var roundingValue = 0.1F;
            if (RoundingEnable && _roundingPercent > 0)
            {
                roundingValue = Height / 100F * _roundingPercent;
            }

            var rectPath = Rounder.MakeRoundedRectangle(rect, roundingValue);

            graphics.DrawPath(new Pen(BackColor), rectPath);
            graphics.FillPath(new SolidBrush(BackColor), rectPath);

            if (_mouseEntered)
            {
                graphics.DrawPath(new Pen(Color.FromArgb(60, Color.White)), rectPath);
                graphics.FillPath(new SolidBrush(Color.FromArgb(60, Color.White)), rectPath);
            }

            if (_mousePressed)
            {
                graphics.DrawPath(new Pen(BackColor), rectPath);
                graphics.FillPath(new SolidBrush(BackColor), rectPath);
            }

            graphics.DrawString(Text, Font, new SolidBrush(ForeColor), rect, _sf);
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
