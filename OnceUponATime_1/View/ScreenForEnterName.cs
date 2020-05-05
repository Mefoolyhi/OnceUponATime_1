using System;
using System.Drawing;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public class ScreenForEnterName : Control
    {
        public TextBox name;
        private Label label;
        public MyButton buttonPlay;
        public ScreenForEnterName()
        {
            BackColor = ColorTranslator.FromHtml("#DACEED");

            label = new Label
            {
                Text = "Введите свое имя:",
                Size = new Size(Width / 2, 60),
                Font = new Font("Palatino Linotype", 22),
                Location = new Point(3 * Width/ 4, Height / 2 - 60),
            };

            name = new TextBox
            {
                Size = new Size(Width / 2, 60),
                Font = new Font("Palatino Linotype", 22, FontStyle.Bold),
                Location = new Point(3 * Width / 4, Height / 2),
            };

            buttonPlay = new MyButton
            {
                RoundingEnable = true,
                RoundingPercent = 100,
                Size = new Size(280, 80),
                Font = new Font("Palatino Linotype", 32, FontStyle.Bold),
                Text = "Играть"
            };

            Controls.Add(label);
            Controls.Add(name);
            Controls.Add(buttonPlay);
            buttonPlay.Click += (object sender, EventArgs e) => this.Hide();

            SizeChanged += (sender, args) =>
            {
                label.Size = new Size(Width / 2, 60);
                label.Location = new Point(Width / 4, Height / 2 - 60);
                name.Size = new Size(Width / 2, 60);
                name.Location = new Point(Width / 4, Height / 2);
                buttonPlay.Location = new Point((Width - buttonPlay.Width) / 2, Height / 2 + 60);
            };
        }
    }
}
