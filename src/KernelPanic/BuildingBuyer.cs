using System;
using Microsoft.Xna.Framework;
using KernelPanic.Table;
using KernelPanic.Input;
using KernelPanic.Entities;
using KernelPanic.Players;
using KernelPanic.Purchasing;

namespace KernelPanic
{
    internal class BuildingBuyer
    {
        internal static Building Building { get; set; }
        private Player mBuyer;
        internal static PurchasableAction<Building> BoughtAction;
        private bool mSelected;
        private GameStateManager mStateManager;
        internal BuildingBuyer(Player player, GameStateManager gameStateManager)
        {
            mBuyer = player;
            mStateManager = gameStateManager;
        }
        internal void Update(InputManager input)
        {
            var position = input.TranslatedMousePosition;
            if (!mSelected && Building != null && mBuyer.DefendingLane.Contains(position))
            {
                mSelected = true;
            }
            else if (Building != null && mBuyer.DefendingLane.Contains(position))
            {
                if (input.MousePressed(InputManager.MouseButton.Left))
                {
                    var entities = mBuyer.DefendingLane.EntityGraph.EntitiesAt(position);
                    foreach (var e in entities)
                    {
                        if (e.GetType() != Building.GetType())
                        {
                            mSelected = false;
                            Building = null;
                            return;
                        }
                        else if (e.GetType() == Building.GetType())
                        {
                            mSelected = false;
                            Building = null;
                            return;
                        }
                    }
                    if (BoughtAction.TryPurchase(mBuyer))
                    {
                        // TODO: Replace Tower.Create(...) with Building.Clone()
                        if (Building.GetType() == typeof(CursorShooter))
                        {
                            mBuyer.DefendingLane.BuildingSpawner.Register
                                (Tower.CreateTower(Vector2.Zero, 64, mStateManager.Sprite, mStateManager.Sound,
                                StrategicTower.Towers.CursorShooter), position);
                        }
                        else if (Building.GetType() == typeof(WifiRouter))
                        {
                            mBuyer.DefendingLane.BuildingSpawner.Register
                                (Tower.CreateTower(Vector2.Zero, 64, mStateManager.Sprite, mStateManager.Sound,
                                StrategicTower.Towers.WifiRouter), position);
                        }
                        else if (Building.GetType() == typeof(CdThrower))
                        {
                            mBuyer.DefendingLane.BuildingSpawner.Register
                                (Tower.CreateTower(Vector2.Zero, 64, mStateManager.Sprite, mStateManager.Sound,
                                StrategicTower.Towers.CdThrower), position);
                        }
                    }
                }
                else
                {
                    
                }
            }
            else
            {
                mSelected = false;
                Building = null;
            }

        }
    }
}
