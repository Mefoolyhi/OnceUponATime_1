using System;
using System.Drawing;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    class MyForm : Form
    {
        private Game game;
        private LoadScreen loadScreen;
        private MainScreen mainScreen;
        private GameScreen gameScreen;
        private EndScreen endScreen;
        private MyExitButton ExitButton;

        public MyForm()
        {
            game = new Game();
            Name = "Once upon a time";
            Text = Name;
            BackColor = ColorTranslator.FromHtml("#DACEED");
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            Icon = Loader.LoadIcon("game images", "heart");

            ExitButton = new MyExitButton
            {
                Size = new Size(30, 30),
                Location = new Point(Size.Width - 31, 1),
            };

            loadScreen = new LoadScreen
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = this.Size
            };

            gameScreen = new GameScreen()
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = this.Size
            };

            mainScreen = new MainScreen
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = this.Size
            };

            endScreen = new EndScreen
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = this.Size
            };

            Controls.Add(ExitButton);
            Controls.Add(loadScreen);
            Controls.Add(gameScreen);
            Controls.Add(mainScreen);
            Controls.Add(endScreen);

            SizeChanged += (sender, args) =>
            {
                ExitButton.Location = new Point(Size.Width - ExitButton.Size.Width - 1, 1);
            };

            ExitButton.Click += ExitButton_Click;
            game.StageChanged += Game_OnStageChanged;
            ShowLoadScreen();

            Shown += (Object sender, EventArgs e) => game.Loading();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            game.End(); 
            this.Close();
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
            ExitButton.Hide();
            loadScreen.Configure(game);
            loadScreen.Show();
        }

        private void ShowGameScreen()
        {
            HideScreens();
            ExitButton.Hide();
            gameScreen.Configure(game);
            gameScreen.Show();
        }

        private void ShowEndScreen()
        {
            HideScreens();
            ExitButton.Hide();
            endScreen.Configure(game);
            endScreen.Show();
        }

        private void ShowMainScreen()
        {
            HideScreens();
            ExitButton.Show();
            mainScreen.Configure(game);
            mainScreen.Show();
        }

        private void HideScreens()
        {
            mainScreen.Hide();
            loadScreen.Hide();
            gameScreen.Hide();
            endScreen.Hide();
        }
    }
}
