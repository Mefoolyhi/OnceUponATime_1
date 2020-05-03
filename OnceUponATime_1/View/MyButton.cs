using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace OnceUponATime_1
{
    public class MyButton : Control
    {
        private readonly StringFormat SF = new StringFormat();
        private bool mouseEntered = false;
        private bool mousePressed = false;

        private bool roundingEnable = false;
        public bool RoundingEnable
        {
            get => roundingEnable;
            set
            {
                roundingEnable = value;
                Refresh();
            }
        }

        private int roundingPercent = 100;
        public int RoundingPercent
        {
            get => roundingPercent;
            set
            {
                if (value >= 0 && value <= 100)
                {
                    roundingPercent = value;
                    Refresh();
                }
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
            SF.Alignment = StringAlignment.Center;
            SF.LineAlignment = StringAlignment.Center;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Parent.BackColor);

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            var roundingValue = 0.1F;
            if (RoundingEnable && roundingPercent > 0)
            {
                roundingValue = Height / 100F * roundingPercent;
            }

            var rectPath = Rounder.MakeRoundedRectangle(rect, roundingValue);

            graphics.DrawPath(new Pen(BackColor), rectPath);
            graphics.FillPath(new SolidBrush(BackColor), rectPath);

            if (mouseEntered)
            {
                graphics.DrawPath(new Pen(Color.FromArgb(60, Color.White)), rectPath);
                graphics.FillPath(new SolidBrush(Color.FromArgb(60, Color.White)), rectPath);
            }

            if (mousePressed)
            {
                graphics.DrawPath(new Pen(BackColor), rectPath);
                graphics.FillPath(new SolidBrush(BackColor), rectPath);
            }

            graphics.DrawString(Text, Font, new SolidBrush(ForeColor), rect, SF);
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
