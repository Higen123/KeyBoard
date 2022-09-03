using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Keyboard
{

    public class BindEventModel
    {
        public string game { get; set; }
        [JsonPropertyName("event")]
        public string _event { get; set; }
        public int min_value { get; set; }
        public int max_value { get; set; }
        public int icon_id { get; set; }
        public Handler[] handlers { get; set; }
    }

    public class Handler
    {
        [JsonPropertyName("device-type")]
        public string devicetype { get; set; }
        public string zone { get; set; }
        public Color color { get; set; }
        public string mode { get; set; }
    }

    public class Color
    {
        public Gradient gradient { get; set; }
    }

    public class Gradient
    {
        public Zero zero { get; set; }
        public Hundred hundred { get; set; }
    }

    public class Zero
    {
        public int red { get; set; }
        public int green { get; set; }
        public int blue { get; set; }
    }

    public class Hundred
    {
        public int red { get; set; }
        public int green { get; set; }
        public int blue { get; set; }
    }

}
