using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SftpExchange
{
    class Settings:IFillSettings
    {
        public string localPath1 { get; private set; }
        public string localPath2 { get; private set; }
        public string localPath3 { get; private set; }
        public string localPath4 { get; private set; }
        public string localPath5 { get; private set; }
        public string sftpPath1 { get; private set; }
        public string sftpPath2 { get; private set; }
        public string sftpPath3 { get; private set; }
        public string sftpPath4 { get; private set; }
        public string server { get; private set; }
        public string port { get; private set; }
        public string user { get; private set; }
        public string password { get; private set; }
        SettingsReader settingsReader;
        public Settings(string pathToFileSettings)
        {
            settingsReader = new SettingsReader(pathToFileSettings);
        }

        public void FillSettings()
        {
            this.localPath1 = settingsReader.GetValue("local1");
            this.localPath2 = settingsReader.GetValue("local2");
            this.localPath3 = settingsReader.GetValue("local3");
            this.localPath4 = settingsReader.GetValue("local4");
            this.localPath5 = settingsReader.GetValue("local5");
            this.sftpPath1 = settingsReader.GetValue("sftp1");
            this.sftpPath2 = settingsReader.GetValue("sftp2");
            this.sftpPath3 = settingsReader.GetValue("sftp3");
            this.sftpPath4 = settingsReader.GetValue("sftp4");
            this.server = settingsReader.GetValue("server");
            this.port = settingsReader.GetValue("port");
            this.user = settingsReader.GetValue("user");
            this.password = settingsReader.GetValue("password");
        }
    }
}
