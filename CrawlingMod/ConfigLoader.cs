using MopisCrawlingMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace MopisCrawlingMod {
    internal class ConfigLoader {

        private static ICoreAPI _api;
        private static string FileName = "mopiscrawlingmod_config.json";
        

        public static void Initialize(ICoreAPI api) {
            _api = api;
        }

        public static Config LoadConfig() {
            Config modConfig = _api.LoadModConfig<Config>(FileName);
            if (modConfig == null || modConfig.version != MopiCrawlModSystem.version) {
                modConfig = GenerateConfig();
            }
            return modConfig;
        }

        public static void ApplyConfig() {
            Config config = LoadConfig();

            MessageLogger.isEnabled = config.MessageLoggingEnabled;
        }

        private static Config GenerateConfig() {
            Config modConfig = new Config {
                version = MopiCrawlModSystem.version,
                MessageLoggingEnabled = true
            };

            _api.StoreModConfig<Config>(modConfig, FileName);
            return modConfig;
        }
    }
}
