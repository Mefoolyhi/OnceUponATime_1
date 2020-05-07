using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public class EndScreen : UserControl
    {
        private Game _game;
        private readonly MyButton _continueButton;
        private readonly MyStates _states;
        private readonly Label _text;
        private readonly Label _numbers;
        private readonly PictureBox _heart;
        private Rectangle _rect;
        private readonly Color _rectColor;
        private readonly int _roundingPercent = 3;
        public EndScreen()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;

            BackColor = ColorTranslator.FromHtml("#DACEED");
            ForeColor = ColorTranslator.FromHtml("#1D132B");
            _rectColor = ColorTranslator.FromHtml("#C5ACEF");
            Font = new Font("Palatino Linotype", 40, FontStyle.Bold);
            _states = new MyStates();

            _continueButton = new MyButton
            {
                Size = new Size(280, 80),
                RoundingEnable = true,
                RoundingPercent = 100,
                Text = "Продолжить",
                Font = new Font("Palatino Linotype", 26, FontStyle.Bold)
            };

            _rect = new Rectangle(200, _states.Size.Height * 2, Width - 400,
                Height - _states.Size.Height * 2 - _continueButton.Size.Height * 2);

            _text = new Label
            {
                Text = "Интуиция:\n\nЛогика:\n\nНаграда за прохождение серии:",
                Font = new Font("Palatino Linotype", 40),
                BackColor = _rectColor
            };

            _numbers = new Label
            {
                Font = Font,
                BackColor = _rectColor
            };
            
            _heart = new PictureBox
            {
                Image = Loader.LoadImagePng("game images", "heart"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = _rectColor
            };

            Controls.Add(_continueButton);
            Controls.Add(_states);
            Controls.Add(_text);
            Controls.Add(_heart);
            Controls.Add(_numbers);

            SizeChanged += (sender, args) =>
            {
                _continueButton.Location = new Point((ClientSize.Width - _continueButton.Size.Width) / 2,
                    (int)(ClientSize.Height - 1.5 * _continueButton.Size.Height));
                _states.Location = new Point((ClientSize.Width - _states.Size.Width) / 2, (int)(0.5 * _states.Size.Height));
                _rect = new Rectangle(200, _states.Size.Height * 2, Width - 400,
                    Height - _states.Size.Height * 2 - _continueButton.Size.Height * 2);
                _text.Location = new Point(_rect.X + 20, _rect.Y + 20);
                _text.Size = new Size(3 * _rect.Width / 5, (int)(_rect.Height * 0.9));
                _numbers.Size = new Size(3 * _rect.Width / 10, (int)(_rect.Height * 0.9));
                _numbers.Location = new Point(_text.Location.X + _text.Width + 20, _rect.Y + 20);
                _heart.Size = new Size(65, 65);
                _heart.Location = new Point(_numbers.Location.X + _numbers.Size.Width / 4, _numbers.Location.Y + _numbers.Width - 40);
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Parent.BackColor);

            var roundingValue = Height / 100F * _roundingPercent;

            var rectPath = Rounder.MakeRoundedRectangle(_rect, roundingValue);
            graphics.DrawPath(new Pen(_rectColor), rectPath);
            graphics.FillPath(new SolidBrush(_rectColor), rectPath);
        }
        public void Configure(Game game)
        {
            if (_game != null)
            {
                UpdateStates();
                return;
            }

            _game = game;
            _continueButton.Click += EndButton_Click;
            UpdateStates();
        }
        private void EndButton_Click(object sender, EventArgs e) => _game.ReturnToMainScreen();

        private void UpdateStates()
        {
            _numbers.Text = $"{_game.Story.Hero.Logic} + {_game.LogicDelta}\n\n{_game.Story.Hero.Intuition} + {_game.IntuitionDelta}\n\n+5";
            _states.Diamonds.Text = _game.Player.Diamonds.ToString();
            _states.Keys.Text = _game.Player.Keys.ToString();
            Invalidate();
        }
    }   
}
