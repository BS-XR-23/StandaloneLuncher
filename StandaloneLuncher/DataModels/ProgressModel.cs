using System;
using System.Collections.Generic;
using System.Text;

namespace StandaloneLuncher.DataModels
{
    public class ProgressModel
    {
        public int Progress { get; set; }
        public long BytesToDownload { get; set; }
        public long MegaBytesToDownload => BytesToDownload / 1000000;
        public long BytesDownloaded { get; set; }
        public long MegaBytesDownloaded => BytesDownloaded / 1000000;
        public int FilesToDownload { get; set; }
        public int FilesDownloaded { get; set; }

        public string Message { get; set; }

    }
}
