using System.Drawing;
using System.Drawing.Drawing2D;

namespace OnceUponATime_1
{
    public static class Rounder
    {
        public static GraphicsPath MakeRoundedRectangle(Rectangle rect, float roundSize)
        {
            GraphicsPath gp = new GraphicsPath();

            gp.AddArc(rect.X, rect.Y, roundSize, roundSize, 180, 90);
            gp.AddArc(rect.X + rect.Width - roundSize, rect.Y, roundSize, roundSize, 270, 90);
            gp.AddArc(rect.X + rect.Width - roundSize, rect.Y + rect.Height - roundSize, roundSize, roundSize, 0, 90);
            gp.AddArc(rect.X, rect.Y + rect.Height - roundSize, roundSize, roundSize, 90, 90);

            gp.CloseFigure();

            return gp;
        }
    }
}
