using System;
using System.Collections.Generic;

namespace TestBlazor.Models.Response
{
    public class UrlResponseModel
    {
        public List<Uri> Images = new List<Uri>();
        public Dictionary<string, int> WordCount = new Dictionary<string, int>();
        public string ErrorMessage { get; set; }
        public int ShowImages { get; set; }
    }
}
