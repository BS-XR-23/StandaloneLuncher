using System;
using Newtonsoft.Json;


namespace StandaloneLuncher.DataModels
{
    public class AppVersionInfo
    {
        public string app_name { get; set; }
        public string app_display_name { get; set; }
        public string app_os { get; set; }
        public string release_notes_url { get; set; }
        public Owner owner { get; set; }


        public string version { get; set; }

        [JsonIgnore]
        public Version Version =>new Version(version);

        public int size { get; set; }



        public string download_url { get; set; }
        public bool mandatory_update { get; set; }


        public string release_notes { get; set; }


        public AppVersionInfo()
        {
            owner=new Owner();
        }

    }

    public class Owner
    {
        public string name { get; set; }
        public string display_name { get; set; }
    }

  

}
