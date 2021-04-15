using System;
using WinSCP;
using System.IO;

//Отправка файлов на SFTP
//Задача отправлять файлы, забирать файлы, перемещать из папки в папку.
//Настраиваться пути, пароли должны из файла XML
//к сожалению, стандартных путей работы с SFTP нет, пришлось подключать стороннюю библиотеку, которая 
//работает как интерфейс для программы winscp
namespace SftpExchange
{
    class Program
    {
        // поля с путями локальными и фтп.
        static string localPath1;
        static string localPath2;
        static string localPath3;
        static string localPath4;
        static string localPath5;
        static string sftpPath1;
        static string sftpPath2;
        static string sftpPath3;
        static string sftpPath4;
        //стримврайтер
        static StreamWriter sw;
        static void Main(string[] args)
        {
            LogLenght();//проверка длинны файла лога
            sw = new StreamWriter(File.Open("log.txt", FileMode.Append));
            SettingsReader settingsReader;
            try
            {
                //Попытка прочитать файл настроек и пути
                //Если что-то из этого не прочитается, то программа закроется с записью в лог.
                //потому и settingsReader инициализируется тут
                settingsReader = new SettingsReader("settings.xml");
                localPath1 = settingsReader.GetValue("local1");
                localPath2 = settingsReader.GetValue("local2");
                localPath3 = settingsReader.GetValue("local3");
                localPath4 = settingsReader.GetValue("local4");
                localPath5 = settingsReader.GetValue("local5");
                sftpPath1 = settingsReader.GetValue("sftp1");
                sftpPath2 = settingsReader.GetValue("sftp2");
                sftpPath3 = settingsReader.GetValue("sftp3");
                sftpPath4 = settingsReader.GetValue("sftp4");
            }
            catch
            {
                Console.WriteLine("Не могу прочитать файл настроек!");
                sw.WriteLine(DateTime.Now + " Не могу прочитать файл настроек!");
                sw.Dispose();
                return;
            }

            try
            {
                // Установка опций сессии 
                //методы сторонней библиотеки. 
                //из файла настроек читаем имя сервера, порт, юзер и пароль.
                //если что-то идет не так, подключиться не получится
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = settingsReader.GetValue("server"),
                    PortNumber = int.Parse(settingsReader.GetValue("port")),
                    UserName = settingsReader.GetValue("user"),
                    Password = settingsReader.GetValue("password")

                };

                using (Session session = new Session())
                {
                    // подключаемся.
                    
                    // получаем отпечаток ключа от сервера
                    sessionOptions.SshHostKeyFingerprint = session.ScanFingerprint(sessionOptions, "SHA-256");
                    //открываем сессию
                    session.Open(sessionOptions);
                    //последовательность необходимых действий для работы.
                    GetFileFromFtp(session, sftpPath1, localPath1);
                    PutFileToFtp(session, localPath2, sftpPath3, localPath3);
                    PutFileToFtp(session, localPath4, sftpPath4, localPath5);
                    Console.WriteLine("Отработали.");
                    sw.WriteLine(DateTime.Now + " Отработали");//запись в лог
                    sw.Dispose();
                }
                

            }
            catch (Exception e)
            {
                sw.WriteLine(DateTime.Now + " Error: {0}", e);
                Console.WriteLine("Error: {0}", e);
                sw.Dispose();
            }
            
        }
        
        /// <summary>
        /// Ограничение лог файла
        /// </summary>
        private static void LogLenght()
        {
            FileInfo fileInfo = new FileInfo("log.txt");
            if (fileInfo.Length > 50*1024)//Если длина больше 50 кб, то
            {
                FileStream fileStream = File.OpenRead("log.txt"); //Открываем файл лога
                fileStream.Position =10*1024;//Перемещаемся на позицию 10 килобайт
                FileStream stream = File.Create("log_tmp.txt");//создаем новый временный файл
                fileStream.CopyTo(stream);//Копируем старый файл в новый без куска в 10 килобайт
                stream.Dispose();//Закрываем
                fileStream.Dispose();//Закрываем
                File.Move("log_tmp.txt", "log.txt", true);//Перемещаем файл с заменой
            }
        }
        /// <summary>
        /// Поместить файлы на фтп
        /// </summary>
        /// <param name="session">объект сессии</param>
        /// <param name="from">откуда</param>
        /// <param name="to">куда</param>
        /// <param name="moveTo">переместить куда</param>
        private static void PutFileToFtp(Session session,string from,string to, string moveTo)
        {
            try
            {
                //методы библиотеки
                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Automatic;
                //выполняет операцию и получает результат
                TransferOperationResult transferResult = session.PutFilesToDirectory(from, to, "*.zip", false, transferOptions);
                transferResult.Check();
                
                foreach (TransferEventArgs transfer in transferResult.Transfers)
                {
                    //Получаем имя файла
                    FileInfo fi = new FileInfo(transfer.FileName);
                    //Перемещаем его
                    File.Move(transfer.FileName, moveTo + fi.Name);
                    //Выводим на консоль и в лог. 
                    Console.WriteLine("Upload and move of {0} succeeded", transfer.FileName);
                    sw.WriteLine(DateTime.Now + " Upload and move of {0} succeeded", transfer.FileName);
                }
            }
            catch (Exception e)
            {
                sw.WriteLine(DateTime.Now + " Error: {0}", e);
                Console.WriteLine("Error: {0}", e);
            }
        }
        /// <summary>
        /// Получение файла с фтп
        /// </summary>
        /// <param name="session">объект сессии</param>
        /// <param name="from">откуда</param>
        /// <param name="to">куда</param>
        private static void GetFileFromFtp(Session session, string from, string to)
        {
            try
            {
                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Automatic;
                TransferOperationResult transferResult = session.GetFilesToDirectory(from, to, "*.zip", false, transferOptions);
                // Throw on any error
                transferResult.Check();
                // Print results
                foreach (TransferEventArgs transfer in transferResult.Transfers)
                {
                    //перемещаем файлы 
                    session.MoveFile(transfer.FileName, sftpPath2);
                    //записываем в консоль и лог.
                    Console.WriteLine("Download and move of {0} succeeded", transfer.FileName);
                    sw.WriteLine(DateTime.Now+ " Download and move of {0} succeeded", transfer.FileName);
                }
            }
            catch (Exception e)
            {
                sw.WriteLine(DateTime.Now + " Error: {0}", e);
                Console.WriteLine("Error: {0}", e);
            }
        }
    }
}

