using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
namespace CoreLib.Helpers
{
    class FileSelectorHelper
    {
        public static string[] LatestSelected = null;
        public static string[] SelectFiles()
        {
            //All files (*.jpg, *.jpeg, *.bmp, *.png, *.mp4, *.mov) | *.jpg; *.jpeg; *.bmp; *.png; *.mp4; *.mov; | 
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "All files (*.jpg, *.jpeg, *.bmp, *.png, *.mp4, *.mov) | *.jpg; *.jpeg; *.bmp; *.png; *.mp4; *.mov; |Image files (*.jpg, *.jpeg, *.bmp, *.png) | *.jpg; *.jpeg; *.bmp; *.png; |Video files (*.mp4, *.mov) | *.mp4; *.mov;",
                Multiselect = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.FileNames.Length > 9)
                {
                    "Only 9 files can select".ShowMsg("", System.Windows.MessageBoxImage.Error);
                    return LatestSelected = null;
                }
                string.Join(Environment.NewLine, dialog.FileNames).Output();
                return LatestSelected = dialog.FileNames;
            }

            return LatestSelected = null;
        }
        public static FileType GetFileType(string path)
        {
            var ext = Path.GetExtension(path).ToLower().Replace(".", "");
            return ext == "mp4" || ext == "mov" ? FileType.Video : FileType.Image;
        }

    }
    enum FileType
    {
        Video,
        Image
    }
}
