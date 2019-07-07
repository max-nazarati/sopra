using System.Collections.Generic;
using KernelPanic.Players;

namespace KernelPanic.Events
{
    internal sealed class Event
    {
        internal enum Id
        {
            LoadEmptySlot,

            BuildingPlaced,
            BuildingImproved,
            BuildingSold,
            
            UpgradeBought,
            
            BoughtUnit,
            DamagedUnit,
            KilledUnit,

            DamagedBase,
            GameWon,
            GameLost,

            FirefoxJump
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
            /// <item><description><see cref="Id.DamagedUnit"/></description></item>
            /// <item><description><see cref="Id.KilledUnit"/></description></item>
            /// <item><description><see cref="Id.DamagedBase"/></description></item>
            /// </list>
            /// </summary>
            Unit,

            /// <summary>
            /// Of type <see cref="Microsoft.Xna.Framework.Point"/>, applies to
            /// <list type="bullet">
            /// <item><description><see cref="Id.BuildingPlaced"/></description></item>
            /// <item><description><see cref="Id.BuildingSold"/></description></item>
            /// </list>
            /// </summary>
            Tile,

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
            Damage
        }

        #region Creating events

        private Event(Id id)
        {
            Kind = id;
        }

        internal static Event GameWon() => new Event(Id.GameWon);
        internal static Event GameLost() => new Event(Id.GameLost);

        #endregion

        #region Event data

        internal Id Kind { get; }

        private readonly Dictionary<Key, object> mPayload = new Dictionary<Key, object>();

        internal T Get<T>(Key key) => (T) mPayload[key];

        #endregion
    }
}