using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.WaitWha.AppScanEnterprise.Utils
{
    public class DateTimeUtils
    {
        /// <summary>
        /// Parses the given string and returns a DateTime object.
        /// </summary>
        /// <param name="dateTime">Date time string (e.g. mm/dd/yyyy HH:MM tt -- or -- 10/31/2017 11:01 PM)</param>
        /// <returns></returns>
        public static DateTime FromString(string dateTime)
        {
            return DateTime.ParseExact(dateTime, "MM/dd/yyyy hh:mm tt", CultureInfo.CurrentCulture);
        }
    }
}
