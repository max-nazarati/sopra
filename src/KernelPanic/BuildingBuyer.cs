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
        private readonly Player mBuyer;
        internal static PurchasableAction<Building> BoughtAction;
        private bool mSelected;
        private readonly GameStateManager mStateManager;
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
                                (new CursorShooter(mStateManager.Sprite, mStateManager.Sound), position);
                        }
                        else if (Building.GetType() == typeof(WifiRouter))
                        {
                            mBuyer.DefendingLane.BuildingSpawner.Register
                                (new WifiRouter(mStateManager.Sprite, mStateManager.Sound), position);
                        }
                        else if (Building.GetType() == typeof(CdThrower))
                        {
                            mBuyer.DefendingLane.BuildingSpawner.Register
                                (new CdThrower(mStateManager.Sprite, mStateManager.Sound), position);
                        }
                        else if (Building.GetType() == typeof(Antivirus))
                        {
                            mBuyer.DefendingLane.BuildingSpawner.Register
                                (new Antivirus(mStateManager.Sprite, mStateManager.Sound), position);
                        }
                        else if (Building.GetType() == typeof(Ventilator))
                        {
                            mBuyer.DefendingLane.BuildingSpawner.Register
                                (new Ventilator(mStateManager.Sprite, mStateManager.Sound), position);
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
