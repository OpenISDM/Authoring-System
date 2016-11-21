﻿using System.Collections.Generic;
using System.Net;
using System.IO;
using HistoricalRepository.Models.Earthquake;

namespace HistoricalRepository.VirtualRepo
{
    static class EarthQuake_CKAN
    {
        public static List<EarthquakeObj> GetData(string startdate, string enddate, string min_magnitude, string max_magnitude)
        {
            string data = null;
            string requestString = RequestProducer.GetEarhquakeRequestString(EarthquakeDataSource.PALERT, startdate, enddate, min_magnitude, max_magnitude);

            List<EarthquakeObj> EqList = new List<EarthquakeObj>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestString);
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/json";

            requestString = RequestProducer.GetEarhquakeRequestString(EarthquakeDataSource.CKAN, startdate, enddate, min_magnitude, max_magnitude);
            request = (HttpWebRequest)WebRequest.Create(requestString);
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/json";

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {

                        data = reader.ReadToEnd();

                        EqList = EarthquakeObjConverter.EarthquakeTrans(data, EarthquakeDataSource.CKAN, startdate, enddate);

                        reader.Close();
                        reader.Dispose();

                    }
                }
            }

            return EqList;
        }
    }
}