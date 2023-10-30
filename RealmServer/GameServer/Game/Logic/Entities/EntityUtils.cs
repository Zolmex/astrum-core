using Common;
using GameServer.Game.Logic.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Logic.Entities
{
    public static class EntityUtils
    {
        private const float MinAttackMult = 0.5f;
        private const float MaxAttackMult = 2f;
        private const float MinAttackFreq = 0.0015f;
        private const float MaxAttackFreq = 0.008f;

        public static float DistSqr(this Entity a, float x, float y)
        {
            var dx = a.Position.X - x;
            var dy = a.Position.Y - y;
            return dx * dx + dy * dy;
        }

        public static float DistSqr(this Entity a, Entity b)
        {
            var dx = a.Position.X - b.Position.X;
            var dy = a.Position.Y - b.Position.Y;
            return dx * dx + dy * dy;
        }

        public static double TileDistSqr(this Entity a, float tileX, float tileY)
        {
            var dx = (int)a.Position.X - (int)tileX;
            var dy = (int)a.Position.Y - (int)tileY;
            return dx * dx + dy * dy;
        }

        public static bool IsStandingOn(this Entity en, IEnumerable<WorldTile> tiles)
        {
            return tiles.Contains(en.Tile);
        }

        //public static IEnumerable<Entity> GetNearestEntities(this Entity entity, float radius, bool byTile = false)
        //    => entity.World.ChunkManager.HitTest(entity.X, entity.Y, radius)
        //        .Where(e => e.Dist(entity, byTile) < radius);

        //public static IEnumerable<T> GetNearestEntities<T>(this Entity entity, float radius, bool byTile = false) where T : Entity
        //    => entity.World.ChunkManager.HitTest<T>(entity.X, entity.Y, radius)
        //        .Where(e => e.Dist(entity, byTile) < radius);

        //public static Entity GetNearestEntity(this Entity entity, float radius, bool byTile = false)
        //{
        //    var ret = GetNearestEntities(entity, radius, byTile);
        //    ret = ret.OrderBy(e => entity.Dist(e, byTile));
        //    return ret.FirstOrDefault();
        //}

        //public static T GetNearestEntity<T>(this Entity entity, float radius, bool byTile = false) where T : Entity
        //{
        //    var ret = GetNearestEntities<T>(entity, radius, byTile);
        //    ret = ret.OrderBy(e => entity.Dist(e, byTile));
        //    return ret.FirstOrDefault();
        //}

        //public static float GetSpeed(this Character entity, float speed)
        //    => entity.HasConditionEffect(ConditionEffectIndex.Slowed) ? (5.55f * speed + 0.74f) / 2 : 5.55f * speed + 0.74f;

        //public static int GetDefenseDamage(this Character entity, int damage, int def, bool ignoreDef)
        //{
        //    if (entity.HasAnyConditionEffects(ConditionEffectIndex.Invulnerable, ConditionEffectIndex.Invincible))
        //        return 0;

        //    if (entity.HasConditionEffect(ConditionEffectIndex.ArmorBroken) || ignoreDef)
        //        return damage;

        //    // curse check

        //    if (entity.HasConditionEffect(ConditionEffectIndex.Armored))
        //        def *= 2;

        //    var min = (int)(damage * 0.15);
        //    return Math.Max(damage - def, min);
        //}

        //public static int GetAttackDamage(this Character entity, int damage)
        //{
        //    if (entity.HasConditionEffect(ConditionEffectIndex.Weak))
        //        return (int)(damage * MinAttackMult);

        //    if (entity.HasConditionEffect(ConditionEffectIndex.Damaging))
        //        damage = (int)(damage * 1.5);

        //    return damage;
        //}

        //public static int GetAttackDamage(this Player player, int min, int max, bool isAbility)
        //{
        //    var damage = (int)player.User.Random.NextIntRange((uint)min, (uint)max);
        //    damage = GetAttackDamage(player, damage);
        //    var mult = isAbility ? 1 : MinAttackMult + player.Attack / 75.0 * (MaxAttackMult - MinAttackMult);
        //    return (int)(damage * mult);
        //}

        //public static float GetAttackFrequency(this Player player)
        //{
        //    if (player.HasConditionEffect(ConditionEffectIndex.Dazed))
        //        return MinAttackFreq;

        //    var ret = MinAttackFreq + player.Dexterity / 75 * (MaxAttackFreq - MinAttackFreq);
        //    if (player.HasConditionEffect(ConditionEffectIndex.Berserk))
        //        ret *= 1.5f;

        //    return ret;
        //}

        //public static int GetAttackPeriod(this Player player, float rof)
        //    => (int)(1 / player.GetAttackFrequency() * (1 / rof));
    }
}
