using System.Linq;
using System.Xml.Linq;

namespace SftpExchange
{
    class SettingsReader
    {
        string path;
        XElement document;
        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="path">путь до файла настроек</param>
        public SettingsReader(string path)
        {
            this.path = path;
            document = XElement.Load(path);
        }
        /// <summary>
        /// получить значение из файла настроек
        /// </summary>
        /// <param name="valueName">имя параметра</param>
        /// <returns>Возвращает значение параметра</returns>
        public string GetValue(string valueName)
        {
            return document.Descendants(valueName).First().Value;
        }
    }
}
