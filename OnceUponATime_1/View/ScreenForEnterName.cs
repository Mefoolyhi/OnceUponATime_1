using System.Drawing;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public sealed class ScreenForEnterName : Control
    {
        public new readonly TextBox Name;
        public readonly MyButton ButtonPlay;
        public ScreenForEnterName()
        {
            BackColor = ColorTranslator.FromHtml("#DACEED");

            var label = new Label
            {
                Text = "Введите свое имя:",
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
                label.Location = new Point(Width / 4, Height / 2 - 60);
                Name.Size = new Size(Width / 2, 60);
                Name.Location = new Point(Width / 4, Height / 2);
                ButtonPlay.Location = new Point((Width - ButtonPlay.Width) / 2, Height / 2 + 60);
            };
        }
    }
}
