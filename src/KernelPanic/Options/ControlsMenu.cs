using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic.Options
{
    internal sealed class ControlsMenu : MenuState
    {
        internal static IEnumerable<Keys> DefaultTowerKeys => RightColumn.Select(value => value.Key);

        internal ControlsMenu(GameStateManager gameStateManager) : base(gameStateManager)
        {
            Components = InterfaceComponents(gameStateManager);
        }

        private TextButton mSelected;
        private Keys mBaseKey;

        private IReadOnlyCollection<InterfaceComponent> InterfaceComponents(GameStateManager gameStateManager)
        {
            var sprites = gameStateManager.Sprite;
            var keyMap = gameStateManager.Settings.KeyMap;

            var resetButton = CreateButton(sprites, "Zurücksetzen", 700);
            resetButton.Clicked += delegate
            {
                keyMap.Clear();
                gameStateManager.Switch(new ControlsMenu(gameStateManager));
            };

            var backButton = CreateButton(sprites, "Zurück", 800);
            backButton.Clicked += gameStateManager.PopOnClick;
            
            var components = new List<InterfaceComponent>(3 + 2 * 6 + 2 * 6)
            {
                CreateBackgroundWithoutText(sprites),
                resetButton,
                backButton
            };

            components.AddRange(KeySelectionComponents(true, keyMap, sprites));
            components.AddRange(KeySelectionComponents(false, keyMap, sprites));

            return components;
        }

        private IEnumerable<InterfaceComponent> KeySelectionComponents(bool left, KeyMap keyMap, SpriteManager sprites)
        {
            var enumerable = left ? LeftColumn : RightColumn;
            var xAlign = sprites.ScreenSize.X / 4 * (left ? 1 : 3);

            var y = 200;
            foreach (var (description, key) in enumerable)
            {
                var realKey = keyMap[key];
                
                var descriptionSprite = sprites.CreateText(description);
                descriptionSprite.X = xAlign - 10;
                descriptionSprite.Y = y;
                descriptionSprite.SetOrigin(RelativePosition.CenterRight);

                var keyButton = new TextButton(sprites) { Title = realKey.ToString(), Sprite = { X = xAlign + 10, Y = y }};
                keyButton.Sprite.SetOrigin(RelativePosition.CenterLeft);
                keyButton.Clicked += delegate { SetSelection(keyButton, key); };

                yield return new StaticComponent(descriptionSprite);
                yield return keyButton;

                y += (int) keyButton.Sprite.Height + 20;
            }
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo", Justification = "Strings are in German")]
        private static IEnumerable<(string Description, Keys Key)> LeftColumn =>
            new[]
            {
                ("Kamera nach oben", Keys.W),
                ("Kamera nach unten", Keys.S),
                ("Kamera nach links", Keys.A),
                ("Kamera nach rechts", Keys.D),

                ("Fähigkeit ausführen", Keys.Q),
                ("Fähigkeit abbrechen", Keys.E)
            };

        [SuppressMessage("ReSharper", "StringLiteralTypo", Justification = "Strings are in German")]
        private static IEnumerable<(string Description, Keys Key)> RightColumn =>
            new[]
            {
                ("Kabel auswählen", Keys.D1),
                ("Lüfter auswählen", Keys.D2),
                ("Mauszeiger auswählen", Keys.D3),
                ("Schockfeld auswählen", Keys.D4),
                ("Wifi-Router auswählen", Keys.D5),
                ("Antivirus auswählen", Keys.D6)
            };

        private void Deselect()
        {
            SetSelection(null, Keys.None);
        }

        private void SetSelection(TextButton selection, Keys baseKey)
        {
            if (mSelected != null)
            {
                mSelected.ViewPressed = false;
                mSelected.Title = GameStateManager.Settings.KeyMap[mBaseKey].ToString();

                if (mSelected == selection)
                {
                    mSelected = null;
                    mBaseKey = Keys.None;
                    return;
                }
            }

            mSelected = selection;
            mBaseKey = baseKey;
            if (mSelected == null)
                return;

            mSelected.ViewPressed = true;
            mSelected.Title = "_";
        }

        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            if (mSelected != null && inputManager.AnyKeyPressed)
                UpdateKeyMap(inputManager);

            base.Update(inputManager, gameTime);
        }

        private void UpdateKeyMap(InputManager inputManager)
        {
            if (inputManager.KeyPressed(Keys.Escape))
            {
                Deselect();
                return;
            }

            var keyMap = GameStateManager.Settings.KeyMap;
            var unmapped = LeftColumn.Concat(RightColumn).Select(value => value.Key).Where(keyMap.KeyUnmapped).ToList();

            bool KeyUsable(Keys key)
            {
                if (keyMap[mBaseKey] == key)
                    return true;

                return keyMap.KeyUsage(key) == Keys.None && !unmapped.Contains(key);
            }

            var pressed = inputManager.PressedKey(KeyUsable);
            if (pressed == Keys.None)
                return;

            keyMap[mBaseKey] = pressed;
            Deselect();
        }
    }
}