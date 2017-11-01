using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Com.WaitWha.AppScanEnterprise.REST
{
    [Serializable]
    [DataContract]
    public class ScanFinding
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "11")]
        public string Severity { get; set; }

        [DataMember(Name = "13")]
        public string Vulnerability { get; set; }

        [DataMember(Name = "17")]
        public string ApplicationName { get; set; }

        [DataMember(Name = "29")]
        public string Tool { get; set; }

        [DataMember(Name = "25")]
        public string FileName { get; set; }

        [DataMember(Name = "26")]
        public string LineNumber { get; set; }

        [DataMember(Name = "30")]
        public string StartDateTime { get; set; }

        [DataMember(Name  = "31")]
        public string EndDateTime { get; set; }

    }
}
