using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;
using Titanium.Web.Proxy.Http;
using uhttpsharp;
using PSHostsFile;
using uhttpsharp.Listeners;
using System.Net.Sockets;
using uhttpsharp.RequestProviders;

namespace TurboEmu
{
    public class Emulation
    {
        /* Reload Internetsettings */
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;

        /* Cryptkey */
        private readonly string CryptPassword = "+;(/`9\"-Z7pE)qSQb(CNY&u;R;Y=;BAB";
        private readonly string CryptPasswordPing = "/Bqe8h:de!Nru>Sj_pd}W[ZWFKdS!j:n";
        private readonly byte[] CryptSalt = { 5, 5, 2, 2, 8, 6, 3, 4, 5, 7, 2, 3, 8, 6, 1, 4 };
        private string AuthToken = string.Empty;
        private int LicenseId = 0;

        /* Settings */
        public bool Tier1_Free = false;
        public bool Tier2_Standard = false;
        public bool Tier3_Unleashed = false;
        public bool Trial = false;
        public int Method = 1;

        /* Proxy Server */
        ProxyServer RoSProxy = null;
        public int ProxyPort = 1337;

        /* Webserver */
        HttpServer RoSWebserver = null;

        /* DEBUG */
        bool DebugRunning = false;
        bool Debugging = false;

        /* TurboHUD Process */
        private string TurboHUDexeName = "TurboHUD.exe";
        private Process TurboHUDexe = null;
        private Version TurboHUDversion = null;

        /* Logging */
        readonly Log log = new Log();

