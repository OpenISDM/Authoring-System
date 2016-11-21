using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using HistoricalRepository.Const;

namespace HistoricalRepository.Models.Earthquake
{
    static class EarthquakeObjConverter
    {

        private static List<EarthquakeObj> eqList = new List<EarthquakeObj>();
        private static string Domain;
        private static string LatLng;
        private static string Time;
        private static string Intensity;
        private static string startTime;
        private static string endTime;

        public static List<EarthquakeObj> EarthquakeTrans(object earthquakeEvent, EarthquakeDataSource dataSource,
                                                string startTime, string endTime)
        {
            EarthquakeObjConverter.eqList.Clear();
            EarthquakeObjConverter.startTime = startTime;
            EarthquakeObjConverter.endTime = endTime;

            if (earthquakeEvent == null)
                return null;

            switch (dataSource)
            {
                case EarthquakeDataSource.PALERT:
                    List<Palert> palertEventList = JsonConvert.DeserializeObject<List<Palert>>((string)earthquakeEvent);
                    foreach (Palert palertEvent in palertEventList)
                    {
                        EarthquakeObj eq = palertTrans(palertEvent);
                        eqList.Add(eq);
                    }
                    return eqList;

                case EarthquakeDataSource.CWB:
                    cwbTrans((string)earthquakeEvent);
                    return eqList;

                case EarthquakeDataSource.CKAN:
                    JObject ckanList = JObject.Parse((string)earthquakeEvent);
                    ckanTrans(ckanList);
                    return eqList;

                case EarthquakeDataSource.TESIS:
                    tesisTrans((string)earthquakeEvent);
                    return eqList;
                default:
                    break;
            }

            // DEBUG
            return null;
        }

        //
        // Transform palert object to earthquake object
        //
        private static EarthquakeObj palertTrans(Palert palertEvent)
        {

            // The time string format of palert : YYYYMMDDhhmmss
            // We only need YYYYMMDDhhmm
            // The time format in P-alert is UTC, we need to convert it into GMT+8       
            string timeString_ = palertEvent.Timestring;
            DateTime timeString_dt = new DateTime(int.Parse(timeString_.Substring(0, 4)), int.Parse(timeString_.Substring(4, 2)),
                                        int.Parse(timeString_.Substring(6, 2)), int.Parse(timeString_.Substring(8, 2)),
                                        int.Parse(timeString_.Substring(10, 2)), int.Parse(timeString_.Substring(12, 2)));

            // UTC time + 8 hours = GMT+8 (Time zone of Taiwan)
            timeString_dt = timeString_dt.AddHours(8);

            string timeString = timeString_dt.ToString("yyyyMMddHHmm");

            string lat = palertEvent.Lat;
            string lng = palertEvent.Lng;

            // The unit of depth of palert event is KM too.
            string depth = palertEvent.Depth;
            string magnitude = palertEvent.Magnitude;
            EarthquakeDataSource dataSource = EarthquakeDataSource.PALERT;

            // The format of the link of the p-alert data : Domain + Key
            //      Domain  :   http://palert.earth.sinica.edu.tw/db/index1.clt2.php?time=
            //      Key     :   YYYYMMDDhhmmss
            //      And the Key is equal to palertEvent.Timestring

            string Domain = "http://palert.earth.sinica.edu.tw/db/index1.clt2.php?time=";
            string Key = palertEvent.Timestring;

            string dataLink = Domain + Key;

            EarthquakeObj eq = new EarthquakeObj(timeString, lat, lng, depth, magnitude, dataSource, dataLink);

            return eq;

        }

