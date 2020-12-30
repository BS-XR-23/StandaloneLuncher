using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using StandaloneLuncher.BusinessLogic;
using StandaloneLuncher.DataModels;

namespace StandaloneLuncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private LocalFileManager _localFileManager;

        public MainWindow()
        {
            InitializeComponent();
            _localFileManager=new LocalFileManager();


            Task.Run(GetUpdateInfo).Wait();
            ChangeLogText.Text = _localFileManager.CurrentVersionInfo.release_notes;

            ButtonVisibility();

        }

        private async Task GetUpdateInfo()
        {
            AppUpdateManager updatemanager=new AppUpdateManager();
            _localFileManager.CurrentVersionInfo = await updatemanager.GetData();
        }






        #region ButtonLogic

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            string message=string.Concat("This will download Application files (", _localFileManager.CurrentVersionInfo.size/ 1000000, "MB)");
            if (MessageBox.Show(
                    this, 
                    message, 
                    "Are you sure?", 
                    MessageBoxButton.OKCancel
                    ) != MessageBoxResult.OK)
            {
                return;
            }

            DownloadButton.Visibility = Visibility.Collapsed;
            ProgressBar.Visibility = Visibility.Visible;
            _localFileManager.OnDownloadCompleted += (progress) =>
            {
                ProgressBar.Visibility = Visibility.Collapsed;
                ButtonVisibility();
            };
            _localFileManager.DownloadProgress += (progress) => ProgressBar.Value = progress;
            
            _localFileManager.DownloadFiles(_localFileManager.CurrentVersionInfo.download_url);

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadButton_Click(sender,e);
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_localFileManager.ExecutablePath);
            Close();
        }

        private void LocalFilesButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", _localFileManager.ApplicationFolder);
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                    this,
                    "This will Delete Application files",
                    "Are you sure?",
                    MessageBoxButton.OKCancel
                ) != MessageBoxResult.OK)
            {
                return;
            }

            if (_localFileManager.DeleteLocalFiles())
            {
                ButtonVisibility();
            }

        }

        private void ButtonVisibility()
        {
            bool executableAvailable=File.Exists(_localFileManager.ExecutablePath);
            DownloadButton.Visibility = executableAvailable ? Visibility.Collapsed : Visibility.Visible;
            LocalFilesButton.Visibility = LaunchButton.Visibility = !executableAvailable ? Visibility.Collapsed : Visibility.Visible;
            UninstallButton.Visibility = !executableAvailable ? Visibility.Collapsed : Visibility.Visible;
            
            if (_localFileManager.LocalVersionInfo != null)
            {
                bool higerVersion = _localFileManager.CurrentVersionInfo.Version.CompareTo(_localFileManager.LocalVersionInfo.Version)>0;
                UpdateButton.Visibility = higerVersion ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                UpdateButton.Visibility = Visibility.Collapsed;
            }


        }

        #endregion

       
    }
}
