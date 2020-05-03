using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public class GameScreen : Control
    {
        private Game game;
        private MyStates states;
        private int count = 0;
        public GameScreen()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;

            BackColor = ColorTranslator.FromHtml("#FFFFFF");
            ForeColor = ColorTranslator.FromHtml("#1D132B");
            Font = new Font("Palatino Linotype", 18, FontStyle.Bold);

            states = new MyStates();
            Controls.Add(states);

            SizeChanged += (sender, args) =>
            {
                states.Location = new Point((ClientSize.Width - states.Size.Width) / 2, (int)(0.5 * states.Size.Height));
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            graphics.DrawString("Game", Font, new SolidBrush(ForeColor), new Point(2 * Width / 3 + 10, Height / 4));
        }

        public void Configure(Game game)
        {
            if (this.game != null)
            {
                UpdateStates();
                return;
            }

            this.game = game;
            BackgroundImage = Loader.LoadImage(game.StoryName, game.CurrentPerson);
            Click += GetNextPhrase;
            states.Hearts.Text = game.Player.Diamonds.ToString();
            states.Keys.Text = game.Player.Keys.ToString();
        }

        private void GetNextPhrase(object sender, EventArgs e)
        {
            if (count == 1)
            {
                game.EndSerie();
            }
            else
            {
                count++;
                game.GetNextPhrase();
                BackgroundImage = Loader.LoadImage(game.StoryName, game.CurrentPerson);
                Invalidate();
            }
        }
        private void UpdateStates()
        {
            states.Hearts.Text = game.Player.Diamonds.ToString();
            states.Keys.Text = game.Player.Keys.ToString();
            Invalidate();
        }
    }
}
