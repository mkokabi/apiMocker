namespace APIMocker
{
    public class APIDetailOptions
    {
        public string Path { get; set; }
        public string Method { get; set; } = "GET";
        public string QueryString { get; set; }
        public string ResponseBody { get; set; }
        public int StatusCode { get; set; }
    }
}
