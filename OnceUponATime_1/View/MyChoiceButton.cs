using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public sealed class MyChoiceButton : UserControl
    {
        private readonly StringFormat _sfText = new StringFormat();
        private readonly StringFormat _sfPrice = new StringFormat();
        private readonly string _choiceText;
        private readonly int _price;
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

        public MyChoiceButton(string text, int price)
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
            _choiceText = text;
            _price = price;
            _sfText.Alignment = StringAlignment.Near;
            _sfText.LineAlignment = StringAlignment.Center;
            _sfPrice.Alignment = StringAlignment.Far;
            _sfPrice.LineAlignment = StringAlignment.Center;
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

            var indent = 10;
            var textRect = new Rectangle(rect.X + indent, rect.Y, (int)((rect.Width - indent) * 0.8), rect.Height);
            var priceRect = new Rectangle(rect.X + textRect.Width + indent, rect.Y, (int)((rect.Width - indent) * 0.1), rect.Height);
            graphics.DrawString(_choiceText, Font, new SolidBrush(ForeColor), textRect, _sfText);
            if (_price != 0)
            {
                graphics.DrawString(_price.ToString(), Font, new SolidBrush(ForeColor), priceRect, _sfPrice);
                graphics.DrawImage(Loader.LoadImagePng("game images", "heart"),
                    rect.X + textRect.Width + priceRect.Width + indent, (rect.Height - 38) / 2, 40, 38);
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
