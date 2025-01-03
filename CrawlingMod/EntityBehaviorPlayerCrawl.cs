using MopisCrawlingMod;
using System;
using System.Numerics;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;


namespace MopisCrawlingMod {
    internal class EntityBehaviorPlayerCrawl : EntityBehavior {

        bool isCrawling;
        float crawlSpeedModifier;
        Vec2f defaultHitboxSize;
        Vec2f crawlHitboxSize;

        double defaultEyeHeight;
        double crawlEyeHeight;

        public override string PropertyName() {
            return "crawling";
        }

        public EntityBehaviorPlayerCrawl(Entity entity) : base(entity) {}

        public override void Initialize(EntityProperties properties, JsonObject attributes) {
            base.Initialize(properties, attributes);

            isCrawling = false;
            crawlSpeedModifier = attributes["crawlSpeedModifier"].AsFloat(-0.8f);

            defaultHitboxSize = entity.Properties.CollisionBoxSize;
            crawlHitboxSize = new Vec2f(defaultHitboxSize.Y, defaultHitboxSize.X);

            defaultEyeHeight = entity.Properties.EyeHeight;
            crawlEyeHeight = defaultEyeHeight - 1;
        }

        public bool CanStand(Entity entity) {
            // Returns true if block above is non solid
            return !entity.World.BlockAccessor.IsSideSolid((int)entity.Pos.X, (int)entity.Pos.Y + 1, (int)entity.Pos.Z, BlockFacing.UP);
        }

        public void ToggleCrawling(Entity entity) {

            if (isCrawling && !CanStand(entity)) {
                MessageLogger.log("You cannot stand here");
                return;
            }

            isCrawling = !isCrawling;

            if (isCrawling) {
                entity.Properties.EyeHeight = crawlEyeHeight;
                entity.Properties.CollisionBoxSize = crawlHitboxSize;

                entity.Stats.Set("walkspeed", "crawlSpeedModifier", crawlSpeedModifier, true);

                entity.StartAnimation("mopi-crawl");

            } else {
                
                entity.Properties.EyeHeight = defaultEyeHeight;
                entity.Properties.CollisionBoxSize = defaultHitboxSize;

                entity.Stats.Remove("walkspeed", "crawlSpeedModifier");

                entity.StopAnimation("mopi-crawl");
            }
        }

    }
}
