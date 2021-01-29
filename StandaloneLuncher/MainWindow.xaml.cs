using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignExtensions.Controls;
using StandaloneLuncher.BusinessLogic;

namespace StandaloneLuncher
{

    public partial class MainWindow : MaterialWindow
    {
        
        private LocalFileManager _localFileManager;

        public MainWindow()
        {
            InitializeComponent();
            _localFileManager=new LocalFileManager();

            
            Task.Run(GetUpdateInfo).Wait();
            ApplicationNameLable.Text = $"{Properties.Resources.AppName}";
            ChangeLogLable.Content = $"Change Log({_localFileManager.CurrentVersionInfo.Version})";
            ChangeLogText.Text=_localFileManager.CurrentVersionInfo.release_notes;
            ButtonVisibility();

        }

        private async Task GetUpdateInfo()
        {
            AppUpdateManager updatemanager=new AppUpdateManager();
            _localFileManager.CurrentVersionInfo = await updatemanager.GetData();
        }






        #region ButtonLogic

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            string message=string.Concat("This will update application core (", _localFileManager.CurrentVersionInfo.size/ 1000000, "MB)");

            ConfirmationDialogArguments dialogArgs = new ConfirmationDialogArguments
            {
                Title = "Download core",
                Message = message,
                OkButtonLabel = "OK",
                CancelButtonLabel = "CANCEL",
                StackedButtons = false
            };

            bool result = await ConfirmationDialog.ShowDialogAsync(dialogHost, dialogArgs);

            if (!result)
            {
                return;
            }

            LaunchButton.Visibility = Visibility.Collapsed;
            UpdateButton.Visibility = Visibility.Collapsed;
            UninstallButton.Visibility = Visibility.Collapsed;
            DownloadButton.Visibility = Visibility.Collapsed;
            ProgressBarPanel.Visibility = Visibility.Visible;

            ProgressLable.Text = "Initializing...";

           _localFileManager.OnDownloadCompleted += (progress) =>
            {
                ProgressBarPanel.Visibility = Visibility.Collapsed;
                ButtonVisibility();
            };
            _localFileManager.DownloadProgress += (progress) =>
            {

                ProgressLable.Text = progress.Message;
                ProgressBar.Value = progress.Progress;
            };
            
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

        private async void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            
            ConfirmationDialogArguments dialogArgs = new ConfirmationDialogArguments
            {
                Title = "Are you sure?",
                Message = "This will delete application core",
                OkButtonLabel = "OK",
                CancelButtonLabel = "CANCEL",
                StackedButtons = false
            };

            bool result = await ConfirmationDialog.ShowDialogAsync(dialogHost, dialogArgs);

            if (!result)
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
             LaunchButton.Visibility = !executableAvailable ? Visibility.Collapsed : Visibility.Visible;
            // LocalFilesButton.Visibility = LaunchButton.Visibility;
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


        private void OpenRepo(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "https://github.com/Warhammer4000/StandaloneLuncher",
                UseShellExecute = true
            };

            Process.Start(psi);
            
        }
       


    }
}
