using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StandaloneLuncher.DataModels;
using StandaloneLuncher.Properties;

namespace StandaloneLuncher.BusinessLogic
{
    public class AppUpdateManager
    {
        private readonly string BaseUrl = "https://api.appcenter.ms/v0.1/public/sdk/apps/";
        private string AppSecret => Resources.AppSecret;
        private string RequestUrl =>  AppSecret + "/releases/latest";

        

        public async Task<AppVersionInfo> GetData()
        {
            using var httpClient = new HttpClient {BaseAddress = new Uri(BaseUrl)};

            using var response = await httpClient.GetAsync(RequestUrl);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<AppVersionInfo>();
            }

            return null;

        }


    }
}
