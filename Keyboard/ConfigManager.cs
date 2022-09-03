using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyboard
{
    public class ConfigManager
    {

        public Appconfig ReadConfig()
        {
            Appconfig config = new Appconfig();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("AppSettings.json", optional: false)
                .Build();

            var configsCollection = configuration.GetSection("AppConfig");
            config.Timer = double.Parse( configsCollection["Timer"]);
            config.BindEvent = configsCollection["BindEvent"];
            config.FireEvent = configsCollection["FireEvent"];
            config.RemoveEvent = configsCollection["RemoveEvent"];
            config.ContinueEvents = configsCollection["ContinueEvents"];
            config.LogFileName = configsCollection["LogFileName"];

            return config;
        }

    }
}
