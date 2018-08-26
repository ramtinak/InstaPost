using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace CoreLib.Helpers
{
    class ImageHelper
    {
        public static string ConvertToJPEG(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            try
            {
                var newPath = Path.Combine(Helper.CacheTempPath, Helper.RandomString()) + ".jpg";
                var img = Image.FromFile(path);
                using (var b = new Bitmap(img.Width, img.Height))
                {
                    b.SetResolution(img.HorizontalResolution, img.VerticalResolution);

                    using (var g = Graphics.FromImage(b))
                    {
                        g.Clear(Color.White);
                        g.DrawImageUnscaled(img, 0, 0);
                    }
                    System.Threading.Thread.Sleep(100);
                    b.Save(newPath, ImageFormat.Jpeg);
                    newPath.Output();
                    CachedHelper.AddFile(newPath);
                    return newPath;
                }
            }
            catch { }
            return path;
        }
    }
}
