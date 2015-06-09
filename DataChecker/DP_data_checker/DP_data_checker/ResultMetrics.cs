using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP_data_checker
{
    public class ResultMetrics
    {
        /// <summary> Labeled Atachment Score </summary>
        public double LAS { get; set; }
        /// <summary> Unlabeled Atachment Score </summary>
        public double UAS { get; set; }
        /// <summary> Labeled Atachment</summary>
        public double LA { get; set; }

        public ResultMetrics()
        {
            LAS = 0;
            UAS = 0;
            LA = 0;
        }

        public ResultMetrics(double las, double uas, double la)
        {
            LAS = las;
            UAS = uas;
            LA = la;
        }

        public string ToStringData()
        {
            string strResult = string.Empty;
            strResult+= "LAS: " + Math.Round(LAS, 2).ToString() + " %\r\n";
            strResult += "UAS: " + Math.Round(UAS, 2).ToString() + " %\r\n";
            strResult += "LA: " + Math.Round(LA, 2).ToString() + " %\r\n";

            return strResult;
        }
    }
}
