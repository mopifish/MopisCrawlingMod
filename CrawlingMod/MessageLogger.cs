using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace MopisCrawlingMod {
    internal class MessageLogger {

        private static ICoreClientAPI _capi;

        public static bool isEnabled;

        public static void Initialize(ICoreClientAPI capi) {
            _capi = capi;
            isEnabled = true;
        }

        public static void log(string message) {
            if (!isEnabled) { return; }

            _capi.World.Player.ShowChatNotification("[Mopi's Crawling Mod]: " + message);
        }

    }
}
