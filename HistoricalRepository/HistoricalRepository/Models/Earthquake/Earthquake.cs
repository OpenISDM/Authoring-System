namespace HistoricalRepository.Models.Earthquake
{
    public class EarthquakeObj
    {

        // timestring
        // Time zone : GMT+8 
        // Format : YYYYMMDDhhmm
        // Example : 2016.03.01 14:36 (GMT+8) -> 201603011436
        public string timeString { get; private set; }
        public string lat { get; private set; }
        public string lng { get; private set; }

        // Depth
        // Unit : KM
        public string depth { get; private set; }
        public string magnitude { get; private set; }
        public EarthquakeDataSource dataSource { get; private set; }
        public string data { get; private set; }

        public string dataLink { get; private set; }

        public EarthquakeObj(string timeString, string lat, string lng, string depth, string magnitude, EarthquakeDataSource dataSource, string data, string dataLink)
        {
            this.timeString = timeString;
            this.lat = lat;
            this.lng = lng;
            this.depth = depth;
            this.magnitude = magnitude;
            this.dataSource = dataSource;
            this.data = data;
            this.dataLink = dataLink;
        }

        public EarthquakeObj(string timeString, string lat, string lng, string depth, string magnitude, EarthquakeDataSource dataSource, string dataLink)
        {
            this.timeString = timeString;
            this.lat = lat;
            this.lng = lng;
            this.depth = depth;
            this.magnitude = magnitude;
            this.dataSource = dataSource;
            this.dataLink = dataLink;
        }

    }
}