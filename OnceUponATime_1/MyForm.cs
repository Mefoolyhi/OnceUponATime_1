using System;
using System.Drawing;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    class MyForm : Form
    {
        private readonly Game _game;
        private readonly LoadScreen _loadScreen;
        private readonly MainScreen _mainScreen;
        private readonly GameScreen _gameScreen;
        private readonly EndScreen _endScreen;
        private readonly MyControlButton _exitButton;
        private readonly MyControlButton _hideButton;

        public MyForm()
        {
            _game = new Game();
            Name = "Once upon a time";
            Text = Name;
            BackColor = ColorTranslator.FromHtml("#DACEED");
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            Icon = Loader.LoadIcon("game images", "heart");

            _exitButton = new MyControlButton(Loader.LoadImagePng("game images", "exit"))
            {
                Size = new Size(30, 30),
                Location = new Point(Size.Width - 31, 1)
            };

            _hideButton = new MyControlButton(Loader.LoadImagePng("game images", "hide"))
            {
                Size = new Size(30, 30),
                Location = new Point(Size.Width - 2 * _exitButton.Width - 1, 1)
            };

            _loadScreen = new LoadScreen
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = Size
            };

            _gameScreen = new GameScreen
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = Size
            };

            _mainScreen = new MainScreen
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = Size
            };

            _endScreen = new EndScreen
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = Size
            };

            Controls.Add(_hideButton);
            Controls.Add(_exitButton);
            Controls.Add(_loadScreen);
            Controls.Add(_gameScreen);
            Controls.Add(_mainScreen);
            Controls.Add(_endScreen);

            SizeChanged += (sender, args) =>
            {
                _exitButton.Location = new Point(Size.Width - _exitButton.Size.Width - 1, 1);
                _hideButton.Location = new Point(Size.Width - 2 * _exitButton.Width - 1, 1);
            };

            _exitButton.Click += ExitButton_Click;
            _hideButton.Click += HideButton_Click;
            _game.StageChanged += Game_OnStageChanged;
            ShowLoadScreen();

            Shown += (sender, e) => _game.Loading();
        }

        public sealed override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private void HideButton_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        private void ExitButton_Click(object sender, EventArgs e)
        {
            _game.End(); 
            Close();
        }

        private void Game_OnStageChanged(GameStage stage)
        {
            switch (stage)
            {
                case GameStage.Game:
                    ShowGameScreen();
                    break;
                case GameStage.Main:
                    ShowMainScreen();
                    break;
                case GameStage.Finished:
                    ShowEndScreen();
                    break;
                default:
                    ShowLoadScreen();
                    break;
            }
        }

        private void ShowLoadScreen()
        {
            HideScreens();
            _exitButton.Show();
            _exitButton.Show();
            _loadScreen.Configure(_game);
            _loadScreen.Show();
        }

        private void ShowGameScreen()
        {
            HideScreens();
            _exitButton.Hide();
            _exitButton.Hide();
            _gameScreen.Configure(_game);
            _gameScreen.Show();
        }

        private void ShowEndScreen()
        {
            HideScreens();
            _exitButton.Hide();
            _exitButton.Hide();
            _endScreen.Configure(_game);
            _endScreen.Show();
        }

        private void ShowMainScreen()
        {
            HideScreens();
            _exitButton.Show();
            _exitButton.Show();
            _mainScreen.Configure(_game);
            _mainScreen.Show();
        }

        private void HideScreens()
        {
            _mainScreen.Hide();
            _loadScreen.Hide();
            _gameScreen.Hide();
            _endScreen.Hide();
        }
    }
}
