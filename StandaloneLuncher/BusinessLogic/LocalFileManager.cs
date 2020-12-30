using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using StandaloneLuncher.DataModels;

namespace StandaloneLuncher.BusinessLogic
{
    public class LocalFileManager
    {
        public AppVersionInfo CurrentVersionInfo;

        public AppVersionInfo LocalVersionInfo => LoadLocalVersionInfo();
        
        private readonly string _appdomainpath = Environment.CurrentDirectory;
        public string ApplicationFolder=>Path.Combine(_appdomainpath, "app");
        private string VersionFilePath => Path.Combine(ApplicationFolder, "version.json");
        private string DownloadedFilePath=> Path.Combine(ApplicationFolder, "download.zip");
        public string ExecutablePath=> Path.Combine(ApplicationFolder, "build", "StandaloneWindows64","Paint23.exe");

        public delegate void DownloadProgressEvent(int progress);

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
            WebClient wc=new WebClient();
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
            wc.DownloadFileAsync(new Uri(downloadUrl), DownloadedFilePath);

        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            SaveVersionInfo();
            UnzipFiles();
      
            OnDownloadCompleted?.Invoke(100);
    
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress?.Invoke(e.ProgressPercentage);
        }


        private bool UnzipFiles()
        {
            try
            {
                ZipFile.ExtractToDirectory(DownloadedFilePath, ApplicationFolder, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            File.Delete(DownloadedFilePath);
            return true;

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
            string data= JsonConvert.SerializeObject(CurrentVersionInfo);
            if (Directory.Exists(ApplicationFolder))
            {
                File.WriteAllText(VersionFilePath, data);
            }
         
        }

        private AppVersionInfo LoadLocalVersionInfo()
        {
            if(!File.Exists(VersionFilePath))return null;
            string data = File.ReadAllText(VersionFilePath);
            return JsonConvert.DeserializeObject<AppVersionInfo>(data);
        }


    }
}
