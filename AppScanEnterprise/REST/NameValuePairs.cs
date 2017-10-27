using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.WaitWha.AppScanEnterprise.REST
{
    public class NameValuePairs : Dictionary<string, dynamic>
    {

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public string ToQueryString(bool prependQuestionMark = false)
        {
            StringBuilder sb = new StringBuilder();
            if (prependQuestionMark)
                sb.Append("?");

            foreach(string key in this.Keys)
            {
                sb.Append(String.Format("{0}={1}&", key, this[key]));
            }

            return sb.ToString().Substring(0, sb.Length - 1);
        }

    }
}
