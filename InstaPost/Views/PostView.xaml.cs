using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using CoreLib.Helpers;
using InstagramApiSharp.Classes.Models;

namespace InstaPost.Views
{
    /// <summary>
    /// Interaction logic for PostView.xaml
    /// </summary>
    public partial class PostView : UserControl
    {
        public PostView()
        {
            InitializeComponent();
        }

        private void SelectFilesButtonClick(object sender, RoutedEventArgs e)
        {
            var files = FileSelectorHelper.SelectFiles();
            if (files == null)
            {
                SelectFilesButton.Content = "Select files";
                UploadButton.IsEnabled = false;
            }
            else
            {
                SelectFilesButton.Content = $"Selected files ({files.Length})";
                UploadButton.IsEnabled = true;
            }
        }

        private async void UploadButtonClick(object sender, RoutedEventArgs e)
        {
            if (FileSelectorHelper.LatestSelected == null)
                "Please select at least one file.".ShowMsg("ERR", MessageBoxImage.Error);
            if (UploadButton.Content.ToString().StartsWith("Uploading") ||
                UploadButton.Content.ToString().StartsWith("Preparing"))
            {
                "Uploading....".ShowMsg("ERR", MessageBoxImage.Error);
                return;
            }
            var caption = CaptionText.Text;
            if (FileSelectorHelper.LatestSelected.Length == 1)
            {
                // single photo or video
                var firstPath = FileSelectorHelper.LatestSelected.FirstOrDefault();
                FileType type = FileSelectorHelper.GetFileType(firstPath);
                if (type == FileType.Video)
                {
                    UploadButton.Content = "Preparing video...";
                    var screenshot = Path.Combine(Helper.CacheTempPath, Helper.RandomString()) + ".jpg";
                    Helper.FFmpeg.ExtractImageFromVideo(firstPath, screenshot);
                    await Task.Delay(500);
                    var vid = new InstaVideoUpload
                    {
                        Video = new InstaVideo(firstPath, 0, 0),
                        VideoThumbnail = new InstaImage(screenshot, 0, 0)
                    };
                    UploadButton.Content = "Preparing video...";
                    var up = await Helper.InstaApi.MediaProcessor.UploadVideoAsync(vid, caption);
                    if (up.Succeeded)
                        "Your video uploaded successfully.".ShowMsg("Succeeded", MessageBoxImage.Information);
                    else
                        up.Info.Message.ShowMsg("ERR", MessageBoxImage.Error);
                    UploadButton.Content = "Upload";
                }
                else
                {
                    UploadButton.Content = "Uploading photo...";
                    var imgPath = ImageHelper.ConvertToJPEG(firstPath);
                    var img = new InstaImage(imgPath, 0, 0);
                    var up = await Helper.InstaApi.MediaProcessor.UploadPhotoAsync(img, caption);
                    if (up.Succeeded)
                        "Your photo uploaded successfully.".ShowMsg("Succeeded", MessageBoxImage.Information);
                    else
                        up.Info.Message.ShowMsg("ERR", MessageBoxImage.Error);
                    UploadButton.Content = "Upload";
                }
            }
            else
            {
                // album
                var videos = new List<InstaVideoUpload>();
                var images = new List<InstaImage>();

                UploadButton.Content = "Preparing album...";
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                (Action)(async() =>
                {
                    foreach (var item in FileSelectorHelper.LatestSelected)
                    {
                        var type = FileSelectorHelper.GetFileType(item);
                        if (type == FileType.Image)
                        {
                            var imgPath = ImageHelper.ConvertToJPEG(item);
                            images.Add(new InstaImage(imgPath, 0, 0));
                        }
                        else
                        {
                            var screenshot = Path.Combine(Helper.CacheTempPath, Helper.RandomString()) + ".jpg";
                            Helper.FFmpeg.ExtractImageFromVideo(item, screenshot);
                  
                            var vid = new InstaVideoUpload
                            {
                                Video = new InstaVideo(item, 0, 0),
                                VideoThumbnail = new InstaImage(screenshot, 0, 0)
                            };
                            videos.Add(vid);
                            await Task.Delay(2000);
                        }
                    }
                    await Task.Delay(6000);
                    UploadButton.Content = "Uploading album...";
                    var up = await Helper.InstaApi.MediaProcessor.UploadAlbumAsync(images.ToArray(),
                        videos.ToArray(), caption);

                    if (up.Succeeded)
                        "Your album uploaded successfully.".ShowMsg("Succeeded", MessageBoxImage.Information);
                    else
                        up.Info.Message.ShowMsg("ERR", MessageBoxImage.Error);
                    UploadButton.Content = "Upload";
                }));
            }
        }
    }
}
