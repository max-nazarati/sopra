using System;
using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Units;
using KernelPanic.Events;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using KernelPanic.Table;
using KernelPanic.Upgrades;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace KernelPanic.ArtificialIntelligence
{
    enum Feature : int
    {
        Bitcoins = 0,
        Bug = 1,
        Virus = 2,
        Trojaner = 3,
        Nokia = 4,
        Thunderbird = 5,
        Settings = 6,
        Firefox = 7,
        //Bluescreen = 8,
        Cable = 9,
        CursorShooter = 10,
        CdThrower = 11,
        Antivirus = 12,
        Ventilator = 13,
        WifiRouter = 14,
        ShockField = 15
    }

    internal sealed class ArtificialPlayer : Player
    {
        private AttackPlanner mAttackPlanner;
        private DefencePlanner mDefencePlanner;
        private UpgradePlanner mUpgradePlanner;
        private int[] mDefenceData;
        private int[] mAttackData;
        private int mAttackMoney;
        private int mDefenceMoney;
        private bool mNeedDefensiveUnits;

        // private int[] mOwnTroupeAmount;

        [JsonConstructor]
        internal ArtificialPlayer(Lane defendingLane, Lane attackingLane, int bitcoins) : base(defendingLane, attackingLane, bitcoins)
        {
            mDefenceData = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            mAttackData = new [] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            mNeedDefensiveUnits = false;
            mDefenceData[(int)Feature.Bitcoins] = bitcoins;
            var eventCenter = EventCenter.Default;

            eventCenter.Subscribe(Event.Id.BoughtUnit,
                e =>
                {
                    UpdateDefenceData(Event.Id.BoughtUnit, e);
                    mNeedDefensiveUnits = true;
                },
                e => e.IsActivePlayer(Event.Key.Buyer));
            eventCenter.Subscribe(Event.Id.BuildingPlaced,
                e => UpdateDefenceData(Event.Id.BuildingPlaced, e),
                e => !e.IsActivePlayer(Event.Key.Buyer));
            eventCenter.Subscribe(Event.Id.BoughtUnit,
                e => UpdateAttackData(Event.Id.BoughtUnit, e),
                e => !e.IsActivePlayer(Event.Key.Buyer));
            eventCenter.Subscribe(Event.Id.BuildingPlaced,
                e => UpdateAttackData(Event.Id.BuildingPlaced, e),
                e => e.IsActivePlayer(Event.Key.Buyer));
            eventCenter.Subscribe(Event.Id.DamagedBase,
                e => mNeedDefensiveUnits = true,
                e => !e.IsActivePlayer(Event.Key.Defender));
            // mOwnTroupeAmount = new int[5]; // amount of different troupes in the game       
            SetData();
        }

        public override T Select<T>(T ifActive, T ifPassive)
        {
            return ifPassive;
        }

        internal void InitializePlanners(
            Dictionary<Type, PurchasableAction<Unit>> unitBuyingActions,
            Func<Upgrade.Id, SinglePurchasableAction<Upgrade>> upgradeLookup,
            SpriteManager spriteManager,
            SoundManager soundManager)
        {
            mAttackPlanner = new AttackPlanner(this, unitBuyingActions);
            mUpgradePlanner = new UpgradePlanner(this, upgradeLookup);
            mDefencePlanner = new DefencePlanner(this, spriteManager, soundManager);
        }

        #region Data
        
        private void SetData()
        {
            SetMoney();
            SetAttackData();
            // SetDefenceData();
        }

        private void SetMoney()
        {
            // for now we are only splitting the money equal between attack and defence
            mAttackMoney = (int)(Bitcoins * 0.5);
            mDefenceMoney = (int)(Bitcoins * 0.5);
        }

        private void SetAttackData()
        {
            // Data Format is:
            // Bitcoin (own), Bug (own), Trojaner, Nokia, Thunderbird, Settings, Firefox, Bluescreen, Cable(enemy), Mauszeigersch., CD-Werfer, Antivirus, Lüftung, WiFi, Shockfield

            var data = new[] {mAttackMoney, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            mAttackData = data;
        }
        /*
        private void SetDefenceData()
        {
            // Data Format is:
            // Bitcoin (own), Bug (enemy), Trojaner, Nokia, Thunderbird, Settings, Firefox, Bluescreen, Cable(own), Mauszeigersch., CD-Werfer, Antivirus, Lüftung, WiFi, Shockfield
            var data = new[] {mDefenceMoney, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            mDefenceData = data;

        } */

        private void UpdateDefenceData(Event.Id id, Event handler)
        {
            switch (id)
            {
                case Event.Id.BoughtUnit:    
                    var unitType = handler.Get<Unit>(Event.Key.Unit);
                    switch (unitType)
                    {
                        case Bug _:
                            mDefenceData[(int)Feature.Bug]++;
                            break;
                        case Virus _:
                            mDefenceData[(int)Feature.Virus]++;
                            break;
                        case Trojan _:
                            mDefenceData[(int)Feature.Trojaner]++;
                            break;
                        case Nokia _:
                            mDefenceData[(int)Feature.Nokia]++;
                            break;
                        case Thunderbird _:
                            mDefenceData[(int)Feature.Thunderbird]++;
                            break;
                        case Settings _:
                            mDefenceData[(int)Feature.Settings]++;
                            break;
                        case Firefox _:
                            mDefenceData[(int)Feature.Firefox]++;
                            break;
                    }

                    break;
                case Event.Id.BuildingPlaced:
                    var buildingType = handler.Get<Building>(Event.Key.Building);
                    switch (buildingType)
                    {
                        //case Bluescreen _:
                        //    break;
                        case Cable _:
                            mDefenceData[(int) Feature.Cable]++;
                            break;
                        case CursorShooter _:
                            mDefenceData[(int) Feature.CursorShooter]++;
                            break;
                        case CdThrower _:
                            mDefenceData[(int) Feature.CdThrower]++;
                            break;
                        case Antivirus _:
                            mDefenceData[(int) Feature.Antivirus]++;
                            break;
                        case ShockField _:
                            mDefenceData[(int) Feature.ShockField]++;
                            break;
                        case Ventilator _:
                            mDefenceData[(int) Feature.Ventilator]++;
                            break;
                        case WifiRouter _:
                            mDefenceData[(int) Feature.WifiRouter]++;
                            break;
                    }

                    break;
            }
        }

        private void UpdateAttackData(Event.Id id, Event handler)
        {
            switch (id)
            {
                case Event.Id.BoughtUnit:
                    var unitType = handler.Get<Unit>(Event.Key.Unit);
                    switch (unitType)
                    {
                        case Bug _:
                            mAttackData[(int) Feature.Bug]++;
                            break;
                        case Virus _:
                            mAttackData[(int) Feature.Virus]++;
                            break;
                        case Trojan _:
                            mAttackData[(int) Feature.Trojaner]++;
                            break;
                        case Nokia _:
                            mAttackData[(int) Feature.Nokia]++;
                            break;
                        case Thunderbird _:
                            mAttackData[(int) Feature.Thunderbird]++;
                            break;
                        case Settings _:
                            mAttackData[(int) Feature.Settings]++;
                            break;
                        case Firefox _:
                            mAttackData[(int) Feature.Firefox]++;
                            break;
                    }

                    break;
                case Event.Id.BuildingPlaced:
                    var buildingType = handler.Get<Building>(Event.Key.Building);
                    switch (buildingType)
                    {
                        //case Bluescreen _:
                        //    break;
                        case Cable _:
                            mAttackData[(int) Feature.Cable]++;
                            break;
                        case CursorShooter _:
                            mAttackData[(int) Feature.CursorShooter]++;
                            break;
                        case CdThrower _:
                            mAttackData[(int) Feature.CdThrower]++;
                            break;
                        case Antivirus _:
                            mAttackData[(int) Feature.Antivirus]++;
                            break;
                        case ShockField _:
                            mAttackData[(int) Feature.ShockField]++;
                            break;
                        case Ventilator _:
                            mAttackData[(int) Feature.Ventilator]++;
                            break;
                        case WifiRouter _:
                            mAttackData[(int) Feature.WifiRouter]++;
                            break;
                    }

                    break;
            }
        }

        #endregion

        public void Update(GameTime gameTime)
        {
            if (mAttackPlanner == null || mDefencePlanner == null || mUpgradePlanner == null || EventCenter.Default == null)
            {
                // I caught an error when starting a new game, I hope this fixes it
                return;
                // does not fix it
            }
            
            SetData();
            
            mAttackPlanner.Update(mAttackData, gameTime);
            if (mNeedDefensiveUnits) mDefencePlanner.Update(mDefenceData, gameTime);
            mUpgradePlanner.Update();
            mNeedDefensiveUnits = false;
        }
    }
}