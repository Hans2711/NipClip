using NipClip.Classes.Clipboard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NipClip.Classes
{
    class XmlConverter
    {
        public static string ConvertToXml<T>(T entries)
        {
            XmlSerializer serializer = new XmlSerializer(entries.GetType());
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, entries);
            return stringWriter.ToString();
        }
        public static T DeserializeXml<T>(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            using (XmlReader xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings { CloseInput = true, IgnoreWhitespace = true }))
            {
                return (T)serializer.Deserialize(xmlReader);
            }
        }
    }
}
