using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using KernelPanic.Entities;
using KernelPanic.Table;
using KernelPanic.Upgrades;
using Newtonsoft.Json;

namespace KernelPanic.Serialization
{
    internal static class StorageManager
    {
        #region Save & Load

        internal static IEnumerable<int> Slots => Enumerable.Range(0, 5);

        internal static void SaveGame(InGameState gameState)
        {
            Directory.CreateDirectory(sFolder);

            if (!Slots.Contains(gameState.SaveSlot))
                throw new InvalidOperationException("InGameState has invalid save-slot " + gameState.SaveSlot);

            var serializer = CreateSerializer(gameState.GameStateManager);

            using (var file = File.CreateText(DataPath(gameState.SaveSlot)))
                serializer.Serialize(file, gameState.Data);
            using (var file = File.CreateText(InfoPath(gameState.SaveSlot)))
                serializer.Serialize(file, new Storage.Info {Timestamp = DateTime.Now});
        }

        internal static Storage LoadGame(int slot, GameStateManager gameStateManager)
        {
            Directory.CreateDirectory(sFolder);

            if (!Slots.Contains(slot))
                throw new ArgumentOutOfRangeException(nameof(slot), slot, "invalid slot value");

            using (var file = File.OpenText(DataPath(slot)))
                return (Storage) CreateSerializer(gameStateManager).Deserialize(file, typeof(Storage));
        }

        internal static Storage.Info? LoadInfo(int slot, GameStateManager gameStateManager)
        {
            if (!Slots.Contains(slot))
                throw new ArgumentOutOfRangeException(nameof(slot), slot, "invalid slot value");

            if (!File.Exists(DataPath(slot)) || !File.Exists(InfoPath(slot)))
                return null;

            using (var file = File.OpenText(InfoPath(slot)))
                return (Storage.Info) CreateSerializer(gameStateManager).Deserialize(file, typeof(Storage.Info));
        }

        #endregion

        #region Private Helpers

        // The builder is only used during deserialization, therefore this message doesn't lead to a potential problem
        // of keeping a value alive for too long.
        [SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
        private static AutofacContractResolver CreateContractResolver(GameStateManager manager)
        {
            const BindingFlags bindingFlags =
                BindingFlags.Instance | BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            var builder = new ContainerBuilder();

            // A unit is assumed to be constructable with a SpriteManager as the only argument.
            void RegisterUnit<TUnit>() where TUnit : Unit =>
                builder.Register(c => Unit.Create<TUnit>(manager.Sprite)).As<TUnit>();

            // A building is assumed to be constructable with the SpriteManager and the SoundManager as the only arguments.
            void RegisterBuilding<TBuilding>() where TBuilding : Building =>
                builder.Register(c => Building.Create<TBuilding>(manager.Sprite, manager.Sound)).As<TBuilding>();

            RegisterUnit<Bug>();
            RegisterUnit<Virus>();
            RegisterUnit<Nokia>();
            RegisterUnit<Trojan>();
            RegisterUnit<Thunderbird>();
            RegisterUnit<Firefox>();
            RegisterUnit<Bluescreen>();
            RegisterUnit<Settings>();
            
            RegisterBuilding<Cable>();
            RegisterBuilding<CdThrower>();
            RegisterBuilding<ShockField>();
            RegisterBuilding<Ventilator>();
            RegisterBuilding<WifiRouter>();
            RegisterBuilding<CursorShooter>();
            RegisterBuilding<StrategicTower>();     // TODO: Remove this when there are no more direct instances.

            // Register further classes.
            builder.Register(c => new Board(manager.Sprite, manager.Sound, true));
            builder.Register(c => new Lane(manager.Sprite, manager.Sound));
            builder.Register(c => new UpgradePool(null, manager.Sprite));

            return new AutofacContractResolver(builder.Build());
        }

        private static JsonSerializer CreateSerializer(GameStateManager gameStateManager)
        {
            return new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ContractResolver = CreateContractResolver(gameStateManager)
            };
        }

        private static readonly string sFolder = "SaveFiles" + Path.DirectorySeparatorChar;
        private static string DataPath(int slot) => Path.Combine(sFolder, "data" + slot + ".json");
        private static string InfoPath(int slot) => Path.Combine(sFolder, "info" + slot + ".json");

        #endregion

    }
}
