﻿using System.Collections.Generic;

namespace APIMocker
{
    public class APIDetailOptions
    {
        public string Path { get; set; }
        public string Method { get; set; } = "GET";
        public string QueryString { get; set; } = "";
        public string ResponseBody { get; set; }
        public string ResponseBodyFile { get; set; } = "";
        public int StatusCode { get; set; }
        public Dictionary<string, string> ResponseHeaders { get; set; }
    }
}
