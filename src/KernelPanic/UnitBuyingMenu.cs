﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KernelPanic.Entities;
using KernelPanic.Interface;
using KernelPanic.Purchasing;

namespace KernelPanic
{
    sealed class UnitBuyingMenu : BuyingMenuOverlay
    {
        private readonly List<Tuple<int, PurchaseButton<ImageButton, Unit>>> mChoices =
            new List<Tuple<int, PurchaseButton<ImageButton, Unit>>>();
        
        private void IncrementCount(int index)
        {
            var x = mChoices[index];
            mChoices[index] = new Tuple<int, PurchaseButton<ImageButton, Unit>>(x.Item1 + 1, x.Item2);
        }

        private void ResetCounts()
        {
            for (int i = 0; i < mChoices.Count; i++)
            {
                mChoices[i] = new Tuple<int, PurchaseButton<ImageButton, Unit>>(0, mChoices[i].Item2);
            }
        }

        internal UnitBuyingMenu(SpriteManager spriteManager, Player player)
        {
            #region Heroes
            
            var firefoxButton = CreateFirefoxPurchaseButton(spriteManager, player);
            mChoices.Add(new Tuple<int, PurchaseButton<ImageButton, Unit>>(0, firefoxButton));
            firefoxButton.Action.Purchased += (buyer, resource) =>
            {
                resource.Sprite.Position = new Vector2(50 * 30, 150 * 3);
                buyer.AttackingLane.EntityGraph.Add(resource);
                firefoxButton.Action.ResetResource(new Firefox(spriteManager));
                IncrementCount(mChoices.FindIndex(el => el.Item2.GetType() == firefoxButton.GetType()));
            };
            
            #endregion

            #region Troupes
            
            var trojanButton = CreateTrojanPurchaseButton(spriteManager, player);
            mChoices.Add(new Tuple<int, PurchaseButton<ImageButton, Unit>>(0, trojanButton));
            trojanButton.Action.Purchased += (buyer, resource) =>
            {
                resource.Sprite.Position = new Vector2(50 * 30, 150 * 3);
                buyer.AttackingLane.EntityGraph.Add(resource);
                trojanButton.Action.ResetResource(new Trojan(spriteManager));
                IncrementCount(mChoices.Count - 1);
            };

            var bugButton = CreateBugPurchaseButton(spriteManager, player);
            mChoices.Add(new Tuple<int, PurchaseButton<ImageButton, Unit>>(0, bugButton));
            bugButton.Action.Purchased += (buyer, resource) =>
            {
                resource.Sprite.Position = new Vector2(50 * 30, 150 * 3);
                buyer.AttackingLane.EntityGraph.Add(resource);
                bugButton.Action.ResetResource(new Bug(spriteManager));
                IncrementCount(mChoices.Count - 1);
            };

            #endregion
        }        
        
        internal void Update(Input.InputManager input, GameTime gameTime)
        {
            foreach (var element in mChoices)
            {
                element.Item2.Update(input, gameTime);
            }
        }
        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach(var element in mChoices)
            {
                element.Item2.Draw(spriteBatch, gameTime);
            }

        }
    }
}
