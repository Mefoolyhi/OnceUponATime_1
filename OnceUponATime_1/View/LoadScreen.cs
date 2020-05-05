using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public class LoadScreen : Control
    {
        private Game game;
        private Font additionalFont;
        public LoadScreen()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.UserPaint,
                true);
            DoubleBuffered = true;
            BackColor = ColorTranslator.FromHtml("#DACEED");
            ForeColor = ColorTranslator.FromHtml("#1D132B");
            Font = new Font(Loader.LoadFont("fonts", "GreatVibes-Regular"), 120);
            additionalFont = new Font("Palatino Linotype", 18);

            var image = new PictureBox
            {
                Image = Loader.LoadImagePNG("game images", "flowers"),
                Size = new Size(Width, 250),
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            
            var gameName = new Label
            {
                Text = "Once upon a time:" +
                " Interactive stories",
            };

            var additionalText = new Label
            {
                Text = "                    " +
                "Персонажи вымышленные.\nЕсли узнали себя или знакомых - это случайность :)",
                Font = additionalFont,
            };

            Controls.Add(additionalText);
            Controls.Add(gameName);
            Controls.Add(image);

            SizeChanged += (sender, args) =>
            {
                image.Size = new Size(Width, 250);
                image.Location = new Point(0, 0);
                gameName.Location = new Point(280, image.Height - 30);
                gameName.Size = new Size(Width - 500, Height - 200);
                additionalText.Location = new Point(Width / 2 - 320, Height - 100);
                additionalText.Size = new Size(Width / 2, 100);
            };
        }

        public void Configure(Game game)
        {
            if (this.game != null)
                return;

            this.game = game;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
        }
    }
}
