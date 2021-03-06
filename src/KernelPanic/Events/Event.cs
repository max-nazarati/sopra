using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Projectiles;
using KernelPanic.Entities.Units;
using KernelPanic.Players;
using KernelPanic.Table;
using KernelPanic.Tracking;
using KernelPanic.Upgrades;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KernelPanic.Events
{
    internal sealed class Event
    {
        [JsonConverter(typeof(StringEnumConverter))]
        internal enum Id
        {
            AchievementUnlocked,
            AchievementImpossible,
            CloseAchievementPool,

            LoadEmptySlot,

            BuildingPlaced,
            BuildingImproved,      // TODO: Not sent yet.
            BuildingSold,
            
            UpgradeBought,
            
            BoughtUnit,
            SpawnedUnit,
            DamagedUnit,
            KilledUnit,
            ProjectileShot,
            ButtonClicked,

            DamagedBase,
            GameWon,
            GameLost,

            BitcoinChanged,

            SetupEnded,
            TechDemoStarted,
            TechDemoClosed,

            FirefoxJump,
            HeroAbility
        }

        [JsonConverter(typeof(StringEnumConverter))]
        internal enum Key
        {
            /// <summary>
            /// Of type <see cref="Player"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.GameWon"/></description></item>
            /// <item><description><see cref="Id.GameLost"/></description></item>
            /// </list>
            /// </summary>
            Winner,
            
            /// <summary>
            /// Of type <see cref="KernelPanic.Entities.Buildings.Tower"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.ProjectileShot"/></description></item>
            /// </list>
            /// </summary>
            Tower,

            /// <summary>
            /// Of type <see cref="Player"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.GameWon"/></description></item>
            /// <item><description><see cref="Id.GameLost"/></description></item>
            /// </list>
            /// </summary>
            Loser,

            /// <summary>
            /// Of type <see cref="Player"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.BuildingPlaced"/></description></item>
            /// <item><description><see cref="Id.BuildingImproved"/></description></item>
            /// <item><description><see cref="Id.BuildingSold"/></description></item>
            /// <item><description><see cref="Id.UpgradeBought"/></description></item>
            /// <item><description><see cref="Id.BoughtUnit"/></description></item>
            /// <item><description><see cref="Id.BitcoinChanged"/></description></item>
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
            /// <item><description><see cref="Id.SpawnedUnit"/></description></item>
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
            /// <item><description><see cref="Id.SpawnedUnit"/></description></item>
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
            /// <item><description><see cref="Id.BoughtUnit"/></description></item>
            /// <item><description><see cref="Id.BuildingPlaced"/></description></item>
            /// <item><description><see cref="Id.BuildingImproved"/></description></item>
            /// <item><description><see cref="Id.UpgradeBought"/></description></item>
            /// <item><description><see cref="Id.BitcoinChanged"/></description></item>
            /// </list>
            /// </summary>
            Price,
            
            /// <summary>
            /// Of type <see cref="Hero"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.HeroAbility"/></description></item>
            /// </list>
            /// </summary>
            Hero,

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
            /// <item><description><see cref="Id.AchievementImpossible"/></description></item>
            /// </list>
            /// </summary>
            Achievement,

            /// <summary>
            /// Of type <see cref="AchievementPool"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.CloseAchievementPool"/></description></item>
            /// </list>
            /// </summary>
            AchievementPool
        }

        #region Creating events

        private Event(Id id)
        {
            Kind = id;
        }

        internal static Event LoadEmptySlot() => new Event(Id.LoadEmptySlot);

        internal static Event GameWon(Player winner, Player loser) =>
            new Event(Id.GameWon)
            {
                mPayload =
                {
                    [Key.Winner] = winner,
                    [Key.Loser] = loser
                }
            };

        internal static Event ProjectileShot(Tower tower) =>
            new Event(Id.ProjectileShot)
            {
                mPayload =
                {
                    [Key.Tower] = tower,
                }
            };

        internal static Event GameLost(Player winner, Player loser) =>
            new Event(Id.GameLost)
            {
                mPayload =
                {
                    [Key.Winner] = winner,
                    [Key.Loser] = loser
                }
            };

        internal static Event BitcoinChanged(Player player) =>
            new Event(Id.BitcoinChanged)
            {
                mPayload =
                {
                    [Key.Buyer] = player,
                    [Key.Price] = player.Bitcoins
                }
            };

        internal static Event ButtonClicked() => new Event(Id.ButtonClicked);

        internal static Event HeroAbility(Hero hero) =>
            new Event(Id.HeroAbility)
            {
                mPayload =
                {
                    [Key.Hero] = hero
                }
            };

        internal static Event FirefoxJumped(Owner owner, Firefox firefox) =>
            new Event(Id.FirefoxJump)
            {
                mPayload =
                {
                    [Key.Attacker] = owner[firefox],
                }
            };

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
        /*    
        internal static Event SpawnedUnit(Player attacker, Unit unit) =>
            new Event(Id.SpawnedUnit)
            {
                mPayload =
                {
                    [Key.Attacker] = attacker,
                    [Key.Unit] = unit
                }
            }; */

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

        internal static Event AchievementImpossible(Achievement achievement) =>
            new Event(Id.AchievementImpossible)
            {
                mPayload =
                {
                    [Key.Achievement] = achievement
                }
            };

        internal static Event CloseAchievementPool(AchievementPool achievementPool) =>
            new Event(Id.CloseAchievementPool)
            {
                mPayload =
                {
                    [Key.AchievementPool] = achievementPool
                }
            };

        internal static Event TechDemoStarted() => new Event(Id.TechDemoStarted);

        internal static Event TechDemoClosed() => new Event(Id.TechDemoClosed);

        internal static Event SetupEnded() =>
            new Event(Id.SetupEnded);

        #endregion

        #region Event data

        internal Id Kind { get; }

        private readonly Dictionary<Key, object> mPayload = new Dictionary<Key, object>();

        internal T Get<T>(Key key) => (T) mPayload[key];

        internal bool IsActivePlayer(Key key) => Get<Player>(key).Select(true, false);

        #endregion
    }
}