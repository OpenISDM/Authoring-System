using HistoricalRepository.VirtualRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using HistoricalRepository.Models.Earthquake;


namespace HistoricalRepository.Controllers
{
    public class EarthquakeController : ApiController
    {

        List<EarthquakeObj> EARTHQUAKE_LIST = new List<EarthquakeObj>();

        public IHttpActionResult GetEarthquake([FromUri]QueryEarthquake qe)
        {

            EARTHQUAKE_LIST.Clear();

            string startdate = qe.startdate;
            string enddate = qe.enddate;
            string min_magnitude = qe.min_magnitude;
            string max_magnitude = qe.max_magnitude;

            //
            // To solve the problem that the time zone used by some repositories is in GMT + 0 but in GMT +8,
            // the search range need to be extended one day backward. 
            // For example, if the user searches the earthquakes in between 08/02 and 08/03 then we need to search from 08/01 to 08/03.
            // By this way we can cover the earthquakes in 08/01 16:00~24:00 in GMT + 0 time zone.
            // And then we will cut off the earthquakes that is out of the range after we convert them into GMT + 8.
            //

            DateTime startDate_dt = new DateTime(int.Parse(startdate.Substring(0, 4)), int.Parse(startdate.Substring(5, 2)), int.Parse(startdate.Substring(8, 2)));
            startdate = startDate_dt.AddDays(-1).ToString("yyyy-MM-dd");

            DateTime endDate_dt = new DateTime(int.Parse(enddate.Substring(0, 4)), int.Parse(enddate.Substring(5, 2)), int.Parse(enddate.Substring(8, 2)));

            //      !! ADD VIRTUAL REPOSITORY HERE !!
            //      !! ADD VIRTUAL REPOSITORY HERE !!
            //      !! ADD VIRTUAL REPOSITORY HERE !!
            //      !! ADD VIRTUAL REPOSITORY HERE !!

            EARTHQUAKE_LIST.AddRange(EarthQuake_PAlert.GetData(startdate, enddate, min_magnitude, max_magnitude));

            EARTHQUAKE_LIST.AddRange(EarthQuake_CWB.GetData(startdate, enddate, min_magnitude, max_magnitude));

            EARTHQUAKE_LIST.AddRange(EarthQuake_CKAN.GetData(startdate, enddate, min_magnitude, max_magnitude));

            EARTHQUAKE_LIST.AddRange(Earthquake_TESIS.GetData(startdate, enddate, min_magnitude, max_magnitude));

            // Sort by date
            EARTHQUAKE_LIST.Sort((x, y) => { return (x.timeString.CompareTo(y.timeString)); });



            // Cut the data which are not in the search range
            // (date, magnitude... etc)
            int eqListCount = EARTHQUAKE_LIST.Count();
            for (int i = 0; i < eqListCount; i++)
            {
                float eqMag = float.Parse(EARTHQUAKE_LIST[i].magnitude);

                DateTime eqTime = new DateTime(int.Parse(EARTHQUAKE_LIST[i].timeString.Substring(0, 4)),
                                int.Parse(EARTHQUAKE_LIST[i].timeString.Substring(4, 2)), int.Parse(EARTHQUAKE_LIST[i].timeString.Substring(6, 2)));

                // Some of the data in ckan will set their attribute "-1"
                // (do not move it)
                if (eqTime > endDate_dt || eqTime < startDate_dt || ((eqMag > float.Parse(max_magnitude) || eqMag < float.Parse(min_magnitude)) && eqMag != -1))
                {
                    EARTHQUAKE_LIST.RemoveAt(i);
                    eqListCount--;
                    i--;
                }
            }

            if (EARTHQUAKE_LIST.Count == 0)
            {
                return NotFound();
            }
            return Ok(EARTHQUAKE_LIST);
        }
    }
}
