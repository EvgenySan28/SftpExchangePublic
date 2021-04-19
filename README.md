# SftpExchangePublic
Пример использования SFTP, XML, работа с файлами  
Функция программы:
- скопировать из папки sftp1 файл в папку local1
- переместить скопированный файл из папки sftp1 в папку sftp2
- скопировать из папки local2 файл в папку sftp3
- переместить скопированный файл из папки local2 в папку local3
- скопировать из папки local4 файл в папку sftp4
- переместить скопированный файл из папки local4 в папку local5  


Настройки путей и подключений хранятся в файле [settings.xml](https://github.com/EvgenySan28/SftpExchangePublic/blob/master/SftpExchange/log.txt) в папке, рядом с программой.  
Лог программы хранится в файле [log.txt](https://github.com/EvgenySan28/SftpExchangePublic/blob/master/SftpExchange/settings.xml) в папке, рядом с программой.  
Когда размер лога становится больше 50 килобайт, он режется до 40 килобайт.
Последние записи всегда остаются  
Точка входа в программу находится в файле [progtam.cs](https://github.com/EvgenySan28/SftpExchangePublic/blob/master/SftpExchange/Program.cs), в методе Main();
