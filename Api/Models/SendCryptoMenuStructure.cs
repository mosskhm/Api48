
using System.Collections.Generic;


namespace Api.Models
{
   

    public class SendCryptoRootMenu
    {
        public int id { get; set; }
        public string title { get; set; }
        public string uri { get; set; }
        public List<Step> steps { get; set; }
    }

    public class Step
    {
        public int id { get; set; }
        public string title { get; set; }
        public bool options { get; set; }
        public List<string> optionsValue { get; set; }
        public bool textField { get; set; }
        public bool exit { get; set; }
    }
}
