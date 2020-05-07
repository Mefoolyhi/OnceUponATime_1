using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public sealed class MyMessageBox : UserControl
    {
        private readonly StringFormat _headSf = new StringFormat();
        private readonly StringFormat _mainTextSf = new StringFormat();
        public readonly MyButton MainButton;
        public readonly MyControlButton ExitButton;
        private readonly string _mainText;
        private readonly Color _strokeColor;

        public MyMessageBox(string labelText, string mainText, string buttonText)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;

            Size = new Size(600, 300);
            BackColor = ColorTranslator.FromHtml("#AF9BCF");
            ForeColor = ColorTranslator.FromHtml("#1D132B");
            _strokeColor = ColorTranslator.FromHtml("#503F6E");
            Font = new Font("Palatino Linotype", 22, FontStyle.Bold);
            Text = labelText;
            _mainText = mainText;

            _headSf.Alignment = StringAlignment.Center;
            _headSf.LineAlignment = StringAlignment.Near;

            _mainTextSf.Alignment = StringAlignment.Near;
            _mainTextSf.LineAlignment = StringAlignment.Near;

            MainButton = new MyButton
            {
                Text = buttonText,
                RoundingEnable = true,
                RoundingPercent = 100,
                Location = new Point((Size.Width - 220) / 2, (int)(Size.Height - 1.3 * 60))
            };
            Controls.Add(MainButton);

            ExitButton = new MyControlButton(Loader.LoadImagePng("game images", "exit"))
            {
                Size = new Size(30, 30),
                Location = new Point(Size.Width - 32, 2)
            };
            Controls.Add(ExitButton);
            ExitButton.Click += ExitButton_Click;
        }

        public MyMessageBox(string labelText, string mainText)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                     | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.ResizeRedraw
                     | ControlStyles.SupportsTransparentBackColor
                     | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;

            Size = new Size(600, 300);
            BackColor = ColorTranslator.FromHtml("#AF9BCF");
            ForeColor = ColorTranslator.FromHtml("#1D132B");
            _strokeColor = ColorTranslator.FromHtml("#503F6E");
            Font = new Font("Palatino Linotype", 22, FontStyle.Bold);
            Text = labelText;
            _mainText = mainText;

            _headSf.Alignment = StringAlignment.Center;
            _headSf.LineAlignment = StringAlignment.Near;

            _mainTextSf.Alignment = StringAlignment.Near;
            _mainTextSf.LineAlignment = StringAlignment.Near;
        }

        private void ExitButton_Click(object sender, EventArgs e) => Hide();

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Parent.BackColor);

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            graphics.DrawRectangle(new Pen(_strokeColor), rect);
            graphics.FillRectangle(new SolidBrush(BackColor), rect);

            var indent = 10;
            var rectHead = new Rectangle(4 * indent, indent, Width - 8 * indent, 40);
            var rectText = new Rectangle(2 * indent, rectHead.Y + rectHead.Height + 2 * indent,
                Width - 4 * indent, Height - rectHead.Height - 2 * indent);
            if (!(MainButton is null))
                rectText = new Rectangle(2 * indent, rectHead.Y + rectHead.Height + 2 * indent,
                Width - 4 * indent, Height - rectHead.Height - MainButton.Size.Height - 2 * indent);

            graphics.DrawString(Text, Font, new SolidBrush(ForeColor), rectHead, _headSf);
            graphics.DrawString(_mainText, new Font("Palatino Linotype", 22), new SolidBrush(ForeColor), rectText, _mainTextSf);
        }
    }
}
