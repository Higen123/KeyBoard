using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Keyboard
{

    public class FireEventModel
    {
        public string game { get; set; }

        [JsonPropertyName("event")]
        public string _event { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public int value { get; set; }
    }

}
