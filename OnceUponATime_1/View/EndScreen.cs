using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public class EndScreen : Control
    {
        private Game game;
        private MyButton continueButton;
        private MyStates states;
        private Label text;
        private Label numbers;
        private PictureBox heart;
        private Rectangle rect;
        private Color rectColor;
        private int roundingPercent = 3;
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
            rectColor = ColorTranslator.FromHtml("#C5ACEF");
            Font = new Font("Palatino Linotype", 40, FontStyle.Bold);
            states = new MyStates();

            continueButton = new MyButton
            {
                Size = new Size(280, 80),
                RoundingEnable = true,
                RoundingPercent = 100,
                Text = "Продолжить",
                Font = new Font("Palatino Linotype", 26, FontStyle.Bold),
            };

            rect = new Rectangle(200, states.Size.Height * 2, Width - 400,
                Height - states.Size.Height * 2 - continueButton.Size.Height * 2);

            text = new Label
            {
                Text = "Интуиция:\n\nЛогика:\n\nНаграда за прохождение серии:",
                Font = new Font("Palatino Linotype", 40),
                BackColor = rectColor
            };

            numbers = new Label
            {
                Font = this.Font,
                BackColor = rectColor
            };
            
            heart = new PictureBox
            {
                Image = Loader.LoadImagePNG("game images", "heart"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = rectColor
            };

            Controls.Add(continueButton);
            Controls.Add(states);
            Controls.Add(text);
            Controls.Add(heart);
            Controls.Add(numbers);

            SizeChanged += (sender, args) =>
            {
                continueButton.Location = new Point((ClientSize.Width - continueButton.Size.Width) / 2,
                    (int)(ClientSize.Height - 1.5 * continueButton.Size.Height));
                states.Location = new Point((ClientSize.Width - states.Size.Width) / 2, (int)(0.5 * states.Size.Height));
                rect = new Rectangle(200, states.Size.Height * 2, Width - 400,
                    Height - states.Size.Height * 2 - continueButton.Size.Height * 2);
                text.Location = new Point(rect.X + 20, rect.Y + 20);
                text.Size = new Size(3 * rect.Width / 5, (int)(rect.Height * 0.9));
                numbers.Size = new Size(3 * rect.Width / 10, (int)(rect.Height * 0.9));
                numbers.Location = new Point(text.Location.X + text.Width + 20, rect.Y + 20);
                heart.Size = new Size(65, 65);
                heart.Location = new Point(numbers.Location.X + numbers.Size.Width / 4, numbers.Location.Y + numbers.Width - 40);
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Parent.BackColor);

            var roundingValue = 0.1F;
            if (roundingPercent > 0)
            {
                roundingValue = Height / 100F * roundingPercent;
            }

            var rectPath = Rounder.MakeRoundedRectangle(rect, roundingValue);

            graphics.DrawPath(new Pen(rectColor), rectPath);
            graphics.FillPath(new SolidBrush(rectColor), rectPath);
        }
        public void Configure(Game game)
        {
            if (this.game != null)
            {
                UpdateStates();
                return;
            }

            this.game = game;

            continueButton.Click += EndButton_Click;
            states.Hearts.Text = game.Player.Diamonds.ToString();
            states.Keys.Text = game.Player.Keys.ToString();
            numbers.Text = $"{game.Story.Hero.Logic} + {game.LogicDelta}\n\n{game.Story.Hero.Intuition} + {game.IntuitionDelta}\n\n+5";
        }
        private void EndButton_Click(object sender, EventArgs e)
        {
            game.ReturnToMainScreen();
        }
        private void UpdateStates()
        {
            numbers.Text = $"{game.Story.Hero.Logic} + {game.LogicDelta}\n\n{game.Story.Hero.Intuition} + {game.IntuitionDelta}\n\n+25";
            states.Hearts.Text = game.Player.Diamonds.ToString();
            states.Keys.Text = game.Player.Keys.ToString();
            Invalidate();
        }
    }   
}
