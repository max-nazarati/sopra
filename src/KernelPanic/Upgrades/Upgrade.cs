using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Purchasing;
using Newtonsoft.Json;

namespace KernelPanic.Upgrades
{
    internal struct Upgrade : IPriced
    {
        internal enum Id : byte
        {
            /// <summary>
            /// Exists so that <c>new Upgrade()</c> constructs an invalid upgrade.
            /// </summary>
            Invalid,

            // Tier 1 Upgrades.
            BeginningTier1,
            IncreaseLp1 = BeginningTier1,
            IncreaseGs1,
            IncreaseVs1,
            DecreaseAi1,

            // Tier 2 Upgrades.
            BeginningTier2,
            IncreaseLp2 = BeginningTier2,
            IncreaseGs2,
            IncreaseVs2,
            IncreaseBitcoins,

            // Tier 3 Upgrades.
            BeginningTier3,
            CdBoomerang = BeginningTier3,
            IncreaseGsNokia,
            IncreaseGsFirefox,
            MoreTrojanChildren,

            // Tier 4
            BeginningTier4,
            // ...
            
            // Tier 5
            BeginningTier5,
            // ...

            EndOfUpgrades
        }

        [JsonProperty]
        internal Id Kind { get; private set; }

        [JsonIgnore]
        public Currency Currency => Currency.Experience;

        [JsonIgnore]
        public int Price => IdPrice(Kind);

        internal static int IdPrice(Id id)
        {
            if (id < Id.BeginningTier2)
                return 1;
            if (id < Id.BeginningTier3)
                return 2;
            if (id < Id.BeginningTier4)
                return 3;
            if (id < Id.BeginningTier5)
                return 4;
            if (id < Id.EndOfUpgrades)
                return 5;

            throw new InvalidOperationException("Invalid upgrade id " + id);
        }

        internal string Label => IdLabel(Kind);

        private static string IdLabel(Id id)
        {
            switch (id)
            {
                case Id.IncreaseLp1:
                    return "+5% LP";
                case Id.IncreaseGs1:
                    return "+5% GS";
                case Id.IncreaseVs1:
                    return "+5% VS";
                case Id.DecreaseAi1:
                    return "-5% AI";

                case Id.IncreaseLp2:
                    return "+10% LP";
                case Id.IncreaseGs2:
                    return "+10% GS";
                case Id.IncreaseVs2:
                    return "+10% VS";
                case Id.IncreaseBitcoins:
                    return "+10% mehr Bitcoin";

                case Id.CdBoomerang:
                    return "CD als Boomerang";
                case Id.IncreaseGsNokia:
                    return "+40% GS bei Nokia";
                case Id.IncreaseGsFirefox:
                    return "+10% GS bei Firefox";
                case Id.MoreTrojanChildren:
                    return "+5 Einheiten bei Trojaner";

                case Id.BeginningTier4:
                    return "TODO";
                    
                case Id.BeginningTier5:
                    return "TODO";

                case Id.Invalid:
                    goto default;
                case Id.EndOfUpgrades:
                    goto default;
                default:
                    throw new InvalidEnumArgumentException(nameof(id), (int) id, typeof(Id));
            }
        }

        internal void Apply(Entity entity)
        {
            var kind = Kind;
            void NotImplemented() =>
                Console.WriteLine("Applying Upgrade " + kind + " to " + entity + " – not implemented");

            switch (Kind)
            {
                case Id.IncreaseLp1:
                {
                    if (entity is Unit unit)
                        unit.MaximumLife = (int) (unit.MaximumLife * 0.05);
                    break;
                }

                case Id.IncreaseLp2:
                {
                    if (entity is Unit unit)
                        unit.MaximumLife = (int) (unit.MaximumLife * 0.05);
                    break;
                }

                case Id.IncreaseGs1:
                {
                    if (entity is Unit unit)
                        unit.MaximumLife = (int) (unit.MaximumLife * 0.05);
                    break;
                }

                case Id.IncreaseGs2:
                {
                    if (entity is Unit unit)
                        unit.MaximumLife = (int) (unit.MaximumLife * 0.05);
                    break;
                }

                case Id.IncreaseVs1:
                    NotImplemented();
                    break;
                case Id.DecreaseAi1:
                    NotImplemented();
                    break;
                case Id.IncreaseVs2:
                    NotImplemented();
                    break;
                case Id.IncreaseBitcoins:
                    // Nothing to do here for this upgrade.
                    break;
                case Id.CdBoomerang:
                    if (entity is CdThrower cdThrower)
                        cdThrower.ShootsBoomerang = true;
                    break;
                case Id.IncreaseGsNokia:
                    NotImplemented();
                    break;
                case Id.IncreaseGsFirefox:
                    NotImplemented();
                    break;
                case Id.MoreTrojanChildren:
                    NotImplemented();
                    break;
                case Id.BeginningTier4:
                    NotImplemented();
                    break;
                case Id.BeginningTier5:
                    NotImplemented();
                    break;

                case Id.EndOfUpgrades:
                    goto default;
                case Id.Invalid:
                    goto default;
                default:
                    throw new InvalidOperationException("Invalid upgrade ID " + Kind);
            }
        }

        internal static IEnumerable<IEnumerable<Upgrade>> Matrix =>
            new[]
            {
                UpgradesInRange(Id.BeginningTier1, Id.BeginningTier2),
                UpgradesInRange(Id.BeginningTier2, Id.BeginningTier3),
                UpgradesInRange(Id.BeginningTier3, Id.BeginningTier4),
                UpgradesInRange(Id.BeginningTier4, Id.BeginningTier5),
                UpgradesInRange(Id.BeginningTier5, Id.EndOfUpgrades)
            };

        private static IEnumerable<Upgrade> UpgradesInRange(Id start, Id stop) =>
            Enumerable.Range((int) start, stop - start).Select(id => new Upgrade {Kind = (Id) id});
    }
}
