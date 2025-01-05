using MopisCrawlingMod;
using System;
using System.Text.Json.Nodes;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.Common;

namespace MopisCrawlingMod {
    public class MopiCrawlModSystem : ModSystem {

        public static string version = "1.0.1"; // Find a way to load this from modinfo

        private static ICoreAPI _api;
        private static ICoreClientAPI _capi;
        private static ICoreServerAPI _sapi;

        public override void Start(ICoreAPI api) {
            base.Start(api);
            _api = api;

            _api.RegisterEntityBehaviorClass("crawling", typeof(EntityBehaviorPlayerCrawl));
        }
        public override void StartClientSide(ICoreClientAPI api) {
            base.StartClientSide(api);
            MessageLogger.Initialize(api);
            ConfigLoader.Initialize(api);

            ConfigLoader.ApplyConfig();

            _capi = api;            

            // Register for key press events
            _capi.Input.RegisterHotKey("crawlKey", "Crawl Key", GlKeys.LAlt);
            _capi.Input.SetHotKeyHandler("crawlKey", OnCrawlKeyPressed);
        }

        public override void StartServerSide(ICoreServerAPI api) {
            base.StartServerSide(api);
            _sapi = api;
        }

        public override void AssetsLoaded(ICoreAPI api) {
            base.AssetsLoaded(api);
        }

        public static bool OnCrawlKeyPressed(KeyCombination comb) {
            EntityPlayer entityPlayer = _capi.World.Player.Entity;

            if (!entityPlayer.HasBehavior<EntityBehaviorPlayerCrawl>()) {
                MessageLogger.log("[ERROR][Mopi's Crawling]: Crawl Behavior Not Found");
                return false; 
            } 

            entityPlayer.GetBehavior<EntityBehaviorPlayerCrawl>().ToggleCrawling();

            return true;
        }

    }
}
