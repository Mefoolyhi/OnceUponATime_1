using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public sealed class GameScreen : UserControl
    {
        private Game _game;
        private readonly StringFormat _nameSf = new StringFormat();
        private readonly StringFormat _phraseSf = new StringFormat();
        private readonly MyStates _states;
        private Label Diamonds { get; }
        private Label Keys { get; }
        private Image _person;
        private string _name;
        private string _phrase;
        private MyChoiceButton[] _choices;
        private bool _isPhrase;
        private readonly ScreenForEnterName _screenForEnterName;
        private readonly MyControlButton _menuButton;
        private readonly MyMenu _menu;
        private readonly MyMessageBox _messageNoDiamonds;
        private readonly MyMessageBox _messageLogicSchene;
        private readonly MyMessageBox _messageIntuitionalSchene;
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

            Diamonds = new Label
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

            _menuButton = new MyControlButton(Loader.LoadImagePng("game images", "menu"))
            {
                Size = new Size(40, 40),
            };

            _messageNoDiamonds = new MyMessageBox("Упс...", "Кажется, у вас недостаточно алмазов для этого выбора");
            _messageNoDiamonds.Size = new Size(400, 250);

            _messageLogicSchene = new MyMessageBox("Путь логики", "");
            _messageLogicSchene.Size = new Size(400, 60);

            _messageIntuitionalSchene = new MyMessageBox("Путь интуиции", "");
            _messageIntuitionalSchene.Size = new Size(400, 60);

            _menu = new MyMenu("Продолжить", "Пройти серию заново", "Выйти");
            _states = new MyStates();
            Controls.Add(_screenForEnterName);
            Controls.Add(Diamonds);
            Controls.Add(Keys);
            Controls.Add(_menuButton);
            Controls.Add(_menu);
            Controls.Add(_messageNoDiamonds);
            Controls.Add(_messageLogicSchene);
            Controls.Add(_messageIntuitionalSchene);
            _screenForEnterName.Hide();
            _menu.Hide();
            _messageNoDiamonds.Hide();
            _messageLogicSchene.Hide();
            _messageIntuitionalSchene.Hide();

            SizeChanged += (sender, args) =>
            {
                _states.Location = new Point((ClientSize.Width - _states.Size.Width) / 2, (int)(0.5 * _states.Size.Height));
                Diamonds.Location = new Point(_states.Location.X + _states.Diamonds.Location.X,
                    _states.Location.Y + _states.Diamonds.Location.Y);
                Keys.Location = new Point(_states.Location.X + _states.Keys.Location.X,
                    _states.Location.Y + _states.Keys.Location.Y);
                _screenForEnterName.Location = new Point(0, 0);
                _screenForEnterName.Size = Size;
                _menuButton.Location = new Point(Width - _menuButton.Width - 20, 20);
                _menu.Location = new Point((Width - _menu.Width) / 2, (Height - _menu.Height) / 2);
                _messageNoDiamonds.Location = new Point((ClientSize.Width - _messageNoDiamonds.Size.Width) / 2,
                    (ClientSize.Height - _messageNoDiamonds.Size.Height) / 4);
                _messageLogicSchene.Location = new Point((ClientSize.Width - _messageLogicSchene.Size.Width) / 2,
                    (ClientSize.Height - _messageLogicSchene.Size.Height) / 4);
                _messageIntuitionalSchene.Location = new Point((ClientSize.Width - _messageIntuitionalSchene.Size.Width) / 2,
                    (ClientSize.Height - _messageIntuitionalSchene.Size.Height) / 4);
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            var rectImage = new Rectangle(Width - 550, Height - 600, 500, 650);
            graphics.DrawImage(_states.States.Image, new Rectangle(_states.Location, _states.Size));
            if (!(_person is null))
                graphics.DrawImage(_person, rectImage);
            else
                rectImage.Width = 0;

            var indent = 10;
            var rectForPhrase = new Rectangle(50, Height - 300, Width - rectImage.Width - 100, 250);
            var roundingValue = Height / 100F * _roundingPercent;
            var rectPathPhrase = Rounder.MakeRoundedRectangle(rectForPhrase, roundingValue);

            graphics.DrawPath(new Pen(ColorTranslator.FromHtml("#503F6E")), rectPathPhrase);
            graphics.FillPath(new SolidBrush(ColorTranslator.FromHtml("#DACEED")), rectPathPhrase);

            var rectName = new Rectangle(rectForPhrase.X + indent, rectForPhrase.Y + indent, rectForPhrase.Width - 2 * indent, 40);
            if (_person is null)
                rectName.Height = 0;
            var rectPhrase = new Rectangle(rectName.X, rectName.Y + rectName.Height + indent,
                rectForPhrase.Width - 2 * indent, rectForPhrase.Height - rectName.Height - 3 * indent);

            graphics.DrawString(_name, new Font("Palatino Linotype", 24, FontStyle.Bold),
                new SolidBrush(ForeColor), rectName, _nameSf);
            if (_isPhrase)
                graphics.DrawString(_phrase, new Font("Palatino Linotype", 22),
                    new SolidBrush(ForeColor), rectPhrase, _phraseSf);
            else
            {
                for (var i = 0; i < _choices.Length; i++)
                {
                    _choices[i].Size = new Size(rectForPhrase.Width - 2 * indent,
                        ((rectForPhrase.Height - rectName.Height - indent * (_choices.Length + 2)) / _choices.Length));
                    _choices[i].Location = new Point(rectForPhrase.X + indent,
                        rectForPhrase.Y + (indent + rectName.Height + (i + 1) * indent + i * _choices[i].Size.Height));
                }
            }
        }

        public void Configure(Game game)
        {
            if (_game != null)
            {
                GetNextPhrase(new object(), new EventArgs());
                return;
            }

            _game = game;
            GetNextPhrase(new object(), new EventArgs());
            Click += GetNext;
            _game.NameEntering += ShowEnteringScreen;
            _game.NoDiamonds += ShowMessageNoDiamonds;
            _game.SceneIsLogic += ShowMessageLogicShene;
            _game.SceneIsIntuitional += ShowMessageIntuitionalSchene;
            _screenForEnterName.ButtonPlay.Click += GetName;
            _menuButton.Click += ShowMenu;
            _menu.FirstButton.Click += Continue;
            _menu.SecondButton.Click += Restart;
            _menu.ThirdButton.Click += Exit;
            Diamonds.Text = game.Player.Diamonds.ToString();
            Keys.Text = game.Player.Keys.ToString();
        }

        private void ShowMessageIntuitionalSchene() => _messageIntuitionalSchene.Show();
        private void ShowMessageLogicShene() => _messageLogicSchene.Show();
        private void ShowMessageNoDiamonds() => _messageNoDiamonds.Show();
        private void GetNext(object sender, EventArgs e)
        {
            if (_isPhrase)
                GetNextPhrase(new object(), new EventArgs());
        }
        private void Exit(object sender, EventArgs e)
        {
            DeleteChoices();
            _game.ExitFromSerie();
            _menu.Hide();
        }

        private void Restart(object sender, EventArgs e)
        {
            DeleteChoices();
            _game.RestartSerie();
            _menu.Hide();
        }

        private void Continue(object sender, EventArgs e) => _menu.Hide();
        private void ShowMenu(object sender, EventArgs e) => _menu.Show();
        private void GetName(object sender, EventArgs e) => _game.SetName(_screenForEnterName.Name.Text);
        private void ShowEnteringScreen() => _screenForEnterName.Show();
        private void GetNextPhrase(object sender, EventArgs e)
        {
            _messageLogicSchene.Hide();
            _messageIntuitionalSchene.Hide();
            var next = _game.GetNext();
            BackgroundImage = Loader.LoadImageJpg(_game.StoryName, _game.CurrentScene.Background);
            if (next is Phrase)
            {
                _isPhrase = true;
                DeleteChoices();
                Phrase nextPhrase = (Phrase)next;
                _name = _game.DecodeName(nextPhrase.Person);
                if (nextPhrase.Person is "MainHero")
                    _person = Loader.LoadImagePng(_game.StoryName, nextPhrase.Person);
                else if (nextPhrase.Person is "")
                    _person = null;
                else
                    _person = Loader.LoadImagePng(_game.StoryName, _name);
                _phrase = _game.DecodeName(nextPhrase.Text);
            }
            else if (next is List<Choice>)
            {
                _isPhrase = false;
                List<Choice> nextPhrase = (List<Choice>)next;
                _person = Loader.LoadImagePng(_game.StoryName, "MainHero");
                _name = _game.DecodeName("MainHero");
                _choices = nextPhrase
                    .Select(x => new MyChoiceButton(x.Text, x.DiamondDelta) { RoundingEnable = true, RoundingPercent = 15 })
                    .ToArray();
                foreach (var c in _choices)
                {
                    Controls.Add(c);
                    c.Click += (object sender, EventArgs e) =>
                    {
                        _messageNoDiamonds.Hide();
                        if (_game.UpdateChoiceSuccess(Array.IndexOf(_choices, c)))
                            GetNextPhrase(new object(), new EventArgs());
                    };
                }
            }
            Diamonds.Text = _game.Player.Diamonds.ToString();
            Keys.Text = _game.Player.Keys.ToString();
            Invalidate();
        }

        private void DeleteChoices()
        {
            if (_choices != null)
                foreach (var c in _choices)
                    Controls.Remove(c);
            _choices = null;
        }
    }
}
