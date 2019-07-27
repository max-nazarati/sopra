using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Units;
using KernelPanic.Events;
using KernelPanic.Players;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KernelPanic.Tracking
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum Achievement
    {
        Win1,
        Win10,
        Win100,

        Lose1,
        Lose10,
        Lose100,

        IronFortress,
        Nutcracker,
        ManyWires,
        Fool,
        TowersWin,
        HighInference,

        MinionSlayer,
        BitcoinAddict,
        AptGetUpgrade,
        DirtyCoder,
        BugsFixed,
        Shockfield,
        Hacker,
        AntiVirus,
        JumpNRun,

        NumberOfAchievements
    }

    internal static class Achievements
    {
        #region Display Information

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        internal static string Title(this Achievement achievement)
        {
            switch (achievement)
            {
                case Achievement.Win1:
                    return "First Victory!";
                case Achievement.Win10:
                    return "GG EASY";
                case Achievement.Win100:
                    return "Is Dis Tetris?";

                case Achievement.Lose1:
                    return "Unlucky Loss";
                case Achievement.Lose10:
                    return "Rekt";
                case Achievement.Lose100:
                    return "Complete Humiliation";

                case Achievement.IronFortress:
                    return "Iron Fortress";
                case Achievement.TowersWin:
                    return "Tower's win the game";
                case Achievement.HighInference:
                    return "High Inference";
                case Achievement.Nutcracker:
                    return "Nutcracker";
                case Achievement.ManyWires:
                    return "Lange Leitung";
                case Achievement.Fool:
                    return "Fool!";

                case Achievement.MinionSlayer:
                    return "Minion Slayer";
                case Achievement.BitcoinAddict:
                    return "Bitcoin Addiction";
                case Achievement.AptGetUpgrade:
                    return "while true DO sudo apt-get upgrade DONE";
                case Achievement.DirtyCoder:
                    return "Dirty Coder!";
                case Achievement.BugsFixed:
                    return "Fix your Code!";
                case Achievement.Hacker:
                    return "Hacker";
                case Achievement.AntiVirus:
                    return "High Security Anti-Virus";
                case Achievement.Shockfield:
                    return "Bzzzzz";
                case Achievement.JumpNRun:
                    return "~Tower Defense~ Jump 'n' Run";    // TODO: Can we do strikethrough?

                case Achievement.NumberOfAchievements:
                    goto default;
                default:
                    throw new InvalidEnumArgumentException(nameof(achievement), (int) achievement, typeof(Achievement));
            }
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo", Justification = "Strings are in German")]
        internal static string Description(this Achievement achievement)
        {
            switch (achievement)
            {
                case Achievement.Win1:
                    return "Du hast das Spiel das erste Mal gewonnen.";
                case Achievement.Win10:
                    return "Du hast das Spiel das zehnte Mal gewonnen.";
                case Achievement.Win100:
                    return "Du hast das Spiel das hundertste Mal gewonnen.";

                case Achievement.Lose1:
                    return "Du hast das Spiel das erste Mal verloren.";
                case Achievement.Lose10:
                    return "Du hast das Spiel das zehnte Mal verloren.";
                case Achievement.Lose100:
                    return "Du hast das Spiel das hundertste Mal verloren.";

                case Achievement.IronFortress:
                    return "Gewinne ein Spiel mit 100% verbleibender Ladung.";
                case Achievement.TowersWin:
                    return "Gewinne ein Spiel, bei dem du 500 BTC in Verteidigung investiert hast.";
                case Achievement.HighInference:
                    return "Gewinne ein Spiel nur mit Wifi-Routern als Verteidigung.";
                case Achievement.Nutcracker:
                    return "Besiege eine Nokia-Einheit.";
                case Achievement.ManyWires:
                    return "Baue 500 mal Kabel.";
                case Achievement.Fool:
                    return "Versuche einen leeren Speicherstand zu laden.";

                case Achievement.MinionSlayer:
                    return "Besiege in einem Spiel 200 Einheiten.";
                case Achievement.BitcoinAddict:
                    return "Erreiche in einem Spiel 300 BTC.";
                case Achievement.AptGetUpgrade:
                    return "Investiere in einem Spiel zwanzig EP in Upgrades.";
                case Achievement.DirtyCoder:
                    return "Spawne in einem Spiel 100 Bugs.";
                case Achievement.BugsFixed:
                    return "Besiege in einem Spiel 100 Bugs.";
                case Achievement.Hacker:
                    return "Spawne in einem Spiel 100 Virus- oder Trojaner-Einheiten.";
                case Achievement.AntiVirus:
                    return "Besiege in einem Spiel 100 Virus- oder Trojaner-Einheiten.";
                case Achievement.Shockfield:
                    return "Besiege in einem Spiel 50 Einheiten mit Schockfeldern.";
                case Achievement.JumpNRun:
                    return "Springe in einem Spiel mit der Firefox-Einheit über 10 Gebäude.";

                case Achievement.NumberOfAchievements:
                    goto default;
                default:
                    throw new InvalidEnumArgumentException(nameof(achievement), (int) achievement, typeof(Achievement));
            }
        }

        #endregion

        #region Connecting

        [JsonConverter(typeof(StringEnumConverter))]
        internal enum Status
        {
            Locked,
            Unlocked,
            Failed
        }

        internal static void ConnectComponents(this Achievement achievement, ProgressConnector progressConnector)
        {
            switch (achievement)
            {
                case Achievement.Win1:
                    progressConnector.ConnectCounter(Event.Id.GameWon);
                    break;
                case Achievement.Win10:
                    progressConnector.ConnectCounter(Event.Id.GameWon, target: 10);
                    break;
                case Achievement.Win100:
                    progressConnector.ConnectCounter(Event.Id.GameWon, target: 100);
                    break;

                case Achievement.Lose1:
                    progressConnector.ConnectCounter(Event.Id.GameLost);
                    break;
                case Achievement.Lose10:
                    progressConnector.ConnectCounter(Event.Id.GameLost, target: 10);
                    break;
                case Achievement.Lose100:
                    progressConnector.ConnectCounter(Event.Id.GameLost, target: 100);
                    break;

                case Achievement.IronFortress:
                    progressConnector.ConnectCounter(Event.Id.GameWon, condition: Is100Percent);
                    break;
                case Achievement.TowersWin:
                    progressConnector.ConnectCounter(Event.Id.GameWon);
                    progressConnector.ConnectCounter(Event.Id.BuildingPlaced, Event.Key.Price, 500, e => e.IsActivePlayer(Event.Key.Buyer));
                    break;
                case Achievement.HighInference:
                    // You must win the game.
                    progressConnector.ConnectCounter(Event.Id.GameWon);
                    // You have to buy at least one building.
                    progressConnector.ConnectCounter(Event.Id.BuildingPlaced, condition: e => e.IsActivePlayer(Event.Key.Buyer));
                    // You shoudn't buy any building except wifi routers.
                    progressConnector.ConnectCounter(Event.Id.BuildingPlaced, condition: PlacedNonWifi, success: false);
                    break;
                case Achievement.Nutcracker:
                    progressConnector.ConnectCounter(Event.Id.KilledUnit,
                        condition: PlayerAndUnitMatches(Event.Key.Defender, typeof(Nokia)));
                    break;
                case Achievement.ManyWires:
                    progressConnector.ConnectCounter(Event.Id.BuildingPlaced, target: 500,
                        condition: PlayerAndBuildingMatches(Event.Key.Buyer, typeof(Cable)));
                    break;
                case Achievement.Fool:
                    progressConnector.ConnectCounter(Event.Id.LoadEmptySlot);
                    break;

                case Achievement.MinionSlayer:
                    progressConnector.ConnectCounter(Event.Id.KilledUnit, target: 200, condition: e => e.IsActivePlayer(Event.Key.Defender));
                    break;
                case Achievement.BitcoinAddict:
                    progressConnector.ConnectComparison(Event.Id.BitcoinChanged, Event.Key.Price, 300, @event => @event.IsActivePlayer(Event.Key.Buyer));
                    break;
                case Achievement.AptGetUpgrade:
                    progressConnector.ConnectCounter(Event.Id.UpgradeBought, Event.Key.Price, 20, @event => @event.IsActivePlayer(Event.Key.Buyer));
                    break;
                case Achievement.DirtyCoder:
                    progressConnector.ConnectCounter(Event.Id.SpawnedUnit, target: 100,
                        condition: PlayerAndUnitMatches(Event.Key.Attacker, typeof(Bug)));
                    break;
                case Achievement.BugsFixed:
                    progressConnector.ConnectCounter(Event.Id.KilledUnit, target: 100,
                        condition: PlayerAndUnitMatches(Event.Key.Defender, typeof(Bug)));
                    break;
                case Achievement.Hacker:
                    progressConnector.ConnectCounter(Event.Id.SpawnedUnit, target: 100,
                        condition: PlayerAndUnitMatches(Event.Key.Attacker, typeof(Virus), typeof(Trojan)));
                    break;
                case Achievement.AntiVirus:
                    progressConnector.ConnectCounter(Event.Id.KilledUnit, target: 100,
                        condition: PlayerAndUnitMatches(Event.Key.Defender, typeof(Virus), typeof(Trojan)));
                    break;
                case Achievement.Shockfield:
                    progressConnector.ConnectCounter(Event.Id.KilledUnit, target: 50,
                        condition: PlayerAndBuildingMatches(Event.Key.Defender, typeof(ShockField)));
                    break;
                case Achievement.JumpNRun:
                    progressConnector.ConnectCounter(Event.Id.FirefoxJump, target: 10,
                        condition: e => e.IsActivePlayer(Event.Key.Attacker));
                    break;

                case Achievement.NumberOfAchievements:
                    goto default;
                default:
                    throw new InvalidEnumArgumentException(nameof(achievement), (int) achievement, typeof(Achievement));
            }
        }

        private static bool Is100Percent(Event @event)
        {
            return @event.Get<Player>(Event.Key.Winner).Base.Power == 100;
        }

        private static bool PlacedNonWifi(Event @event)
        {
            var building = @event.Get<Building>(Event.Key.Building);
            return @event.IsActivePlayer(Event.Key.Buyer) && !(building is WifiRouter);
        }

        private static Func<Event, bool> PlayerAndBuildingMatches(Event.Key playerKey, params Type[] types) =>
            e => e.IsActivePlayer(playerKey) && types.Contains(e.Get<Building>(Event.Key.Building).GetType());

        private static Func<Event, bool> PlayerAndUnitMatches(Event.Key playerKey, params Type[] types) =>
            e => e.IsActivePlayer(playerKey) && types.Contains(e.Get<Unit>(Event.Key.Unit).GetType());

        #endregion

        #region Achievement Sets

        internal const int Count = (int) Achievement.NumberOfAchievements;

        internal static Achievement[] GloballyTracked =>
            new[]
            {
                Achievement.Win1,
                Achievement.Win10,
                Achievement.Win100,

                Achievement.Lose1,
                Achievement.Lose10,
                Achievement.Lose100,

                Achievement.IronFortress,
                Achievement.Nutcracker,
                Achievement.ManyWires,
                Achievement.Fool
            };

        internal static Achievement[] PerGame =>
            new[]
            {
                Achievement.MinionSlayer,
                Achievement.BitcoinAddict,
                Achievement.AptGetUpgrade,
                Achievement.DirtyCoder,
                Achievement.BugsFixed,
                Achievement.Shockfield,
                Achievement.Hacker,
                Achievement.AntiVirus,
                Achievement.JumpNRun,
                Achievement.HighInference,
                Achievement.TowersWin
            };

        #endregion
    }
}