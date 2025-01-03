using MopisCrawlingMod;
using System;
using System.Text.Json.Nodes;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.Common;

namespace MopisCrawlingMod {
    public class MopiCrawlModSystem : ModSystem {

        public static string version = "1.0.0"; // Find a way to load this from modinfo

        private static ICoreAPI _api;
        private static ICoreClientAPI _capi;
        private static ICoreServerAPI _sapi;
        private bool isCrouching = false;


        public override void Start(ICoreAPI api) {
            base.Start(api);
            _api = api;
        }
        public override void StartClientSide(ICoreClientAPI api) {
            base.StartClientSide(api);
            MessageLogger.Initialize(api);
            ConfigLoader.Initialize(api);

            ConfigLoader.ApplyConfig();

            _capi = api;

            _capi.RegisterEntityBehaviorClass("crawling", typeof(EntityBehaviorPlayerCrawl));

            // Register for key press events
            _capi.Input.RegisterHotKey("crawlKey", "Crawl Key", GlKeys.LAlt);
            _capi.Input.SetHotKeyHandler("crawlKey", OnCrawlKeyPressed);
        }

        public override void StartServerSide(ICoreServerAPI api) {
            base.StartServerSide(api);
            _sapi = api;

            _sapi.RegisterEntityBehaviorClass("crawling", typeof(EntityBehaviorPlayerCrawl));
        }

        public static bool OnCrawlKeyPressed(KeyCombination comb) {
            EntityPlayer entityPlayer = _capi.World.Player.Entity;

            if (!entityPlayer.HasBehavior<EntityBehaviorPlayerCrawl>()) {
                _capi.World.Player.ShowChatNotification("[ERROR][Mopi's Crawling]: Crawl Behavior Not Found");
                return false; 
            } 

            entityPlayer.GetBehavior<EntityBehaviorPlayerCrawl>().ToggleCrawling(_capi.World.Player.Entity);

            return true;
        }

    }
}
