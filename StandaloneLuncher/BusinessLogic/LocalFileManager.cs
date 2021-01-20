using Ionic.Zip;
using Newtonsoft.Json;
using StandaloneLuncher.DataModels;
using StandaloneLuncher.Properties;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace StandaloneLuncher.BusinessLogic
{
    public class LocalFileManager
    {
        public AppVersionInfo CurrentVersionInfo;

        public AppVersionInfo LocalVersionInfo => LoadLocalVersionInfo();

        private readonly string _appdomainpath = Environment.CurrentDirectory;
        public string ApplicationFolder => Path.Combine(_appdomainpath, "app");
        private string VersionFilePath => Path.Combine(ApplicationFolder, "version.json");
        private string DownloadedFilePath => Path.Combine(ApplicationFolder, "download.zip");
        public string ExecutablePath => Path.Combine(ApplicationFolder, Resources.ApplicationRelativePath);

        public delegate void DownloadProgressEvent(ProgressModel progress);

        public DownloadProgressEvent DownloadProgress;
        public DownloadProgressEvent OnDownloadCompleted;

        public LocalFileManager()
        {
            if (!Directory.Exists(ApplicationFolder))
            {
                Directory.CreateDirectory(ApplicationFolder);
            }

        }

        public void DownloadFiles(string downloadUrl)
        {
            if (!Directory.Exists(ApplicationFolder))
            {
                Directory.CreateDirectory(ApplicationFolder);
            }
            using WebClient wc = new WebClient();
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
            wc.DownloadFileAsync(new Uri(downloadUrl), DownloadedFilePath);
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            SaveVersionInfo();

           

            UnzipFiles();
            
        }

        

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressModel progress = new ProgressModel
            {
                BytesToDownload = e.TotalBytesToReceive,
                BytesDownloaded = e.BytesReceived,
                Progress = e.ProgressPercentage
            };
            progress.Message = $"Downloading {progress.Progress}% ({progress.MegaBytesDownloaded}MB/{progress.MegaBytesToDownload}MB)";
            DownloadProgress?.Invoke(progress);
        }


        private async void UnzipFiles()
        {
            
            using (var zip = ZipFile.Read(DownloadedFilePath))
            {
                ProgressModel progress = new ProgressModel {FilesToDownload = zip.Count, FilesDownloaded = 0};

                foreach (var e in zip)
                {
                    await Task.Run(() =>
                    {
                        e.Extract(ApplicationFolder, ExtractExistingFileAction.OverwriteSilently);
                    });

                    progress.FilesDownloaded++;
                    progress.Progress = Convert.ToInt32(100 * progress.FilesDownloaded / progress.FilesToDownload);
                    progress.Message =
                        $"Extracting {progress.Progress}% ({progress.FilesDownloaded}/{progress.FilesToDownload})";
                    
                    DownloadProgress?.Invoke(progress);
                }

            }

            
            File.Delete(DownloadedFilePath);
            OnDownloadCompleted?.Invoke(null);
        }

        public bool DeleteLocalFiles()
        {
            try
            {
                Directory.Delete(ApplicationFolder, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

            return true;

        }

        private void SaveVersionInfo()
        {
            string data = JsonConvert.SerializeObject(CurrentVersionInfo);
            if (Directory.Exists(ApplicationFolder))
            {
                File.WriteAllText(VersionFilePath, data);
            }

        }

        private AppVersionInfo LoadLocalVersionInfo()
        {
            if (!File.Exists(VersionFilePath)) return null;
            string data = File.ReadAllText(VersionFilePath);
            return JsonConvert.DeserializeObject<AppVersionInfo>(data);
        }


    }
}
