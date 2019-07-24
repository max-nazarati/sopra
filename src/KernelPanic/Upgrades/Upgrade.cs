using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Units;
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
            MoreTrojanChildren1,

            // Tier 4
            BeginningTier4,
            EmpDuration = BeginningTier4,
            AdditionalFirefox1,
            IncreaseSettingsArea1,
            IncreaseSettingsHeal1,
            MoreTrojanChildren2,

            // Tier 5
            BeginningTier5,
            EmpTwoTargets = BeginningTier5,
            AdditionalFirefox2,
            IncreaseSettingsArea2,
            IncreaseSettingsHeal2,

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

        [SuppressMessage("ReSharper", "StringLiteralTypo", Justification = "Strings are in German")]
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
                case Id.MoreTrojanChildren1:
                    return "+5 Einheiten bei Trojaner";

                case Id.EmpDuration:
                    return "+40% Dauer EMP";
                case Id.AdditionalFirefox1:
                    return "+1 Firefox verfügbar";
                case Id.IncreaseSettingsArea1:
                    return "+5% Einzugsbereich von Settings";
                case Id.IncreaseSettingsHeal1:
                    return "+5% Heilrate von Settings";
                case Id.MoreTrojanChildren2:
                    return "+10 Einheiten bei Trojaner";

                case Id.EmpTwoTargets:
                    return "Bluescreen trifft 2 Türme";
                case Id.AdditionalFirefox2:
                    return "+1 Firefox verfügbar";
                case Id.IncreaseSettingsArea2:
                    return "+10% Einzugsbereich von Settings";
                case Id.IncreaseSettingsHeal2:
                    return "+10% Heilrate von Settings";

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
                        {
                            if (unit.RemainingLife == unit.MaximumLife)
                                unit.RemainingLife = (int)(unit.MaximumLife * 1.05);
                            unit.MaximumLife = (int)(unit.MaximumLife * 1.05);
                        }
                    break;
                }

                case Id.IncreaseLp2:
                {
                    if (entity is Unit unit)
                        {
                            if (unit.RemainingLife == unit.MaximumLife)
                                unit.RemainingLife = (int)(unit.MaximumLife * 1.10);
                            unit.MaximumLife = (int)(unit.MaximumLife * 1.10);
                        }
                    break;
                }

                case Id.IncreaseGs1:
                {
                    if (entity is Unit unit)
                        unit.Speed *= 1.05f;
                    break;
                }

                case Id.IncreaseGs2:
                {
                    if (entity is Unit unit)
                        unit.Speed *= 1.10f;
                    break;
                }

                case Id.IncreaseVs1:
                {
                    if (entity is Tower tower)
                        tower.Damage = (int) (tower.Damage * 1.05);
                    break;
                }

                case Id.IncreaseVs2:
                {
                    if (entity is Tower tower)
                        tower.Damage = (int) (tower.Damage * 1.10);
                    break;
                }

                case Id.DecreaseAi1:
                {
                    if (entity is Tower tower)
                    {
                        var timer = tower.FireTimer;
                        timer.Cooldown = new TimeSpan((long) (timer.Cooldown.Ticks * 0.95));
                        if (timer.RemainingCooldown > timer.Cooldown)
                            timer.Reset();
                    }
                    break;
                }

                case Id.CdBoomerang:
                    if (entity is CdThrower cdThrower)
                        cdThrower.ShootsBoomerang = true;
                    break;

                case Id.IncreaseGsNokia:
                    if (entity is Nokia nokia)
                        nokia.Speed *= 1.40f;
                    break;

                case Id.IncreaseGsFirefox:
                    if (entity is Firefox firefox)
                        firefox.Speed *= 1.10f;
                    break;

                case Id.MoreTrojanChildren1:
                {
                    if (entity is Trojan trojan)
                        trojan.ChildCount += 5;
                    break;
                }

                case Id.EmpDuration:
                {
                    if (entity is Bluescreen bluescreen)
                        bluescreen.mEmpDurationAmplifier += 0.4f;
                    break;
                }
                
                case Id.IncreaseSettingsArea1:
                {
                    if (entity is Settings settings)
                        settings.AmplifyAbilityRange(0.5f);
                    break;
                }
                    
                case Id.IncreaseSettingsHeal1:
                    NotImplemented();
                    break;

                case Id.MoreTrojanChildren2:
                {
                    if (entity is Trojan trojan)
                        trojan.ChildCount += 10;
                    break;
                }

                case Id.EmpTwoTargets:
                {
                    if (entity is Bluescreen bluescreen)
                        bluescreen.TargetsTwoTower = true;
                    break;
                }

                case Id.IncreaseSettingsArea2:
                {
                    if (entity is Settings settings)
                        settings.AmplifyAbilityRange(0.10f);
                    break;
                }

                case Id.IncreaseSettingsHeal2:
                    NotImplemented();
                    break;

                case Id.IncreaseBitcoins:
                case Id.AdditionalFirefox1:
                case Id.AdditionalFirefox2:
                    // Nothing to do here for this upgrade.
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
