using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public class GameScreen : Control
    {
        private Game game;
        private readonly StringFormat nameSF = new StringFormat();
        private readonly StringFormat phraseSF = new StringFormat();
        private MyStates states;
        private int count = 0;
        private Label Hearts { get; set; }
        private Label Keys { get; set; }
        private Image person;
        private string name;
        private string phrase;
        public ScreenForEnterName ScreenForEnterName;

        private int roundingPercent = 3;

        public GameScreen()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;

            ForeColor = ColorTranslator.FromHtml("#1D132B");
            var statesColor = ColorTranslator.FromHtml("#AF89F0");
            Font = new Font("Palatino Linotype", 22, FontStyle.Bold);

            nameSF.Alignment = StringAlignment.Near;
            nameSF.LineAlignment = StringAlignment.Near;

            phraseSF.Alignment = StringAlignment.Near;
            phraseSF.LineAlignment = StringAlignment.Near;

            Hearts = new Label
            {
                BackColor = statesColor,
                Location = new Point(Width / 3 - 15, Height / 4),
                Size = new Size(90, 40)
            };

            Keys = new Label
            {
                BackColor = statesColor,
                Location = new Point(2 * Width / 3 + 25, Height / 4),
                Size = new Size(90, 40)
            };

            ScreenForEnterName = new ScreenForEnterName
            {
                Size = this.Size,
                Location = new Point(0, 0),
            };

            states = new MyStates();
            Controls.Add(ScreenForEnterName);
            Controls.Add(Hearts);
            Controls.Add(Keys);
            ScreenForEnterName.Hide();

            SizeChanged += (sender, args) =>
            {
                states.Location = new Point((ClientSize.Width - states.Size.Width) / 2, (int)(0.5 * states.Size.Height));
                Hearts.Location = new Point(states.Location.X + states.Hearts.Location.X, states.Location.Y + states.Hearts.Location.Y);
                Keys.Location = new Point(states.Location.X + states.Keys.Location.X, states.Location.Y + states.Keys.Location.Y);
                ScreenForEnterName.Location = new Point(0, 0);
                ScreenForEnterName.Size = this.Size;
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            graphics.DrawImage(states.States.Image, new Rectangle(states.Location, states.Size));
            graphics.DrawImage(person, new Rectangle(50, Height - 500, 500, 500));

            var rect = new Rectangle(550, Height - 300, Width - 600, 250);
            var roundingValue = Height / 100F * roundingPercent;
            var rectPath = Rounder.MakeRoundedRectangle(rect, roundingValue);

            graphics.DrawPath(new Pen(ColorTranslator.FromHtml("#503F6E")), rectPath);
            graphics.FillPath(new SolidBrush(ColorTranslator.FromHtml("#DACEED")), rectPath);

            var rectName = new Rectangle(rect.X + 10, rect.Y + 10, rect.Width - 20, 40);
            var rectPhrase = new Rectangle(rectName.X, rectName.Y + 50, rect.Width - 20, rect.Height - rectName.Height - 30);

            graphics.DrawString(name, new Font("Palatino Linotype", 22, FontStyle.Bold), new SolidBrush(ForeColor), rectName, nameSF);
            graphics.DrawString(phrase, new Font("Palatino Linotype", 22), new SolidBrush(ForeColor), rectPhrase, phraseSF);
        }

        public void Configure(Game game)
        {
            if (this.game != null)
            {
                UpdateStates();
                return;
            }

            this.game = game;
            game.NameEntering += ShowEnteringScreen;
            ScreenForEnterName.buttonPlay.Click += GetName;
            BackgroundImage = Loader.LoadImageJPG(game.StoryName, game.CurrentScene.Background);
            person = Loader.LoadImagePNG(game.StoryName, game.CurrentPhrase.Person);
            name = game.CurrentPerson;
            phrase = game.CurrentPhrase.Text;
            Click += GetNextPhrase;
            Hearts.Text = game.Player.Diamonds.ToString();
            Keys.Text = game.Player.Keys.ToString();
        }

        private void GetName(object sender, EventArgs e) => game.SetName(ScreenForEnterName.name.Text);
        private void ShowEnteringScreen() => ScreenForEnterName.Show();
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
                BackgroundImage = Loader.LoadImageJPG(game.StoryName, game.CurrentScene.Background);
                person = Loader.LoadImagePNG(game.StoryName, game.CurrentPhrase.Person);
                name = game.CurrentPerson;
                phrase = game.CurrentPhrase.Text;
                Invalidate();
            }
        }
        private void UpdateStates()
        {
            Hearts.Text = game.Player.Diamonds.ToString();
            Keys.Text = game.Player.Keys.ToString();
            Invalidate();
        }
    }
}
