using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using HistoricalRepository.Models.Earthquake;

namespace HistoricalRepository.VirtualRepo
{
    static class EarthQuake_CWB
    {
        public static List<EarthquakeObj> GetData(string startdate, string enddate, string min_magnitude, string max_magnitude)
        {
            string data = null;
            string requestString = RequestProducer.GetEarhquakeRequestString(EarthquakeDataSource.PALERT, startdate, enddate, min_magnitude, max_magnitude);

            List<EarthquakeObj> EqList = new List<EarthquakeObj>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestString);
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/json";

            DateTime startDate_dt = new DateTime(int.Parse(startdate.Substring(0, 4)), int.Parse(startdate.Substring(5, 2)), int.Parse(startdate.Substring(8, 2)));
            DateTime endDate_dt = new DateTime(int.Parse(enddate.Substring(0, 4)), int.Parse(enddate.Substring(5, 2)), int.Parse(enddate.Substring(8, 2)));

            DateTime date_i;
            if (startDate_dt.Year < 1995)
                date_i = new DateTime(1995, 1, 1);
            else
                date_i = new DateTime(startDate_dt.Year, startDate_dt.Month, 1);

            for (; date_i <= endDate_dt; date_i = date_i.AddMonths(1))
            {
                requestString = RequestProducer.GetEarhquakeRequestString(EarthquakeDataSource.CWB, date_i.ToString(), enddate, min_magnitude, max_magnitude);
                request = (HttpWebRequest)WebRequest.Create(requestString);
                request.Method = WebRequestMethods.Http.Get;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var stream = response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {

                            data = reader.ReadToEnd();

                            EqList.AddRange(EarthquakeObjConverter.EarthquakeTrans(data, EarthquakeDataSource.CWB, startdate, enddate));

                            reader.Close();
                            reader.Dispose();
                        }
                    }
                }
            }

            return EqList;
        }
    }
}