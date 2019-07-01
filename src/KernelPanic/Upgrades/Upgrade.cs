using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Entities;

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

        internal Id Kind { get; private set; }

        public Currency Currency => Currency.Experience;
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
                    NotImplemented();
                    break;
                case Id.CdBoomerang:
                    NotImplemented();
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

        internal static IEnumerable<Upgrade> All =>
            Enumerable
                .Range((int) Id.BeginningTier1, Id.EndOfUpgrades - Id.BeginningTier1)
                .Select(id => new Upgrade {Kind = (Id) id});
    }
}