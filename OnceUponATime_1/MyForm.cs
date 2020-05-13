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
        private readonly MyControlButton _menuButton;
        private readonly MyMenu _menu;

        public MyForm()
        {
            _game = new Game();
            Name = "Once upon a time";
            Text = Name;
            BackColor = ColorTranslator.FromHtml("#DACEED");
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            Icon = Loader.LoadIcon("game images", "heart");

            _menuButton = new MyControlButton(Loader.LoadImagePng("game images", "menu"))
            {
                Size = new Size(40, 40),
            };

            _menu = new MyMenu("Продолжить", "Скрыть игру", "Выйти");

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

            Controls.Add(_menu);
            Controls.Add(_menuButton);
            Controls.Add(_loadScreen);
            Controls.Add(_gameScreen);
            Controls.Add(_mainScreen);
            Controls.Add(_endScreen);
            _menu.Hide();

            SizeChanged += (sender, args) =>
            {
                _menuButton.Location = new Point(Width - _menuButton.Width - 20, 20);
                _menu.Location = new Point((Width - _menu.Width) / 2, (Height - _menu.Height) / 2);
            };

            _menuButton.Click += MenuButton_Click;
            _menu.FirstButton.Click += ContinueButton_Click;
            _menu.SecondButton.Click += HideButton_Click;
            _menu.ThirdButton.Click += ExitButton_Click;
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

        private void MenuButton_Click(object sender, EventArgs e) => _menu.Show();
        private void ContinueButton_Click(object sender, EventArgs e) => _menu.Hide();
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
            _menuButton.Show();
            _loadScreen.Configure(_game);
            _loadScreen.Show();
        }

        private void ShowGameScreen()
        {
            HideScreens();
            _menuButton.Hide();
            _gameScreen.Configure(_game);
            _gameScreen.Show();
        }

        private void ShowEndScreen()
        {
            HideScreens();
            _menuButton.Hide();
            _endScreen.Configure(_game);
            _endScreen.Show();
        }

        private void ShowMainScreen()
        {
            HideScreens();
            _menuButton.Show();
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
