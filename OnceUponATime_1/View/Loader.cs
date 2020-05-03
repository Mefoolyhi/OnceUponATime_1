using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace OnceUponATime_1
{
    public class Loader
    {
        public static Image LoadImage(string dirName, string pictureName)
        {
            try
            {
                var image = Image.FromFile($@"images\{dirName}\{pictureName}.png");
                return image;
            }
            catch
            {
                throw new FileNotFoundException($@"В каталоге images не удалось найти файл {dirName}\{pictureName}.png");
            }
        }

        public static Image LoadImage2(string dirName, string pictureName)
        {
            try
            {
                var image = Image.FromFile($@"images\{dirName}\{pictureName}");
                return image;
            }
            catch
            {
                throw new FileNotFoundException($@"В каталоге images не удалось найти файл {dirName}\{pictureName}");
            }
        }

        public static Icon LoadIcon(string dirName, string pictureName)
        {
            try
            {
                return new Icon($@"images\{dirName}\{pictureName}.ico"); ;
            }
            catch
            {
                throw new FileNotFoundException($@"В каталоге images не удалось найти файл {dirName}\{pictureName}.icon");
            }
        }

        public static FontFamily LoadFont(string dirName, string fontName)
        {
            try
            {
                PrivateFontCollection fontCollection = new PrivateFontCollection();
                fontCollection.AddFontFile($@"{dirName}\{fontName}.otf"); // файл шрифта
                return fontCollection.Families[0]; 
            }
            catch
            {
                throw new FileNotFoundException($@"Не удалось найти файл {dirName}\{fontName}.otf");
            }
        }
    }
}
