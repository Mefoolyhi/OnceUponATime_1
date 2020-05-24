using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public sealed class ScreenForEnterName : UserControl
    {
        public new readonly TextBox Name;
        public readonly MyButton ButtonPlay;
        public string StoryName;
        public ScreenForEnterName()
        {
            BackColor = ColorTranslator.FromHtml("#DACEED");

            var label = new Label
            {
                Text = "Введите имя главной героини:",
                Size = new Size(Width / 2, 60),
                Font = new Font("Palatino Linotype", 22),
                Location = new Point(3 * Width/ 4, Height / 2 - 60)
            };

            Name = new TextBox
            {
                Size = new Size(Width / 2, 60),
                Font = new Font("Palatino Linotype", 22, FontStyle.Bold),
                Location = new Point(3 * Width / 4, Height / 2)
            };

            ButtonPlay = new MyButton
            {
                RoundingEnable = true,
                RoundingPercent = 100,
                Size = new Size(280, 80),
                Font = new Font("Palatino Linotype", 32, FontStyle.Bold),
                Text = "Играть"
            };

            Controls.Add(label);
            Controls.Add(Name);
            Controls.Add(ButtonPlay);
            ButtonPlay.Click += (sender, e) => Hide();

            SizeChanged += (sender, args) =>
            {
                label.Size = new Size(Width / 2, 60);
                label.Location = new Point(Width / 4 + 250, Height / 2 - 60);
                Name.Size = new Size(Width / 2, 60);
                Name.Location = new Point(Width / 4 + 250, Height / 2);
                ButtonPlay.Location = new Point(Width / 4 + 250, Height / 2 + 60);
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            var rect = new Rectangle(Width / 4 - 250, Height / 2 - 350, 500, 650);
            var person = Loader.LoadImagePng(StoryName, "MainHero");
            graphics.DrawImage(person, rect);
        }
    }
}
