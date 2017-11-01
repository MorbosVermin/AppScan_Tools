using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Com.WaitWha.AppScanEnterprise.Reports
{
    [XmlRoot("xml-report")]
    public class XmlReport
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("technology")]
        public string Technology { get; set; }

        [XmlAttribute("xmlExportVersion")]
        public string XmlExportVersion { get; set; }

        [XmlElement("issue-type-group")]
        public IssueTypeGroup IssueTypeGroup { get; set; }

        /// <summary>
        /// Parses the XML report at the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XmlReport GetInstance(string path)
        {
            return GetInstance(File.OpenRead(path));
        }

        /// <summary>
        /// Parses the Stream given to return an XmlReport.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static XmlReport GetInstance(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XmlReport));
            return serializer.Deserialize(stream) as XmlReport;
        }

    }

    public class IssueTypeGroup
    {
        [XmlElement("item")]
        public List<Item> Items { get; set; }

    }

    public class Item
    {

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlAttribute("maxIssueSeverity")]
        public int MaxIssueSeverity { get; set; }

        [XmlAttribute("critical")]
        public int Criticals { get; set; }

        [XmlAttribute("high")]
        public int Highs { get; set; }

        [XmlAttribute("medium")]
        public int Mediums { get; set; }
        
        [XmlAttribute("low")]
        public int Lows { get; set; }

        [XmlAttribute("info")]
        public int Infos { get; set; }

    }

}
