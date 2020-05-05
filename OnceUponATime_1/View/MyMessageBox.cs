using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public sealed class MyMessageBox : Control
    {
        private readonly StringFormat _headSf = new StringFormat();
        private readonly StringFormat _mainTextSf = new StringFormat();
        public readonly MyButton MainButton;
        public readonly MyExitButton ExitButton;
        private readonly string _mainText;
        private readonly Color _strokeColor;

        private bool _roundingEnable;

        private bool RoundingEnable
        {
            get => _roundingEnable;
            set
            {
                _roundingEnable = value;
                Refresh();
            }
        }
        private int _roundingPercent;
        public int RoundingPercent
        {
            get => _roundingPercent;
            set
            {
                if (value < 0 || value > 100) return;
                _roundingPercent = value;
                Refresh();
            }
        }


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
            this._mainText = mainText;

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

            ExitButton = new MyExitButton
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
            this._mainText = mainText;

            _headSf.Alignment = StringAlignment.Center;
            _headSf.LineAlignment = StringAlignment.Near;

            _mainTextSf.Alignment = StringAlignment.Near;
            _mainTextSf.LineAlignment = StringAlignment.Near;

            var buttonYes = new MyButton
            {
                Text = "Yes",
                RoundingEnable = true,
                RoundingPercent = 100,
                Size = new Size(150, 60),
                Location = new Point((Size.Width - 320) / 4, (int)(Size.Height - 1.3 * 60))
            };
            Controls.Add(buttonYes);

            var buttonNo = new MyButton
            {
                Text = "No",
                RoundingEnable = true,
                RoundingPercent = 100,
                Size = new Size(150, 60),
                Location = new Point((Size.Width + 160) / 2, (int)(Size.Height - 1.3 * 60))
            };
            Controls.Add(buttonNo);

            ExitButton = new MyExitButton
            {
                Size = new Size(30, 30),
                Location = new Point(Size.Width - 32, 2)
            };
            Controls.Add(ExitButton);
            ExitButton.Click += ExitButton_Click;
        }

        private void ExitButton_Click(object sender, EventArgs e) => Hide();

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

            graphics.DrawPath(new Pen(_strokeColor), rectPath);
            graphics.FillPath(new SolidBrush(BackColor), rectPath);

            var rect1 = new Rectangle(40, 10, Width - 80, Height - 250);
            var rect2 = new Rectangle(20, 70, Width - 40, Height - 160);

            graphics.DrawString(Text, Font, new SolidBrush(ForeColor), rect1, _headSf);
            graphics.DrawString(_mainText, new Font("Palatino Linotype", 22), new SolidBrush(ForeColor), rect2, _mainTextSf);

        }
    }
}
