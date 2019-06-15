using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Entities;

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
        private readonly Lane mOwnedLane;

        /// <summary>
        /// The <see cref="Lane"/> with the enemy's <see cref="Unit"/>s.
        /// </summary>
        private readonly Lane mEnemyLane;

        [DataMember]
        private Entity mSelection;

        internal SelectionManager(Lane ownedLane, Lane enemyLane)
        {
            mOwnedLane = ownedLane;
            mEnemyLane = enemyLane;
        }

        internal Entity Selection
        {
            get => mSelection;
            private set
            {
                if (mSelection != null)
                    mSelection.Selected = false;
                if (value != null)
                    value.Selected = true;
                    
                SelectionChanged?.Invoke(mSelection, value);
                mSelection = value;
            }
        }

        internal void Update(InputManager inputManager)
        {
            if (inputManager.IsClaimed(InputManager.MouseButton.Left))
            {
                // No click for us to handle.
                return;
            }
            
            var mouse = inputManager.TranslatedMousePosition;

            bool ProcessLane(Lane lane)
            {
                if (!(lane.EntityGraph.EntitiesAt(mouse).FirstOrDefault() is Entity entity))
                    return false;

                if (!inputManager.MousePressed(InputManager.MouseButton.Left))
                    return false;

                Selection = entity;
                return true;
            }
            
            if (ProcessLane(mOwnedLane) || ProcessLane(mEnemyLane))
                return;

            if (!mOwnedLane.Contains(mouse) && !mEnemyLane.Contains(mouse))
                return;

            if (inputManager.MousePressed(InputManager.MouseButton.Left))
            {
                // The click was inside a lane but not on an entity => deselect any selected entities.
                Selection = null;
            }
        }
    }
}
