using System;
using System.Linq;
using KernelPanic.Input;
using KernelPanic.Entities;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class BuildingBuyer
    {
        private readonly Player mPlayer;
        private readonly PurchasableAction<Building> mBuyAction;
        private readonly SoundManager mSoundManager;

        private Building mBuilding;

        internal Building Building
        {
            set
            {
                mBuilding = value;
                mBuyAction.ResetResource(value);

                if (mBuilding != null)
                {
                    TintEntity(mBuilding, Color.Gray);
                }
            }
        }

        private Point? mPosition;

        internal BuildingBuyer(Player player, SoundManager soundManager)
        {
            mPlayer = player;
            mBuyAction = new PurchasableAction<Building>();
            mBuyAction.Purchased += PurchasedBuilding;
            mSoundManager = soundManager;
        }

        internal void Update(InputManager input)
        {
            if (!mBuyAction.HasResource)
                return;

            UpdatePosition(input);

            if (!input.MousePressed(InputManager.MouseButton.Left))
                return;

            if (mPosition == null || !mBuyAction.TryPurchase(mPlayer))
            {
                // TODO: Play failure sound.
            }
        }

        private void UpdatePosition(InputManager inputManager)
        {
            var lane = mPlayer.DefendingLane;
            var mouse = inputManager.TranslatedMousePosition;
            if (!(lane.Grid.TileFromWorldPoint(mouse) is TileIndex tile))
            {
                mPosition = null;
                return;
            }

            var tilePoint = lane.Grid.GetTile(tile).Position;
            if (lane.EntityGraph.EntitiesAt(tilePoint).Any(entity => entity is Building))
            {
                mPosition = null;
                return;
            }

            mPosition = tile.ToPoint();
            mBuilding.Sprite.Position = tilePoint;
        }
        
        private void PurchasedBuilding(Player buyer, Building building)
        {
            if (!(mPosition is Point point))
                throw new InvalidOperationException("Can't purchase the building if the position is null.");

            var clone = building.Clone();
            TintEntity(clone, Color.White);
            buyer.DefendingLane.BuildingSpawner.Register(clone, point);
            mSoundManager.PlaySound(SoundManager.Sound.TowerPlacement);
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (mPosition.HasValue)
                mBuilding?.Draw(spriteBatch, gameTime);
        }

        private static void TintEntity(Entity building, Color color)
        {
            ((ImageSprite) building.Sprite).TintColor = color;
        }
    }
}
