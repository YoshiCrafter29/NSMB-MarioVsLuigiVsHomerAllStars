using System.IO;
using System.Net;
using UnityEngine;

using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

public class UpdateChecker {

    private static readonly string API_URL = "http://api.github.com/repos/YoshiCrafter29/NSMB-MarioVsLuigiVsHomerAllStars/releases/latest";

    /// <summary>
    /// Returns if we're up to date, OR newer, compared to the latest GitHub release version number
    /// </summary>
    public async static void IsUpToDate(Action<bool, string> callback) {

        //get http results
        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(API_URL);
        request.Accept = "application/json";
        request.UserAgent = "YoshiCrafter29/NSMB-MarioVsLuigiVsHomerAllStars";

        HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync();

        if (response.StatusCode != HttpStatusCode.OK)
            return;


        try {
            bool ret = true;

            //get the latest release version number from github
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
            JObject data = JObject.Parse(json);

            string tag = data.Value<string>("tag_name");
            if (tag.StartsWith("v"))
                tag = tag[1..];

            string[] splitTag = tag.Split(".");

            string ver = Application.version;
            if (ver.StartsWith("v"))
                ver = ver[1..];

            string[] splitVer = Application.version.Split(".");

            Debug.Log($"[UPDATE CHECK] Local version: {Application.version} / Remote version: {tag}");

            //check if we're a higher version
            for (int i = 0; i < 4; i++) {
                int.TryParse(splitTag[i], out int remote);
                int.TryParse(splitVer[i], out int local);

                if (local == remote)
                    continue;

                ret &= local >= remote;
            }

            callback(ret, tag);
        } catch { }
    }
}
