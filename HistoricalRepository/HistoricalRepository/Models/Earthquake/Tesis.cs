namespace HistoricalRepository.Models.Earthquake
{
    public class Tesis
    {
        // Tesis Data :
        // The time zone of Date / Time is in GMT + 8 (Time zone of Taiwan)
        // The time zone of timestring is in GMT + 0

        public string No { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Depth { get; set; }
        public string ML { get; set; }
        public string CWB_ID { get; set; }
        public string Vectorfile { get; set; }
        public string intensitymap { get; set; }
        public string timestring { get; set; }
    }
}