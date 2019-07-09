using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Entities.Projectiles;
using KernelPanic.Players;
using KernelPanic.Table;
using KernelPanic.Tracking;
using KernelPanic.Upgrades;

namespace KernelPanic.Events
{
    internal sealed class Event
    {
        internal enum Id
        {
            AchievementUnlocked,

            LoadEmptySlot,         // TODO: Not sent yet.

            BuildingPlaced,
            BuildingImproved,      // TODO: Not sent yet.
            BuildingSold,
            
            UpgradeBought,
            
            BoughtUnit,
            DamagedUnit,
            KilledUnit,

            DamagedBase,
            GameWon,
            GameLost,

            // FirefoxJump            // TODO: Not sent yet.
        }

        internal enum Key
        {
            /// <summary>
            /// Of type <see cref="Player"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.BuildingPlaced"/></description></item>
            /// <item><description><see cref="Id.BuildingImproved"/></description></item>
            /// <item><description><see cref="Id.BuildingSold"/></description></item>
            /// <item><description><see cref="Id.UpgradeBought"/></description></item>
            /// <item><description><see cref="Id.BoughtUnit"/></description></item>
            /// </list>
            /// </summary>
            Buyer,

            /// <summary>
            /// Of type <see cref="Player"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.DamagedUnit"/></description></item>
            /// <item><description><see cref="Id.KilledUnit"/></description></item>
            /// <item><description><see cref="Id.DamagedBase"/></description></item>
            /// </list>
            /// </summary>
            Defender,

            /// <summary>
            /// Of type <see cref="Player"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.DamagedUnit"/></description></item>
            /// <item><description><see cref="Id.KilledUnit"/></description></item>
            /// <item><description><see cref="Id.DamagedBase"/></description></item>
            /// <item><description><see cref="Id.FirefoxJump"/></description></item>
            /// </list>
            /// </summary>
            Attacker,

            /// <summary>
            /// Of type <see cref="KernelPanic.Entities.Building"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.BuildingPlaced"/></description></item>
            /// <item><description><see cref="Id.BuildingSold"/></description></item>
            /// </list>
            /// 
            /// Of type <see cref="KernelPanic.Entities.Buildings.Tower"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.BuildingImproved"/></description></item>
            /// <item><description><see cref="Id.DamagedUnit"/></description></item>
            /// <item><description><see cref="Id.KilledUnit"/></description></item>
            /// </list>
            /// </summary>
            Building,

            /// <summary>
            /// Of type <see cref="KernelPanic.Entities.Unit"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.BoughtUnit"/></description></item>
            /// <item><description><see cref="Id.DamagedUnit"/></description></item>
            /// <item><description><see cref="Id.KilledUnit"/></description></item>
            /// <item><description><see cref="Id.DamagedBase"/></description></item>
            /// </list>
            /// </summary>
            Unit,

            /// <summary>
            /// Of type <see cref="KernelPanic.Upgrades.Upgrade"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.UpgradeBought"/></description></item>
            /// </list>
            /// </summary>
            Upgrade,

            /// <summary>
            /// Of type <see cref="int"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.FirefoxJump"/></description></item>
            /// </list>
            /// </summary>
            CrossedBuildingsCount,

            /// <summary>
            /// Of type <see cref="int"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.BoughtUnit"/></description></item>
            /// <item><description><see cref="Id.BuildingPlaced"/></description></item>
            /// <item><description><see cref="Id.BuildingImproved"/></description></item>
            /// <item><description><see cref="Id.UpgradeBought"/></description></item>
            /// </list>
            /// </summary>
            Price,

            /// <summary>
            /// Of type <see cref="int"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.DamagedUnit"/></description></item>
            /// <item><description><see cref="Id.DamagedBase"/></description></item>
            /// </list>
            /// </summary>
            Damage,
            
            /// <summary>
            /// Of type <see cref="Achievement"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.AchievementUnlocked"/></description></item>
            /// </list>
            /// </summary>
            Achievement
        }

        #region Creating events

        private Event(Id id)
        {
            Kind = id;
        }

        internal static Event GameWon() => new Event(Id.GameWon);
        internal static Event GameLost() => new Event(Id.GameLost);

        internal static Event BuildingPlaced(Player buyer, Building building) =>
            new Event(Id.BuildingPlaced)
            {
                mPayload =
                {
                    [Key.Buyer] = buyer,
                    [Key.Building] = building,
                    [Key.Price] = building.Price
                }
            };

        internal static Event BuildingSold(Player buyer, Building building) =>
            new Event(Id.BuildingSold)
            {
                mPayload =
                {
                    [Key.Buyer] = buyer,
                    [Key.Building] = building,
                }
            };
        
        internal static Event UpgradeBought(Player buyer, Upgrade upgrade) =>
            new Event(Id.UpgradeBought)
            {
                mPayload =
                {
                    [Key.Buyer] = buyer,
                    [Key.Upgrade] = upgrade,
                    [Key.Price] = upgrade.Price
                }
            };
            
        internal static Event BoughtUnit(Player buyer, Unit unit) =>
            new Event(Id.BoughtUnit)
            {
                mPayload =
                {
                    [Key.Buyer] = buyer,
                    [Key.Unit] = unit,
                    [Key.Price] = unit.Price
                }
            };

        internal static Event DamagedUnit(Owner owner, Projectile projectile, Unit unit) =>
            new Event(Id.DamagedUnit)
            {
                mPayload =
                {
                    [Key.Attacker] = owner[unit],
                    [Key.Defender] = owner[projectile.Origin],
                    [Key.Building] = projectile.Origin,
                    [Key.Damage] = projectile.Damage,
                    [Key.Unit] = unit
                }
            };

        internal static Event KilledUnit(Owner owner, Projectile projectile, Unit unit) =>
            new Event(Id.KilledUnit)
            {
                mPayload =
                {
                    [Key.Attacker] = owner[unit],
                    [Key.Defender] = owner[projectile.Origin],
                    [Key.Building] = projectile.Origin,
                    [Key.Unit] = unit
                }
            };

        internal static Event DamagedBase(Owner owner, Unit unit) =>
            new Event(Id.DamagedBase)
            {
                mPayload =
                {
                    [Key.Attacker] = owner[unit],
                    [Key.Defender] = owner.Swapped[unit],
                    [Key.Damage] = unit.AttackStrength,
                    [Key.Unit] = unit
                }
            };

        internal static Event AchievementUnlocked(Achievement achievement) =>
            new Event(Id.AchievementUnlocked)
            {
                mPayload =
                {
                    [Key.Achievement] = achievement
                }
            };

        #endregion

        #region Event data

        internal Id Kind { get; }

        private readonly Dictionary<Key, object> mPayload = new Dictionary<Key, object>();

        internal T Get<T>(Key key) => (T) mPayload[key];

        #endregion
    }
}