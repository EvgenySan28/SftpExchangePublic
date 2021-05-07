using System;
using WinSCP;
using System.IO;
using System.Collections.Generic;

/*
Отправка файлов на SFTP
Задача отправлять файлы, забирать файлы, перемещать из папки в папку.
Настраиваться пути, пароли должны из файла XML
к сожалению, стандартных путей работы с SFTP нет, пришлось подключать стороннюю библиотеку, которая 
работает как интерфейс для программы winscp
*/
namespace SftpExchange
{
    class Program
    {
        static Settings settings;
        //стримврайтер используется во многих методах, чтобы не открывать, не закрывать.
        static StreamWriter sw;
        const string pathToLog = "log.txt";
        const string pathToSettings = "settings.xml";
        static void Main(string[] args)
        {
            LogLenght();//проверка длинны файла лога
            sw = new StreamWriter(File.Open(pathToLog, FileMode.Append));
            settings = new Settings(pathToSettings);

            try
            {
                /*
                Установка опций сессии
                методы сторонней библиотеки. 
                из файла настроек читаем имя сервера, порт, юзер и пароль.
                если что-то идет не так, подключиться не получится
                */
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = settings.server,
                    PortNumber = Int32.Parse(settings.port),
                    UserName = settings.user,
                    Password = settings.password

                };

                using (Session session = new Session())
                {
                    // подключаемся.
                    
                    // получаем отпечаток ключа от сервера
                    sessionOptions.SshHostKeyFingerprint = session.ScanFingerprint(sessionOptions, "SHA-256");
                    //открываем сессию
                    session.Open(sessionOptions);
                    //последовательность необходимых действий для работы.
                    GetFileFromFtp(session,settings.sftpPath1, settings.localPath1);
                    PutFileToFtp(session,settings.localPath2, settings.sftpPath3,settings.localPath3);
                    PutFileToFtp(session,settings.localPath4,settings.sftpPath4,settings.localPath5);
                    Console.WriteLine("Отработали.");
                    sw.WriteLine(DateTime.Now + " Отработали");
                    sw.Dispose();
                }
            }
            catch (Exception e)
            {
                sw.WriteLine(DateTime.Now + " Error: {0}", e);
                Console.WriteLine("Error: {0}", e);
                sw.Dispose();
            }
            finally
            {
                sw.Dispose();
            }
        }

        /// <summary>
        /// Ограничение лог файла. Если длинна больше 50 кб, то отрезается 10 кб от начала.
        /// </summary>
        private static void LogLenght()
        {
            if (File.Exists(pathToLog))
            {
                FileInfo fileInfo = new FileInfo(pathToLog);
                if (fileInfo.Length > 50 * 1024)
                {
                    FileStream fileStream = fileInfo.OpenRead();
                    fileStream.Position = 10 * 1024;
                    FileStream stream = File.Create($"tmp{pathToLog}");
                    fileStream.CopyTo(stream);
                    stream.Dispose();
                    fileStream.Dispose();
                    try
                    {
                        File.Move($"tmp{pathToLog}", pathToLog, true);
                    }
                    catch
                    {
                        Console.WriteLine("Не могу перезаписать файл лога!");
                    }
                }
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
                    
                    FileInfo fi = new FileInfo(transfer.FileName);
                    File.Move(transfer.FileName, moveTo + fi.Name);
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
                    session.MoveFile(transfer.FileName,settings.sftpPath2);
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

