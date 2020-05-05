using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public sealed class GameScreen : Control
    {
        private Game _game;
        private readonly StringFormat _nameSf = new StringFormat();
        private readonly StringFormat _phraseSf = new StringFormat();
        private readonly MyStates _states;
        private int _count;
        private Label Hearts { get; }
        private Label Keys { get; }
        private Image _person;
        private string _name;
        private string _phrase;
        private readonly ScreenForEnterName _screenForEnterName;
        private readonly PictureBox _menuButton;
        private readonly MyMenu _menu;

        private readonly int _roundingPercent = 3;

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

            _nameSf.Alignment = StringAlignment.Near;
            _nameSf.LineAlignment = StringAlignment.Near;

            _phraseSf.Alignment = StringAlignment.Near;
            _phraseSf.LineAlignment = StringAlignment.Near;

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

            _screenForEnterName = new ScreenForEnterName
            {
                Size = Size,
                Location = new Point(0, 0)
            };

            _menuButton = new PictureBox
            {
                Image = Loader.LoadImagePng("game images", "menu"),
                Size = new Size(40, 40),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            _menu = new MyMenu();
            _states = new MyStates();
            Controls.Add(_screenForEnterName);
            Controls.Add(Hearts);
            Controls.Add(Keys);
            Controls.Add(_menuButton);
            Controls.Add(_menu);
            _screenForEnterName.Hide();
            _menu.Hide();

            SizeChanged += (sender, args) =>
            {
                _states.Location = new Point((ClientSize.Width - _states.Size.Width) / 2, (int)(0.5 * _states.Size.Height));
                Hearts.Location = new Point(_states.Location.X + _states.Hearts.Location.X, _states.Location.Y + _states.Hearts.Location.Y);
                Keys.Location = new Point(_states.Location.X + _states.Keys.Location.X, _states.Location.Y + _states.Keys.Location.Y);
                _screenForEnterName.Location = new Point(0, 0);
                _screenForEnterName.Size = Size;
                _menuButton.Location = new Point(20, 20);
                _menu.Location = new Point((Width - _menu.Width) / 2, (Height - _menu.Height) / 2);
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            var rectImage = new Rectangle(50, Height - 500, 500, 500);
            graphics.DrawImage(_states.States.Image, new Rectangle(_states.Location, _states.Size));
            graphics.DrawImage(_person, rectImage);

            var rect = new Rectangle(rectImage.X + rectImage.Width, Height - 300, Width - rectImage.X - rectImage.Width - 50, 250);
            var roundingValue = Height / 100F * _roundingPercent;
            var rectPath = Rounder.MakeRoundedRectangle(rect, roundingValue);

            graphics.DrawPath(new Pen(ColorTranslator.FromHtml("#503F6E")), rectPath);
            graphics.FillPath(new SolidBrush(ColorTranslator.FromHtml("#DACEED")), rectPath);

            var rectName = new Rectangle(rect.X + 10, rect.Y + 10, rect.Width - 20, 40);
            var rectPhrase = new Rectangle(rectName.X, rectName.Y + 50, rect.Width - 20, rect.Height - rectName.Height - 30);

            graphics.DrawString(_name, new Font("Palatino Linotype", 24, FontStyle.Bold), new SolidBrush(ForeColor), rectName, _nameSf);
            graphics.DrawString(_phrase, new Font("Palatino Linotype", 22), new SolidBrush(ForeColor), rectPhrase, _phraseSf);
        }

        public void Configure(Game game)
        {
            if (_game != null)
            {
                UpdateStates();
                return;
            }

            _game = game;
            game.NameEntering += ShowEnteringScreen;
            _screenForEnterName.ButtonPlay.Click += GetName;
            _menuButton.Click += ShowMenu;
            _menu.Continue.Click += Continue;
            _menu.Restart.Click += Restart;
            _menu.Exit.Click += Exit;
            BackgroundImage = Loader.LoadImageJpg(game.StoryName, game.CurrentScene.Background);
            _person = Loader.LoadImagePng(game.StoryName, game.CurrentPhrase.Person);
            _name = game.CurrentPerson;
            _phrase = game.CurrentPhrase.Text;
            Click += GetNextPhrase;
            Hearts.Text = game.Player.Diamonds.ToString();
            Keys.Text = game.Player.Keys.ToString();
        }

        private void Exit(object sender, EventArgs e)
        {
            _game.ExitFromSerie();
            _menu.Hide();
        }

        private void Restart(object sender, EventArgs e)
        {
            _game.RestartSerie();
            _menu.Hide();
        }

        private void Continue(object sender, EventArgs e) => _menu.Hide();
        private void ShowMenu(object sender, EventArgs e) => _menu.Show();
        private void GetName(object sender, EventArgs e) => _game.SetName(_screenForEnterName.Name.Text);
        private void ShowEnteringScreen() => _screenForEnterName.Show();
        private void GetNextPhrase(object sender, EventArgs e)
        {
            if (_count == 1)
            {
                _game.EndSerie();
            }
            else
            {
                _count++;
                _game.GetNextPhrase();
                BackgroundImage = Loader.LoadImageJpg(_game.StoryName, _game.CurrentScene.Background);
                _person = Loader.LoadImagePng(_game.StoryName, _game.CurrentPhrase.Person);
                _name = _game.CurrentPerson;
                _phrase = _game.CurrentPhrase.Text;
                Invalidate();
            }
        }
        private void UpdateStates()
        {
            Hearts.Text = _game.Player.Diamonds.ToString();
            Keys.Text = _game.Player.Keys.ToString();
            BackgroundImage = Loader.LoadImageJpg(_game.StoryName, _game.CurrentScene.Background);
            _person = Loader.LoadImagePng(_game.StoryName, _game.CurrentPhrase.Person);
            _name = _game.CurrentPerson;
            _phrase = _game.CurrentPhrase.Text;
            _count = 0;
            Invalidate();
        }
    }
}
