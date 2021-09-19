using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace TurboEmu
{
    public class Log
    {
        public static string LogPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\thud_emu\\";
        public static string LogFile = Path.Combine(LogPath, "turboemu.log");

        public void Write(string LogText)
        {
            try
            {
                if (!Directory.Exists(LogPath))
                    Directory.CreateDirectory(LogPath);
                using (StreamWriter streamWriter = File.AppendText(LogFile))
                {
                    streamWriter.WriteLine(DateTime.Now.ToString("o") + ": " + LogText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to write logfile. " + LogFile + "\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Debug(string LogText)
        {
            if (!TurboEmu.Debug)
                return;

            try
            {
                if(!Directory.Exists(LogPath))
                    Directory.CreateDirectory(LogPath);
                using (StreamWriter streamWriter = File.AppendText(LogFile))
                {
                    streamWriter.WriteLine(DateTime.Now.ToString("o") + " [DEBUG]: " + LogText);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed to write logfile. " + LogFile + "\n\nException:\n" + ex.Message, "TurboEmu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
