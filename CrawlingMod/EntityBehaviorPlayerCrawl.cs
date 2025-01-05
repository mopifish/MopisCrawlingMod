using MopisCrawlingMod;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using Vintagestory.ServerMods.NoObf;


namespace MopisCrawlingMod {
    internal class EntityBehaviorPlayerCrawl : EntityBehavior {

        bool isCrawling;
        float crawlSpeedModifier;
        Vec2f defaultHitboxSize;
        Vec2f sneakingHitboxSize;
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
            sneakingHitboxSize = new Vec2f(defaultHitboxSize.X, defaultHitboxSize.Y - 0.5f);

            defaultEyeHeight = entity.Properties.EyeHeight;
            crawlEyeHeight = defaultEyeHeight - 1;
        }

        public override void OnEntitySpawn() {
            base.OnEntitySpawn();

            if (!CanStand()) { StartCrawling(); }
            MessageLogger.log("Loaded");
        }

        private static bool IsBoxesColliding(Cuboidf a, Cuboidf b) {
            // AABB Collision check
            return (
                a.X1 <= b.X2 &&
                a.X2 >= b.X1 &&
                a.Y1 <= b.Y2 &&
                a.Y2 >= b.Y1 &&
                a.Z1 <= b.Z2 &&
                a.Z2 >= b.Z1
              );
        }

        public override void AfterInitialized(bool onFirstSpawn) {
            base.AfterInitialized(onFirstSpawn);
            if (!CanStand()) { StartCrawling(); }
        }

        public override void OnEntityLoaded() {
            base.OnEntityLoaded();
        }

        public bool CanStand() {
            // Gets block above
            BlockPos topPos = new BlockPos((int)entity.Pos.X, (int)entity.Pos.Y + 1, (int)entity.Pos.Z, entity.Pos.Dimension);
            Block topBlock = entity.World.BlockAccessor.GetBlock(topPos, BlockLayersAccess.Solid);

            // Gets current block (in case youre inside of a chiselled block)
            BlockPos bottomPos = new BlockPos((int)entity.Pos.X, (int)entity.Pos.Y, (int)entity.Pos.Z, entity.Pos.Dimension);
            Block bottomBlock = entity.World.BlockAccessor.GetBlock(bottomPos, BlockLayersAccess.Solid);

            Cuboidf[] topColliders = topBlock.GetCollisionBoxes(entity.World.BlockAccessor, topPos);
            Cuboidf[] bottomColliders = bottomBlock.GetCollisionBoxes(entity.World.BlockAccessor, bottomPos);

            // Check bottom block first
            if (bottomColliders != null) {
                // == Get players "standing" collider for checks
                EntityProperties props = new EntityProperties(); // Going through props so I'm relying on internal logic. Hopefully future proof?
                props.CollisionBoxSize = sneakingHitboxSize;
                Cuboidf playerCollider = props.SpawnCollisionBox.OffsetCopy(Math.Abs((float)entity.Pos.X % 1), 0, Math.Abs((float)entity.Pos.Z % 1)); // NOTE: Y offset is -1, this is because player is more than 1 block tall

                foreach (Cuboidf collider in bottomColliders) {
                    if (IsBoxesColliding(collider, playerCollider)) { return false; }
                }
            }

            // If bottom bock is clear, check top block
            if (topColliders != null) {
                // == Get players "standing" collider for checks
                EntityProperties props = new EntityProperties(); // Going through props so I'm relying on internal logic. Hopefully future proof?
                props.CollisionBoxSize = sneakingHitboxSize;
                Cuboidf playerCollider = props.SpawnCollisionBox.OffsetCopy(Math.Abs((float)entity.Pos.X % 1), -1f, Math.Abs((float)entity.Pos.Z % 1)); // NOTE: Y offset is -1, this is because player is more than 1 block tall

                foreach (Cuboidf collider in topColliders) {
                    if (IsBoxesColliding(collider, playerCollider)) { return false; }
                }
            }

            // All blocks clear, return true
            return true;
        }

        public void StartCrawling() {
            entity.Properties.EyeHeight = crawlEyeHeight;
            entity.Properties.CollisionBoxSize = crawlHitboxSize;

            entity.Stats.Set("walkspeed", "crawlSpeedModifier", crawlSpeedModifier, true);

            entity.StartAnimation("mopi-crawl");
        }

        public void StopCrawling() {
            entity.Properties.EyeHeight = defaultEyeHeight;
            entity.Properties.CollisionBoxSize = defaultHitboxSize;

            entity.Stats.Remove("walkspeed", "crawlSpeedModifier");

            entity.StopAnimation("mopi-crawl");
        }

        public void ToggleCrawling() {
            if (isCrawling && !CanStand()) {
                MessageLogger.log("You cannot stand here");
                return;
            }

            isCrawling = !isCrawling;

            (isCrawling ? (Action)StartCrawling : StopCrawling)();
        }
    }
}
