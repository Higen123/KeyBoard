using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Keyboard
{

    public class RemoveModel
    {
        public string game { get; set; }

        [JsonPropertyName("event")]
        public string _event { get; set; }
    }

}
