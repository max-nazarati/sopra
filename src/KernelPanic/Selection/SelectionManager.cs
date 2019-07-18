using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.Players;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Selection
{
    [DataContract]
    internal sealed class SelectionManager
    {
        internal delegate void SelectionChangedDelegate(Entity oldSelection, Entity newSelection);

        internal event SelectionChangedDelegate SelectionChanged;
        
        /// <summary>
        /// The <see cref="Lane"/> with this <see cref="Player"/>'s <see cref="Unit"/>s.
        /// </summary>
        private readonly Lane mLeftLane;

        /// <summary>
        /// The <see cref="Lane"/> with the enemy's <see cref="Unit"/>s.
        /// </summary>
        private readonly Lane mRightLane;

        private readonly Sprite mSelectionBorder;

        [DataMember]
        private Entity mSelection;

        internal SelectionManager(Lane leftLane, Lane rightLane, SpriteManager spriteManager)
        {
            mLeftLane = leftLane;
            mRightLane = rightLane;
            mSelectionBorder = spriteManager.CreateSelectionBorder();
        }

        internal Entity Selection
        {
            get => mSelection;
            private set
            {
                if (mSelection != null)
                {
                    mSelection.Selected = false;
                }

                if (value != null)
                {
                    value.Selected = true;
                }

                SelectionChanged?.Invoke(mSelection, value);
                mSelection = value;
            }
        }

        internal void Update(InputManager inputManager, bool leftOnlyBuildings)
        {
            if (Selection?.WantsRemoval == true)
            {
                Selection = null;
            }
            
            if (inputManager.IsClaimed(InputManager.MouseButton.Left))
            {
                // No click for us to handle.
                return;
            }
            
            var mouse = inputManager.TranslatedMousePosition;

            bool ProcessLane(Lane lane, bool onlyBuildings)
            {
                var mouseEntities = lane.EntityGraph.EntitiesAt(mouse);
                var maybeSelection = onlyBuildings
                        ? mouseEntities.FirstOrDefault(e => e is Building)
                        : mouseEntities.FirstOrDefault();

                if (!(maybeSelection is Entity entity))
                    return false;

                if (!inputManager.MousePressed(InputManager.MouseButton.Left))
                    return false;

                Selection = Selection == entity ? null : entity;
                return true;
            }

            if (ProcessLane(mLeftLane, leftOnlyBuildings) || ProcessLane(mRightLane, false))
                return;

            if (leftOnlyBuildings && mLeftLane.Contains(mouse))
                return;

            if (mSelection == null || !(mLeftLane.Contains(mouse) || mRightLane.Contains(mouse)))
                return;

            if (inputManager.MousePressed(InputManager.MouseButton.Left))
            {
                // The click was inside a lane but not on an entity => deselect any selected entities.
                Selection = null;
            }
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Selection == null)
                return;

            mSelectionBorder.Position = Selection.Sprite.Position;
            mSelectionBorder.Draw(spriteBatch, gameTime);
            Selection.DrawActions(spriteBatch, gameTime);
        }
    }
}