        //
        // Transform CWB typhoon data to earthquake object
        //
        private static void cwbTrans(string htmlData)
        {
            bool datalistFound = false;
            bool firstRowSkip = false;
            string[] data = htmlData.Split('\n');

            string timeString;
            string lat;
            string lng;
            string depth;
            string magnitude;
            string dataLink;

            for (int i = 0; i < data.Length; i++)
            {

                // First, find the table datalist4
                if (data[i].Contains("datalist4"))
                {
                    datalistFound = true;
                    continue;
                }

                // Skip the first row of datalist4
                if (datalistFound && !firstRowSkip)
                {
                    if (data[i].Contains("<tr>"))
                    {
                        firstRowSkip = true;
                        continue;
                    }
                }

                // Every other rows are the data we need
                if (datalistFound && firstRowSkip)
                {
                    // The data in the row is like :
                    // ID, Time, Lat, Lng, Magnitude, Depth, Central
                    //
                    // And the HTML of the table row is like :
                    // <td>
                    // <a href="....."> THE DATA WE NEED </a>
                    // </td><td>
                    // <a href="....."> THE DATA WE NEED 2</a>
                    // ...
                    // </tr><tr>
                    if (data[i].Contains("<tr>"))
                    {
                        i++; // go to next line


                        // This line is <td>
                        // Skip.
                        i++;

                        // <a href="?ItemId=49&fileString=2016030119411338">EARTHQUAKE ID</a>
                        // We can get timestring here.
                        // We can also get data link here.
                        string[] timeStringtmp = data[i].Split(new char[] { '=', '\'' });
                        timeString = timeStringtmp[4].Substring(0, 12);
                        dataLink = "http://scweb.cwb.gov.tw/GraphicContent.aspx?ItemId=49&fileString=" + timeStringtmp[4];
                        i++;

                        // </td><td>
                        i++;


                        // Time, but we already got time string.
                        // Skip them.

                        //<a href="xxx">X月X日X時X分</a>
                        //</td><td>
                        i = i + 2;


                        // Longitude

                        // <a href="xxx">123.4</a>
                        string[] tmp = data[i].Split(new char[] { '>', '<' });
                        lng = tmp[2];
                        i++;

                        //</td><td>
                        i++;

                        // Latitude
                        // Same as above.
                        tmp = data[i].Split(new char[] { '>', '<' });
                        lat = tmp[2];
                        i = i + 2;

                        // Magnitude
                        tmp = data[i].Split(new char[] { '>', '<' });
                        magnitude = tmp[2];
                        i = i + 2;

                        // Depth
                        tmp = data[i].Split(new char[] { '>', '<' });
                        depth = tmp[2];



                        EarthquakeObj eq = new EarthquakeObj(timeString, lat, lng, depth, magnitude, EarthquakeDataSource.CWB, dataLink);
                        eqList.Add(eq);

                        continue;
                    }
                    else if (data[i].Contains("</table>"))
                        break;
                }
            }
        }

