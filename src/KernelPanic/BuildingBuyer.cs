using System;
using Microsoft.Xna.Framework;
using KernelPanic.Table;
using KernelPanic.Input;
using KernelPanic.Entities;
using KernelPanic.Purchasing;

namespace KernelPanic
{
    internal class BuildingBuyer
    {
        internal static Building Building { get; set; }
        internal Player mBuyer;
        internal static PurchasableAction<Building> mAction;
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
                if (!mBuyer.DefendingLane.EntityGraph.HasEntityAt(position) && input.MouseDown(InputManager.MouseButton.Left))
                {
                    mAction.TryPurchase(mBuyer);
                    // TODO: Replace Tower.Create(...) with Building.Clone()
                    mBuyer.DefendingLane.BuildingSpawner.Register(Tower.Create(Vector2.Zero, 64, mStateManager.Sprite, mStateManager.Sound), position);
                }
                else
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
                    }
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
