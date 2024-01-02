///----------------------------------------------------------------------------
/// Class     : XmlUtility
/// Purpose   : Simple XML serialization & desirialization suport. 
///----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace MultiCastSend
{
    public class XmlUtility
    {
        public static void Serialize(Object data, string fileName)
        {
            Type type = data.GetType();
            XmlSerializer xs = new XmlSerializer(type);
            using (XmlTextWriter xmlWriter = new XmlTextWriter(fileName, System.Text.Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xs.Serialize(xmlWriter, data);                
            }
        }

        public static Object Deserialize(Type type, string fileName)
        {
            Object data = null;
            XmlSerializer xs = new XmlSerializer(type);

            try
            {
                using (XmlTextReader xmlReader = new XmlTextReader(fileName))
                {
                    data = xs.Deserialize(xmlReader);
                }
            }
            catch (Exception ex) 
            { 
                Trace.WriteLine(ex);
            }
            return data;
        }        
    } 
}
