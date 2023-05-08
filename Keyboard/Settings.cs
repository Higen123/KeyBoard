using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Keyboard
{
    [Serializable]
    public class Settings
    {

        public byte MinR { get; set; }
        public byte MinG { get; set; }
        public byte MinB { get; set; }

        public byte MaxR { get; set; }
        public byte MaxG { get; set; }
        public byte MaxB { get; set; }

        [XmlIgnore]
        public string SettingsPath { get; set; }
        
        public Settings()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            SettingsPath = Path.Combine(path, "Settings.xml");
        }

        public void SerializeMe()
        {
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(this.GetType());

            using (FileStream fs = new FileStream(SettingsPath, FileMode.Create, FileAccess.Write))
            {
                x.Serialize(fs, this);
            }
        }

        public static Settings DeserializeMe()
        {
            Settings elem = new Settings();
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(elem.GetType());
            using (FileStream fs = new FileStream(elem.SettingsPath, FileMode.Open, FileAccess.Read))
            {
                elem = (Settings)x.Deserialize(fs);
                return elem;
            }
        }
    }
}