        /* Start TurboHUD */
        public void StartTurboHUD()
        {
            log.Write("Starting TurboEmu");

            /* Check if TurboHUD is installed */
            try
            {
                if (!Directory.Exists("thud"))
                    Directory.CreateDirectory("thud");
            }
            catch (Exception ex)
            {
                log.Write("Can't create folder \"thud\". Exception: " + ex.Message);
                MessageBox.Show("Can't create folder \"thud\".\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            string[] Files = Directory.GetFiles("thud", "*.exe");
            if (Files.Length == 2)
            {
                for (int i = 0; i < Files.Length; i++)
                {
                    string FileName = Path.GetFileName(Files[i]);
                    if (FileName.Equals("TurboHUD.exe"))
                        break;
                    if (FileName.Equals("TurboMGR.exe"))
                        continue;
                    TurboHUDexeName = FileName;
                }
                log.Write("Found TurboHUD: " + TurboHUDexeName);

                /* Start TurboHUD */
                bool StartTurboHUD = false;
                switch (Method)
                {
                    case 0:
                        log.Write("Loading webserver method...");
                        if (StartWebserver())
                            if (InstallHosts())
                                StartTurboHUD = true;
                        break;
                    case 1:
                        log.Write("Loading proxy method...");
                        if (StartProxy())
                            if (InstallProxy())
                                StartTurboHUD = true;
                        break;
                }

                if (StartTurboHUD && InstallCert())
                {
                    try
                    {
                        log.Write("Starting TurboHUD");
                        /* Start TurboHUD Process */

                        ProcessStartInfo CmdStart = new ProcessStartInfo
                        {
                            UseShellExecute = false,
                            FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\cmd.exe"),
                            WorkingDirectory = "thud\\",
                            Arguments = "/c start \"\" \"" + Application.StartupPath + "\\thud\\" + TurboHUDexeName + "\""
                        };
                        Process.Start(CmdStart);

                        try
                        {
                            while (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(TurboHUDexeName)).Length == 0)
                            {
                                System.Threading.Thread.Sleep(50);
                                Application.DoEvents();
                            }
                            TurboHUDexe = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(TurboHUDexeName))[0];
                            TurboHUDexe.WaitForInputIdle();
                            Version.TryParse(TurboHUDexe.MainModule.FileVersionInfo.FileVersion, out TurboHUDversion);
                        }
                        catch (Exception)
                        {
                            Version.TryParse(FileVersionInfo.GetVersionInfo(Application.StartupPath + "\\thud\\" + TurboHUDexeName).FileVersion, out TurboHUDversion);
                            throw;
                        }


                        /* Wait for Modules loaded */
                        TurboHUDexe.WaitForInputIdle();
                        Version.TryParse(TurboHUDexe.MainModule.FileVersionInfo.FileVersion, out TurboHUDversion);
                        log.Write("TurboHUD version: " + TurboHUDversion);
                        AuthToken = GetRandomAuthToken();
                        log.Write("Generating AuthToken: " + AuthToken);
                        LicenseId = new Random().Next(1000, 99999);
                        log.Write("Fake LicenseId: " + LicenseId);

                        /* Wait for Exit */
                        log.Write("TurboHUD started");
                        if (!Debugging)
                        {
                            TurboHUDexe.WaitForExit();
                            /* TurboHUD closed */
                            StopTurboHUD();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Write("There is a problem while starting TurboHUD. " + TurboHUDexeName + " Exception: " + ex.Message);
                        MessageBox.Show("There is a problem while starting TurboHUD. " + TurboHUDexeName + "\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                Application.Exit();
            }
            else
            {
                log.Write("TurboHUD is not installed");
                log.Write("Please redownload the latest version of TurboEmu and extract the \"thud\" where TurboEmu is located!");
                MessageBox.Show("TurboHUD is not installed!\nPlease redownload the latest version of TurboEmu and extract the \"thud\" where TurboEmu is located!", "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Process.Start("https://forum.turboemu.com/");
                Application.Exit();
                return;
            }
        }

        /* Stop TurboHUD */
        public void StopTurboHUD()
        {
            try
            {
                if (TurboHUDexe != null)
                    TurboHUDexe.Kill();
            }
            catch { }
            switch (Method)
            {
                case 0:
                    StopWebserver();
                    UninstallHosts();
                    break;
                case 1:
                    StopProxy();
                    UninstallProxy();
                    break;
            }
            UninstallCert();
        }

        /* Install Windows proxy */
        public bool InstallProxy()
        {
            try
            {
                log.Write("Modify Windows proxy settings...");
                RegistryKey registryKey = Registry.CurrentUser;
                using (RegistryKey ProxySettings = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings", true))
                {
                    ProxySettings.SetValue("ProxyEnable", 1);
                    ProxySettings.SetValue("ProxyServer", "localhost:" + ProxyPort);
                }
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
                log.Write("Windows proxy settings changed to localhost:" + ProxyPort);
                return true;
            }
            catch (Exception ex)
            {
                log.Write("Modify Windows proxy settings failed. Exception: " + ex.Message);
                MessageBox.Show("Modify Windows proxy settings failed.\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /* Uninstall Windows proxy */
        public void UninstallProxy()
        {
            try
            {
                log.Write("Removing Windows proxy settings...");
                RegistryKey registryKey = Registry.CurrentUser;
                using (RegistryKey ProxySettings = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings", true))
                {
                    ProxySettings.SetValue("ProxyEnable", 0);
                    ProxySettings.SetValue("ProxyServer", string.Empty);
                }
                log.Write("Windows proxy settings deleted");
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
            }
            catch (Exception ex)
            {
                log.Write("Modify Windows proxy settings failed. Exception: " + ex.Message);
                MessageBox.Show("Modify Windows proxy settings failed.\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* Install Windows hosts */
        public bool InstallHosts()
        {
            try
            {
                log.Write("Modify Windows hosts file...");
                log.Write("Binding ros-bot.com -> 127.0.0.1");
                HostsFile.Set("ros-bot.com", "127.0.0.1");
                log.Write("Binding www.ros-bot.com -> 127.0.0.1");
                HostsFile.Set("www.ros-bot.com", "127.0.0.1");
                log.Write("Binding lstrckr.hopto.org -> 127.0.0.1");
                HostsFile.Set("lstrckr.hopto.org", "127.0.0.1");
                log.Write("Windows hosts is now modified");
                return true;
            }
            catch (Exception ex)
            {
                log.Write("Modify Windows hosts failed. Exception: " + ex.Message);
                MessageBox.Show("Modify Windows hosts failed.\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /* Uninstall Windows hosts */
        public void UninstallHosts()
        {
            try
            {
                log.Write("Modify Windows hosts file...");
                log.Write("Removing ros-bot.com");
                HostsFile.Remove("ros-bot.com");
                log.Write("Removing www.ros-bot.com");
                HostsFile.Remove("www.ros-bot.com");
                log.Write("Removing lstrckr.hopto.org");
                HostsFile.Remove("lstrckr.hopto.org");
                log.Write("Windows hosts cleared");
            }
            catch (Exception ex)
            {
                log.Write("Modify Windows hosts failed. Exception: " + ex.Message);
                MessageBox.Show("Modify Windows hosts failed.\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* Install SSL certificate */
        public bool InstallCert()
        {
            try
            {
                log.Write("Installing ros-bot.com certificates");
                X509Certificate2 BaltimoreCA = new X509Certificate2(Properties.Resources.BaltimoreCA);
                X509Certificate2 CloudflareSubCA = new X509Certificate2(Properties.Resources.CloudflareSubCA);
                X509Store x509Store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                log.Debug("Certificate CA fingerprint: " + BaltimoreCA.Thumbprint);
                log.Debug("Certificate SubCA fingerprint: " + CloudflareSubCA.Thumbprint);
                x509Store.Open(OpenFlags.ReadWrite);
                x509Store.Add(BaltimoreCA);
                x509Store.Add(CloudflareSubCA);
                x509Store.Close();
                log.Write("ros-bot.com certificates are now installed");
                return true;
            }
            catch (Exception ex)
            {
                log.Write("Can't install fake ros-bot.com certificate. Exception: " + ex.Message);
                MessageBox.Show("Can't install fake ros-bot.com certificate.\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /* Uninstall SSL certificate */
        public void UninstallCert()
        {
            try
            {
                log.Write("Removing ros-bot.com certificates...");
                X509Certificate2 BaltimoreCA = new X509Certificate2(Properties.Resources.BaltimoreCA);
                X509Certificate2 CloudflareSubCA = new X509Certificate2(Properties.Resources.CloudflareSubCA);
                X509Store x509Store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                x509Store.Open(OpenFlags.ReadWrite);
                x509Store.Remove(BaltimoreCA);
                x509Store.Remove(CloudflareSubCA);
                x509Store.Close();
                log.Write("ros-bot.com certificates removed");
            }
            catch (Exception ex)
            {
                log.Write("Can't uninstall fake ros-bot.com certificate. Exception: " + ex.Message);
                MessageBox.Show("Can't uninstall fake ros-bot.com certificate.\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* Start Webserver */
        public bool StartWebserver()
        {
            uint ProcessID = ProcessPortCheck(443).FirstOrDefault();
            if (ProcessID == 0)
            {
                try
                {
                    log.Write("Starting webserver...");
                    X509Certificate2Collection collection = new X509Certificate2Collection();
                    log.Write("Loading ros-bot.com certificate...");
                    collection.Import(Properties.Resources.TurboEmu, "turboemu", X509KeyStorageFlags.PersistKeySet);
                    log.Debug("Certificate fingerprint: " + collection[0].Thumbprint);
                    RoSWebserver = new HttpServer(new HttpRequestProvider());
                    RoSWebserver.Use(new ListenerSslDecorator(new TcpListenerAdapter(new TcpListener(IPAddress.Loopback, 443)), collection[0]));
                    RoSWebserver.Use((context, next) =>
                    {
                        string[] RequestParameters = context.Request.RequestParameters;
                        byte[] getBytes = context.Request.Post.Raw;
                        if (RequestParameters.Length != 0)
                        {
                            if (!RequestParameters[0].Equals("favicon.ico")) /* Ignore favicon.ico request */
                            {
                                if (RequestParameters[0] != null && RequestParameters[0].Equals("ms"))
                                    if (RequestParameters[1] != null && RequestParameters[1].Equals("v1"))
                                    {
                                        if (RequestParameters[2] != null && RequestParameters[2].Equals("auth"))
                                        {
                                            if (RequestParameters[3] != null && RequestParameters[3].Equals("thud"))
                                            {
                                                byte[] LicenseResult = TurboHUDLicenseCheck(getBytes);
                                                context.Response = new HttpResponse(HttpResponseCode.Ok, LicenseResult, false);
                                                return Task.Factory.GetCompleted();
                                            }
                                        }
                                        else if (RequestParameters[2] != null && RequestParameters[2].Equals("logout"))
                                        {
                                            TurboHUDLogout();
                                            context.Response = new HttpResponse(HttpResponseCode.Ok, string.Empty, false);
                                            return Task.Factory.GetCompleted();
                                        }
                                    }
                                    else if (RequestParameters[1] != null && RequestParameters[1].Equals("v4"))
                                        if (RequestParameters[2] != null && RequestParameters[2].Equals("ping"))
                                        {
                                            byte[] PingResult = TurboHUDPingCheck(getBytes);
                                            context.Response = new HttpResponse(HttpResponseCode.Ok, PingResult, false);
                                            return Task.Factory.GetCompleted();
                                        }
                            }
                        }
                        string HTML = "<html><title>TurboEmu</title><center><h1>TurboEmu</h1><br><h3>Aslong you use the webserver method you can't reach ros-bot.com from your computer.</h3></center></html>";
                        context.Response = new HttpResponse(HttpResponseCode.Ok, HTML, false);
                        return Task.Factory.GetCompleted();
                    });
                    RoSWebserver.Start();
                    return true;
                }
                catch (Exception ex)
                {
                    log.Write("Failed to start webserver. Please check if port 443 is used by other programms. Exception: ");
                    MessageBox.Show("Failed to start webserver.\nPlease check if port 443 is used by other programms.\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    Process BlockProcess = Process.GetProcessById((int)ProcessID);
                    log.Write("Following process is blocking port 443 what is needed for the webserver. | Process: " + BlockProcess.MainModule.ModuleName + " PID: " + (int)ProcessID);
                    MessageBox.Show("Following process is blocking port 443 what is needed for the webserver.\n\nProcess:\n" + BlockProcess.MainModule.FileName + "\nPID:\n" + (int)ProcessID, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    log.Write("Following process is blocking port 443 what is needed for the webserver. | PID: " + (int)ProcessID + " Exception: " + ex.Message);
                    MessageBox.Show("Following process is blocking port 443 what is needed for the webserver.\n\nPID:\n" + (int)ProcessID + "\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;
        }

        /* Stop Webserver */
        public void StopWebserver()
        {
            if (RoSWebserver == null)
                return;

            try
            {
                log.Write("Shutdown webserver...");
                RoSWebserver.Dispose();
                RoSWebserver = null;
            }
            catch (Exception ex)
            {
                log.Write("Failed to shutdown the webserver. Exception: " + ex.Message);
                MessageBox.Show("Failed to shutdown the webserver.\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* Start Proxy */
        public bool StartProxy()
        {
            try
            {
                uint ProcessID = ProcessPortCheck((uint)ProxyPort).FirstOrDefault();
                if (ProcessID != 0)
                    log.Write("The port for the proxy server " + ProxyPort + " is already used by another process. Wait a moment, we are looking for a free port.");
                while (ProcessID != 0 && ProcessID < 65535)
                {
                    ProxyPort++;
                    ProcessID = ProcessPortCheck((uint)ProxyPort).FirstOrDefault();
                }

                if (ProcessID == 0)
                {
                    log.Write("Starting proxy server....");
                    X509Certificate2Collection collection = new X509Certificate2Collection();
                    log.Write("Loading ros-bot.com certificate...");
                    collection.Import(Properties.Resources.TurboEmu, "turboemu", X509KeyStorageFlags.PersistKeySet);
                    log.Debug("Certificate fingerprint " + collection[0].Thumbprint);
                    RoSProxy = new ProxyServer(false, true, false);
                    RoSProxy.CertificateManager.RootCertificate = collection[0];
                    ExplicitProxyEndPoint explicitProxyEndPoint = new ExplicitProxyEndPoint(IPAddress.Loopback, ProxyPort, true)
                    {
                        GenericCertificate = collection[0]
                    };

                    explicitProxyEndPoint.BeforeTunnelConnectRequest += (sender, e) =>
                    {
                        if (e.HttpClient.Request.RequestUri.Host.Contains("ros-bot.com"))
                        {
                            if (TurboHUDexe != null)
                            {
                                e.DecryptSsl = true;
                                return Task.CompletedTask;
                            }
                        }
                        e.DecryptSsl = false;
                        return Task.CompletedTask;
                    };
                    RoSProxy.AddEndPoint(explicitProxyEndPoint);
                    RoSProxy.Start();
                    RoSProxy.SetAsSystemHttpProxy(explicitProxyEndPoint);
                    RoSProxy.SetAsSystemHttpsProxy(explicitProxyEndPoint);
                    log.Write("Proxy server is now online");

                    /* RoS-Bot API Emulierung */
                    RoSProxy.BeforeRequest += async (sender, e) =>
                    {
                        /* Block Telemetry */
                        if (e.HttpClient.Request.RequestUri.AbsoluteUri.Contains("lstrckr.hopto.org"))
                        {
                            log.Write("Blocking TurboHUD telemetry");
                            e.Ok(string.Empty);
                            return;
                        }
                        /* Ignore non RoS Bot Stuff */
                        if (!e.HttpClient.Request.RequestUri.AbsoluteUri.Contains("ros-bot.com"))
                        {
                            e.UserData = e.HttpClient.Request;
                            return;
                        }

                        string[] RequestParameters = e.HttpClient.Request.RequestUri.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        byte[] getBytes = await e.GetRequestBody();
                        if (RequestParameters.Length != 0)
                        {
                            if (!RequestParameters[0].Equals("favicon.ico")) /* Ignore favicon.ico request */
                            {
                                if (RequestParameters[0] != null && RequestParameters[0].Equals("ms"))
                                    if (RequestParameters[1] != null && RequestParameters[1].Equals("v1"))
                                    {
                                        if (RequestParameters[2] != null && RequestParameters[2].Equals("auth"))
                                        {
                                            if (RequestParameters[3] != null && RequestParameters[3].Equals("thud"))
                                            {
                                                byte[] LicenseResult = TurboHUDLicenseCheck(getBytes);
                                                e.Ok(LicenseResult);
                                                return;
                                            }
                                        }
                                        else if (RequestParameters[2] != null && RequestParameters[2].Equals("logout"))
                                        {
                                            TurboHUDLogout();
                                            e.Ok(string.Empty);
                                            return;
                                        }
                                    }
                                    else if (RequestParameters[1] != null && RequestParameters[1].Equals("v4"))
                                        if (RequestParameters[2] != null && RequestParameters[2].Equals("ping"))
                                        {
                                            byte[] PingResult = TurboHUDPingCheck(getBytes);
                                            if (PingResult != null)
                                                e.Ok(PingResult);
                                            else
                                                e.UserData = e.HttpClient.Request;
                                            return;
                                        }
                            }
                        }
                        else
                        {
                            e.UserData = e.HttpClient.Request;
                            return;
                        }
                    };
                    RoSProxy.BeforeResponse += async (sender, e) =>
                    {
                        var responseHeaders = e.HttpClient.Response.Headers;
                        if (e.HttpClient.Request.Method == "GET" || e.HttpClient.Request.Method == "POST")
                        {
                            if (e.HttpClient.Response.StatusCode == 200)
                            {
                                if (e.HttpClient.Response.ContentType != null && e.HttpClient.Response.ContentType.Trim().ToLower().Contains("text/html"))
                                {
                                    byte[] BodyBytes = await e.GetRequestBody();
                                    e.SetResponseBody(BodyBytes);

                                    string Body = await e.GetRequestBodyAsString();
                                    e.SetResponseBodyString(Body);
                                }
                            }
                            if (e.UserData != null)
                            { Request request = (Request)e.UserData; }
                        }
                    };
                    RoSProxy.ServerCertificateValidationCallback += (sender, e) =>
                    {
                        if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                            e.IsValid = true;

                        return Task.CompletedTask;
                    };
                    RoSProxy.ClientCertificateSelectionCallback += (sender, e) =>
                    {
                        return Task.CompletedTask;
                    };

                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Write("Starting proxy server failed. Please check if port " + ProxyPort + " is used by other programms. Exception: ");
                MessageBox.Show("Starting proxy server failed.\nPlease check if port " + ProxyPort + " is used by other programms.\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        /* Stop Proxy */
        public void StopProxy()
        {
            if (RoSProxy == null)
                return;

            try
            {
                log.Write("Stopping proxy server...");
                RoSProxy.CertificateManager.RemoveTrustedRootCertificate(true);
                RoSProxy.Stop();
                RoSProxy = null;
            }
            catch (Exception ex)
            {
                log.Write("Stopping proxy server failed. Exception: " + ex.Message);
                MessageBox.Show("Stopping proxy server failed.\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* Licensecheck v1 */
        public byte[] TurboHUDLicenseCheck(byte[] getBytes)
        {
            /* TurboHUD Licensecheck */
            log.Write("TurboHUD license check");
            log.Debug("Received bytes: " + getBytes.Length);
            log.Debug(ByteToString(getBytes));
            string Decrypted = Decrypt(getBytes, CryptPassword);
            Int32.TryParse(Decrypted.Split('|').FirstOrDefault(), out int RandomCheck);
            log.Debug("Found TurboHUD check: " + RandomCheck);
            Dictionary<string, object> ValidateData = new Dictionary<string, object>
                                                    {
                                                { "rnd", RandomCheck.ToString() },
                                                { "version", TurboHUDversion.ToString() },
                                                { "expires", (int)DateTime.UtcNow.AddDays(30).Subtract(new DateTime(1970, 1, 1)).TotalSeconds },
                                                { "is_tier_1", Tier1_Free }, // Tier 1
                                                { "is_tier_2", Tier2_Standard }, // Tier 2
                                                { "is_tier_3", Tier3_Unleashed }, // Tier 3
                                                { "is_trial", Trial }, // Trial License
                                                { "license_id", LicenseId },
                                                { "auth_token", AuthToken }
                                                    };
            string ValidateJSON = JsonConvert.SerializeObject(ValidateData);
            log.Debug("JSON response created");
            log.Debug(ValidateJSON);
            byte[] Encrypted = Encrypt(ValidateJSON, Decrypted.Split('|').LastOrDefault(), RandomCheck);
            log.Write("Sending data to TurboHUD");
            return Encrypted;
        }

        /* Pingcheck v4 */
        public byte[] TurboHUDPingCheck(byte[] getBytes)
        {
            /* TurboHUD Ping */
            log.Write("Received TurboHUD ping check (" + getBytes.Length + "bytes)...");
            string Decrypted = Decrypt(getBytes, CryptPasswordPing);
            Dictionary<string, object> TurboHUDJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(Decrypted);
            TurboHUDJson.TryGetValue("authtoken", out object authtoken);
            string GetJsonAuthtoken = authtoken.ToString();
            int.TryParse(GetJsonAuthtoken.Split('_').LastOrDefault(), out int RandomCheck);
            string ClientAuthToken = GetJsonAuthtoken.Split('_').FirstOrDefault();
            if (AuthToken == ClientAuthToken)
            {
                log.Debug("TurboHUD check is: " + RandomCheck);
                Dictionary<string, object> ValidateData = new Dictionary<string, object>
                                                    {
                                                { "AuthToken", AuthToken },
                                                { "Rnd", RandomCheck.ToString() },
                                                { "Action", 0 },
                                                { "DropReason", 0 },
                                                { "Version", TurboHUDversion.ToString() },
                                                    };
                /*
                 * DropReason:
                 * 0 = Nothing
                 * 1 = Service denied. If you wonder why, contact us.
                 * 2 = Trial key denied! You most probably already have been using one.
                 */
                string ValidateJSON = JsonConvert.SerializeObject(ValidateData);
                log.Debug("JSON response created");
                log.Debug(ValidateJSON);
                byte[] Encrypted = Encrypt(ValidateJSON, AuthToken, RandomCheck);
                log.Debug("Sending result to TurboHUD");
                if (!TurboEmu.Debug)
                    log.Write("TurboHUD ping check completed");
                return Encrypted;
            }
            log.Write("TurboHUD ping check failed");
            return null;
        }

        /* TurboHUD logout */
        public void TurboHUDLogout()
        {
            log.Write("TurboHUD is logging out");
            /* TurboHUD Logout */
            if (TurboHUDexe != null)
                TurboHUDexe.Kill();
        }

        /* Get ProcessID what use specific port */
        private static IEnumerable<uint> ProcessPortCheck(uint Port)
        {
            return PowerShell.Create().AddScript("Get-NetTCPConnection -LocalPort " + Port + "| Where-Object OwningProcess -NE 0").Invoke().Select(p => (uint)p.Properties["OwningProcess"].Value);
        }

        /* Create fake AuthToken */
        public static string GetRandomAuthToken()
        {
            Random random = new Random();
            byte[] buffer = new byte[32 / 2];
            random.NextBytes(buffer);
            string result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (32 % 2 == 0)
                return result.ToLower();
        }

        private string ByteToString(byte[] ToString)
        {
            StringBuilder stringBuilder = new StringBuilder("new byte[] { ");
            for (int i = 0; i < ToString.Length; i++)
            {
                if (i != ToString.Length - 1)
                    stringBuilder.Append(ToString[i] + ", ");
                else
                    stringBuilder.Append(ToString[i]);
            }
            stringBuilder.Append(" }");
            return stringBuilder.ToString();
        }

        /* Decrypt TurboHUD License */
        private string Decrypt(byte[] DecryptData, string CryptPassword)
        {
            try
            {
                byte[] CryptPasswordByte = Encoding.UTF8.GetBytes(CryptPassword);
                log.Debug("Start decrypting...");
                log.Debug("Decrypt password:" + CryptPassword);
                log.Debug("Decrypt salt: " + ByteToString(CryptSalt));
                if (DecryptData == null)
                    return null;
                Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(CryptPasswordByte, CryptSalt, 10);
                byte[] array = rfc2898DeriveBytes.GetBytes(32);
                byte[] array2 = new byte[16];
                byte[] array3 = new byte[16];
                Array.Copy(array, 0, array2, 0, 16);
                Array.Copy(array, 16, array3, 0, 16);
                Aes AES = Aes.Create();
                ICryptoTransform IcryptoTransform = AES.CreateDecryptor(array2, array3);
                byte[] Transformresult = IcryptoTransform.TransformFinalBlock(DecryptData, 0, DecryptData.Length);
                byte[] ToDecrypt = new byte[Transformresult.Length - 32];
                Array.Copy(Transformresult, 32, ToDecrypt, 0, Transformresult.Length - 32);
                IcryptoTransform.Dispose();
                AES.Dispose();
                rfc2898DeriveBytes.Dispose();
                MemoryStream memoryStream = new MemoryStream(ToDecrypt);
                StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8);
                string Decrypted = streamReader.ReadToEnd();
                log.Debug("Data decrypted");
                return Decrypted;
            }
            catch (Exception ex)
            {
                log.Write("You using propably a newer TurboHUD version. (Decrypt) Exception: " + ex.Message);
                MessageBox.Show("You using propably a newer TurboHUD version. (Decrypt) \n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        /* Encrypt TurboHUD License */
        private byte[] Encrypt(string ToCrypt, string EncryptKey, int NumCheck)
        {
            try
            {
                log.Debug("Start encrypting...");
                log.Debug("Key: " + EncryptKey);
                log.Debug("Rnd: " + NumCheck.ToString());
                if (ToCrypt == null || EncryptKey == null || NumCheck == 0)
                    return null;
                byte[] ToCryptData = Encoding.UTF8.GetBytes(ToCrypt);
                byte[] array = new byte[ToCryptData.Length + 32];
                byte[] array2 = new byte[16];
                byte[] array3 = new byte[16];
                byte[] Password = Encoding.UTF8.GetBytes(EncryptKey + "_" + NumCheck);
                byte[] encrypted = new Rfc2898DeriveBytes(Password, CryptSalt, 10).GetBytes(32);
                Array.Copy(ToCryptData, 0, array, 32, ToCryptData.Length);
                RandomNumberGenerator.Create().GetBytes(array, 0, 32);
                Array.Copy(encrypted, 0, array2, 0, 16);
                Array.Copy(encrypted, 16, array3, 0, 16);
                Aes aes = Aes.Create();
                ICryptoTransform IcryptoTransform = aes.CreateEncryptor(array2, array3);
                byte[] Encrypted = IcryptoTransform.TransformFinalBlock(array, 0, array.Length);
                log.Debug("Data encrypted");
                return Encrypted;
            }
            catch (Exception ex)
            {
                log.Write("You using propably a newer TurboHUD version. (Encrypt) Exception: " + ex.Message);
                MessageBox.Show("You using propably a newer TurboHUD version. (Encrypt)\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public void Debug()
        {
            Debugging = true;
            if (TurboHUDversion == null)
            {
                Version.TryParse("21.7.22.20", out TurboHUDversion);
                AuthToken = GetRandomAuthToken();
                LicenseId = new Random().Next(1000, 99999);
            }
            if (!DebugRunning)
            {
                if (InstallCert())
                    if (StartWebserver())
                        if (InstallHosts())
                        {
                            DebugRunning = true;
                            MessageBox.Show("Webserver debug started", "TurboEmu Debug", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
            }
            else
            {
                UninstallCert();
                StopWebserver();
                UninstallHosts();
                DebugRunning = true;
                MessageBox.Show("Webserver debug stopped", "TurboEmu Debug", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DebugRunning = false;
            }
        }
    }
}
