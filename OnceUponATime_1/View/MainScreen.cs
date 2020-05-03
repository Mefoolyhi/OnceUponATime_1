using System;
using System.Drawing;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public class MainScreen : Control 
    {
        private Game game;
        private MyButton buttonPlay;
        private MyArrowButton leftButton;
        private MyArrowButton rightButton;
        private MyStates states;
        private PictureBox image;
        private MyMessageBox messageNoSerie;
        private MyMessageBox messageYouGetGift;
        private MyMessageBox messageNotSpaceForKeys;
        private MyMessageBox messageNotKeys;
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
            states = new MyStates();
            rightButton = new MyArrowButton(false);
            leftButton = new MyArrowButton(true);

            buttonPlay = new MyButton
            {
                RoundingEnable = true,
                RoundingPercent = 100,
                Size = new Size(280, 80),
                Font = new Font("Palatino Linotype", 32, FontStyle.Bold),
                Text = "Играть"
            };

            messageYouGetGift = new MyMessageBox("С возвращаением!",
                "Вот ваш подарок:\n   +3 ключа\n   +25 алмазов",
                "Получить");

            messageNotSpaceForKeys = new MyMessageBox("Аккуратно!", 
                "В сундуках больше нет места.\nМы оставили только 10 ключей.\nПора освободить место для новых.",
                "Закрыть");

            messageNoSerie = new MyMessageBox("Что было дальше?",
                "Мы сами не знаем! Ждем обновление!",
                "Продолжить");

            messageNotKeys= new MyMessageBox("Аккуратно!",
                "В сундуках больше нет ключей!\nНе время расстраиваться,\nпополняем запасы!",
                "Закрыть");

            image = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point((ClientSize.Width - 1000) / 2, (int)(states.Location.Y + 1.5 * states.Size.Height))
            };

            Controls.Add(states);
            Controls.Add(buttonPlay);
            Controls.Add(rightButton);
            Controls.Add(leftButton);
            Controls.Add(messageYouGetGift);
            Controls.Add(messageNotSpaceForKeys);
            Controls.Add(messageNoSerie);
            Controls.Add(messageNotKeys);
            Controls.Add(image);

            messageYouGetGift.Hide();
            messageNotSpaceForKeys.Hide();
            messageNoSerie.Hide();
            messageNotKeys.Hide();

            SizeChanged += (sender, args) =>
            {
                buttonPlay.Location = new Point((ClientSize.Width - buttonPlay.Size.Width) / 2,
                    (int)(ClientSize.Height - 1.5 * buttonPlay.Size.Height));
                states.Location = new Point((ClientSize.Width - states.Size.Width) / 2,
                    (int)(0.5 * states.Size.Height));
                image.Size = new Size(Width - 650,
                    (int)(Height - states.Height * 2.5 - buttonPlay.Height * 2));
                image.Location = new Point((ClientSize.Width - image.Size.Width) / 2,
                    (int)(states.Location.Y + 1.6 * states.Size.Height));
                messageYouGetGift.Location = new Point((ClientSize.Width - messageNoSerie.Size.Width) / 2,
                    (ClientSize.Height - messageNoSerie.Size.Height) / 2);
                messageNotSpaceForKeys.Location = new Point((ClientSize.Width - messageNoSerie.Size.Width) / 2,
                    (ClientSize.Height - messageNoSerie.Size.Height) / 2);
                messageNoSerie.Location = new Point((ClientSize.Width - messageNoSerie.Size.Width) / 2,
                    (ClientSize.Height - messageNoSerie.Size.Height) / 2);
                messageNotKeys.Location = new Point((ClientSize.Width - messageNoSerie.Size.Width) / 2,
                    (ClientSize.Height - messageNoSerie.Size.Height) / 2);
                rightButton.Location = new Point((int)(Width - rightButton.Size.Width * 1.3),
                    (Height - rightButton.Height) / 2);
                leftButton.Location = new Point((int)(leftButton.Size.Width * 0.3),
                    (Height - leftButton.Height) / 2);
            };
        }

        public void Configure(Game game)
        {
            if (this.game != null)
            {
                UpdateStates();
                return;
            }

            this.game = game;
            image.Image = Loader.LoadImage(game.StoryName, game.StoryName);

            buttonPlay.Click += StartButton_Click;
            rightButton.Button.Click += RightButton_Click;
            leftButton.Button.Click += LeftButton_Click;
            messageNoSerie.MainButton.Click += NoSerieButton_Click;
            messageYouGetGift.MainButton.Click += GetGiftButton_Click;
            messageYouGetGift.ExitButton.Button.Click += GetGiftButton_Click;
            messageNotSpaceForKeys.MainButton.Click += NoSpaceButton_Click;
            messageNotKeys.MainButton.Click += NoKeysButton_Click;
            game.NoSerie += ShowNoSerieMessage;
            game.GetGift += ShowGiftMessage;
            game.NoPlace += ShowNoPlaceMessage;
            game.NoKeys += ShowNoKeysMessage;
            game.StatesUpdeted += UpdateStates;
            states.Hearts.Text = game.Player.Diamonds.ToString();
            states.Keys.Text = game.Player.Keys.ToString();

            // Раскомменть, чтобы скрывать кнопки для перехода к другой истории, если история только одна
            //if(game.StoriesCount < 2)
            //{
            //    leftButton.Hide();
            //    rightButton.Hide();
            //}
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            game.GetNextStory();
            image.Image = Loader.LoadImage(game.StoryName, game.StoryName);
            Invalidate();
        }

        private void LeftButton_Click(object sender, EventArgs e)
        {
            game.GetPreviousStory();
            image.Image = Loader.LoadImage(game.StoryName, game.StoryName);
            Invalidate();
        }

        private void UpdateStates()
        {
            states.Hearts.Text = game.Player.Diamonds.ToString();
            states.Keys.Text = game.Player.Keys.ToString();
            Invalidate();
        }

        private void ShowNoPlaceMessage() => messageNotSpaceForKeys.Show();
        private void ShowGiftMessage() => messageYouGetGift.Show();
        private void ShowNoSerieMessage() => messageNoSerie.Show();
        private void ShowNoKeysMessage() => messageNotKeys.Show();
        private void NoSerieButton_Click(object sender, EventArgs e) => messageNoSerie.Hide();
        private void GetGiftButton_Click(object sender, EventArgs e)
        {
            messageYouGetGift.Hide();
            UpdateStates();
        }
        private void NoSpaceButton_Click(object sender, EventArgs e) => messageNotSpaceForKeys.Hide();
        private void NoKeysButton_Click(object sender, EventArgs e) => messageNotKeys.Hide();
        private void StartButton_Click(object sender, EventArgs e) => game.PlayGame();
    }
}
