using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.WaitWha.AppScanEnterprise.REST
{
    [Serializable]
    public class Application
    {

        public string name;
        public string url;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