        //
        // Transform CKAN object to earthquake object
        //
        private static void ckanTrans(JObject ckanlist)
        {


            IList<JToken> results = ckanlist["result"]["packages"].Children().ToList();
            IList<Ckan> ckanList = new List<Ckan>();
            foreach (JToken result in results)
            {
                bool HasDetailFile = false;
                // The JSON file name format is equal to timestring
                string name = result["name"].ToString();
                string[] dateParse = (startTime + '&' + endTime).Split('&', '-');


                // Find the Dataset
                if (int.Parse(name.Substring(0, 8)) >= int.Parse(dateParse[0] + dateParse[1] + dateParse[2]) &&
                    int.Parse(name.Substring(0, 8)) <= int.Parse(dateParse[3] + dateParse[4] + dateParse[5]))
                {

                    // Go to the dataset and take out the earthquake data
                    //string dataset_url = "http://140.109.17.71/api/3/action/package_show?id=" + name;
                    string dataset_url = ConstCkan.CkanAPIPrefix + "/package_show?id=" + name;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(dataset_url);
                    request.Method = WebRequestMethods.Http.Get;
                    request.ContentType = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            using (var stream = response.GetResponseStream())
                            using (var reader = new StreamReader(stream))
                            {

                                string data = reader.ReadToEnd();

                                JObject datalist = JObject.Parse(data);
                                IList<JToken> datalist_JSON = datalist["result"]["resources"].Children().ToList();

                                foreach (JToken dataResult in datalist_JSON)
                                {
                                    string dataName = dataResult["name"].ToString();

                                    if (dataName == name)
                                    {
                                        HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(dataResult["url"].ToString());
                                        request2.Method = WebRequestMethods.Http.Get;
                                        request2.ContentType = "application/json";
                                        using (var response2 = (HttpWebResponse)request2.GetResponse())
                                        {
                                            if (response2.StatusCode == HttpStatusCode.OK)
                                            {
                                                using (var stream2 = response2.GetResponseStream())
                                                using (var reader2 = new StreamReader(stream2))
                                                {
                                                    string data2 = reader2.ReadToEnd();

                                                    Ckan ckan = JsonConvert.DeserializeObject<Ckan>(data2.ToString());
                                                    EarthquakeObj eq = new EarthquakeObj(ckan.Timestring, ckan.Lat, ckan.Lng, ckan.Depth, ckan.Magnitude, EarthquakeDataSource.CKAN, "http://" + ConstCkan.CkanIP + "/dataset/" + name);
                                                    eqList.Add(eq);
                                                    HasDetailFile = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!HasDetailFile)
                                {
                                    EarthquakeObj eq = new EarthquakeObj(name, "-1", "-1", "-1", "-1", EarthquakeDataSource.CKAN, "http://" + ConstCkan.CkanIP + "/dataset/" + name);
                                    eqList.Add(eq);
                                }

                                reader.Close();
                                reader.Dispose();
                            }
                        }
                    }
                }
            }
        }

        private static void tesisTrans(string tesisList)
        {

            //
            // The response of TESIS stores in tesisList.
            // Example : ["992","993","994","995","996","997"];
            // This means we need to call another API to get these events
            // Example : http://tesis.earth.sinica.edu.tw/common/php/processdatanew.php?firstid=992&secondid=997
            // And then we will got a JSON list of earthquakes.
            //


            // Get rid of symbols like '\', '"', '[', ']'
            string pattern = "[\\\"\\[\\]]";
            Regex regex = new Regex(pattern);

            // tesisList_ will contain all the earthquake event ID
            string[] tesisList_ = regex.Replace(tesisList, "").Split(',');

            //
            // Go to the dataset and take out the earthquake data
            // string api_url = "http://tesis.earth.sinica.edu.tw/common/php/processdatanew.php?" + firstid + '&' +  secondid;
            // firstid = firstid=992
            // secondid = secondid=997
            //
            // This API will output all the earthquake data between id 992 and 997
            //

            string firstid = "firstid=" + tesisList_[0];
            string secondid = "secondid=" + tesisList_[tesisList_.Length - 1];
            string api_url = "http://tesis.earth.sinica.edu.tw/common/php/processdatanew.php?" + firstid + '&' + secondid;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api_url);
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/json";

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {

                        string data = reader.ReadToEnd();

                        JObject datalist = JObject.Parse(data);
                        IList<JToken> datalist_JSON = datalist["earthquakes"].Children().ToList();

                        foreach (JToken dataResult in datalist_JSON)
                        {
                            string eqData = dataResult["cwb"].ToString();

                            Tesis tesis = JsonConvert.DeserializeObject<Tesis>(eqData.ToString());

                            // The time zone of tesis.timestring is GMT + 0
                            // We need to add 8 hours to match the time zone of Taiwan (GMT + 8)

                            DateTime tesisDt = new DateTime(int.Parse(tesis.timestring.Substring(0, 4)), int.Parse(tesis.timestring.Substring(4, 2)), int.Parse(tesis.timestring.Substring(6, 2)),
                                int.Parse(tesis.timestring.Substring(8, 2)), int.Parse(tesis.timestring.Substring(10, 2)), int.Parse(tesis.timestring.Substring(12, 2)));
                            tesisDt = tesisDt.AddHours(8);
                            string timeString = tesisDt.ToString("yyyyMMddHHmm");

                            EarthquakeObj eq = new EarthquakeObj(timeString, tesis.Latitude, tesis.Longitude, tesis.Depth, tesis.ML, EarthquakeDataSource.TESIS,
                                "http://tesis.earth.sinica.edu.tw/showDetail.php?date=%22" + tesis.Date + "%22&time=%22" + tesis.Time + "%22");
                            eqList.Add(eq);

                        }
                        reader.Close();
                        reader.Dispose();
                    }
                }
            }


        }


    }
}