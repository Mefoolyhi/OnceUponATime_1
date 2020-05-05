using System;
using System.Drawing;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public sealed class MainScreen : Control 
    {
        private Game _game;
        private readonly MyButton _buttonPlay;
        private readonly MyArrowButton _leftButton;
        private readonly MyArrowButton _rightButton;
        private readonly MyStates _states;
        private readonly PictureBox _image;
        private readonly MyMessageBox _messageNoSerie;
        private readonly MyMessageBox _messageYouGetGift;
        private readonly MyMessageBox _messageNotSpaceForKeys;
        private readonly MyMessageBox _messageNotKeys;
        public MainScreen()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;

            BackColor = ColorTranslator.FromHtml("#DACEED");
            _states = new MyStates();
            _rightButton = new MyArrowButton(false);
            _leftButton = new MyArrowButton(true);

            _buttonPlay = new MyButton
            {
                RoundingEnable = true,
                RoundingPercent = 100,
                Size = new Size(280, 80),
                Font = new Font("Palatino Linotype", 32, FontStyle.Bold),
                Text = "Играть"
            };

            _messageYouGetGift = new MyMessageBox("С возвращаением!",
                "Вот ваш подарок:\n   +3 ключа\n   +25 алмазов",
                "Получить");

            _messageNotSpaceForKeys = new MyMessageBox("Аккуратно!", 
                "В сундуках больше нет места.\nМы оставили только 10 ключей.\nПора освободить место для новых.",
                "Закрыть");

            _messageNoSerie = new MyMessageBox("Что было дальше?",
                "Мы сами не знаем! Ждем обновление!",
                "Продолжить");

            _messageNotKeys= new MyMessageBox("Аккуратно!",
                "В сундуках больше нет ключей!\nНе время расстраиваться,\nпополняем запасы!",
                "Закрыть");

            _image = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point((ClientSize.Width - 1000) / 2, (int)(_states.Location.Y + 1.5 * _states.Size.Height))
            };

            Controls.Add(_states);
            Controls.Add(_buttonPlay);
            Controls.Add(_rightButton);
            Controls.Add(_leftButton);
            Controls.Add(_messageYouGetGift);
            Controls.Add(_messageNotSpaceForKeys);
            Controls.Add(_messageNoSerie);
            Controls.Add(_messageNotKeys);
            Controls.Add(_image);

            _messageYouGetGift.Hide();
            _messageNotSpaceForKeys.Hide();
            _messageNoSerie.Hide();
            _messageNotKeys.Hide();

            SizeChanged += (sender, args) =>
            {
                _buttonPlay.Location = new Point((ClientSize.Width - _buttonPlay.Size.Width) / 2,
                    (int)(ClientSize.Height - 1.5 * _buttonPlay.Size.Height));
                _states.Location = new Point((ClientSize.Width - _states.Size.Width) / 2,
                    (int)(0.5 * _states.Size.Height));
                _image.Size = new Size(Width - 650,
                    (int)(Height - _states.Height * 2.5 - _buttonPlay.Height * 2));
                _image.Location = new Point((ClientSize.Width - _image.Size.Width) / 2,
                    (int)(_states.Location.Y + 1.6 * _states.Size.Height));
                _messageYouGetGift.Location = new Point((ClientSize.Width - _messageNoSerie.Size.Width) / 2,
                    (ClientSize.Height - _messageNoSerie.Size.Height) / 2);
                _messageNotSpaceForKeys.Location = new Point((ClientSize.Width - _messageNoSerie.Size.Width) / 2,
                    (ClientSize.Height - _messageNoSerie.Size.Height) / 2);
                _messageNoSerie.Location = new Point((ClientSize.Width - _messageNoSerie.Size.Width) / 2,
                    (ClientSize.Height - _messageNoSerie.Size.Height) / 2);
                _messageNotKeys.Location = new Point((ClientSize.Width - _messageNoSerie.Size.Width) / 2,
                    (ClientSize.Height - _messageNoSerie.Size.Height) / 2);
                _rightButton.Location = new Point((int)(Width - _rightButton.Size.Width * 1.3),
                    (Height - _rightButton.Height) / 2);
                _leftButton.Location = new Point((int)(_leftButton.Size.Width * 0.3),
                    (Height - _leftButton.Height) / 2);
            };
        }

        public void Configure(Game game)
        {
            if (_game != null)
            {
                UpdateStates();
                return;
            }

            _game = game;
            _image.Image = Loader.LoadImagePng(game.StoryName, game.StoryName);

            _buttonPlay.Click += StartButton_Click;
            _rightButton.Click += RightButton_Click;
            _leftButton.Click += LeftButton_Click;
            _messageNoSerie.MainButton.Click += NoSerieButton_Click;
            _messageYouGetGift.MainButton.Click += GetGiftButton_Click;
            _messageYouGetGift.ExitButton.Click += GetGiftButton_Click;
            _messageNotSpaceForKeys.MainButton.Click += NoSpaceButton_Click;
            _messageNotKeys.MainButton.Click += NoKeysButton_Click;
            game.NoSerie += ShowNoSerieMessage;
            game.GetGift += ShowGiftMessage;
            game.NoPlace += ShowNoPlaceMessage;
            game.NoKeys += ShowNoKeysMessage;
            game.StatesUpdated += UpdateStates;
            _states.Hearts.Text = game.Player.Diamonds.ToString();
            _states.Keys.Text = game.Player.Keys.ToString();


            if (game.StoriesNumber >= 2) return;
            _leftButton.Hide();
            _rightButton.Hide();
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            _game.GetNextStory();
            _image.Image = Loader.LoadImagePng(_game.StoryName, _game.StoryName);
            Invalidate();
        }

        private void LeftButton_Click(object sender, EventArgs e)
        {
            _game.GetPreviousStory();
            _image.Image = Loader.LoadImagePng(_game.StoryName, _game.StoryName);
            Invalidate();
        }

        private void UpdateStates()
        {
            _states.Hearts.Text = _game.Player.Diamonds.ToString();
            _states.Keys.Text = _game.Player.Keys.ToString();
            Invalidate();
        }

        private void ShowNoPlaceMessage() => _messageNotSpaceForKeys.Show();
        private void ShowGiftMessage() => _messageYouGetGift.Show();
        private void ShowNoSerieMessage() => _messageNoSerie.Show();
        private void ShowNoKeysMessage() => _messageNotKeys.Show();
        private void NoSerieButton_Click(object sender, EventArgs e) => _messageNoSerie.Hide();
        private void GetGiftButton_Click(object sender, EventArgs e)
        {
            _messageYouGetGift.Hide();
            UpdateStates();
        }
        private void NoSpaceButton_Click(object sender, EventArgs e) => _messageNotSpaceForKeys.Hide();
        private void NoKeysButton_Click(object sender, EventArgs e) => _messageNotKeys.Hide();
        private void StartButton_Click(object sender, EventArgs e) => _game.PlayGame();
    }
}
