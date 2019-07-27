using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Autofac;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Units;
using KernelPanic.Options;
using KernelPanic.Table;
using KernelPanic.Tracking;
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

        internal static void RemoveGame(int slot)
        {
            try
            {
                File.Delete(DataPath(slot));
            }
            catch
            {
                // Ignore.
            }

            try
            {
                File.Delete(InfoPath(slot));
            }
            catch
            {
                // Ignore.
            }
        }

        internal static OptionsData LoadSettings() =>
            Load<OptionsData>(SettingsPath, out var data) ? data : null;

        internal static void SaveSettings(OptionsData data)
        {
            Directory.CreateDirectory(sFolder);

            using (var file = File.CreateText(SettingsPath))
                CreateSerializer().Serialize(file, data);
        }

        internal static Statistics.Data? LoadStatistics() =>
            Load<Statistics.Data>(StatisticsPath, out var data) ? (Statistics.Data?) data : null;

        internal static void SaveStatistics(Statistics.Data data)
        {
            Directory.CreateDirectory(sFolder);

            using (var file = File.CreateText(StatisticsPath))
                CreateSerializer().Serialize(file, data);
        }
        
        internal static AchievementPool.Data? LoadAchievements() =>
            Load<AchievementPool.Data>(AchievementsPath, out var data) ? (AchievementPool.Data?) data : null;

        internal static void SaveAchievements(AchievementPool.Data data)
        {
            Directory.CreateDirectory(sFolder);

            using (var file = File.CreateText(AchievementsPath))
                CreateSerializer().Serialize(file, data);
        }

        /// <summary>
        /// Loads a value of type <typeparamref name="T"/> from the file at <paramref name="path"/> catching all exceptions.
        /// The result is deserialized with using a serializer created from <see cref="CreateSerializer()"/>.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="result">Contains on completion the deserialized object or if an exception occured the default value for type <typeparamref name="T"/>.</param>
        /// <typeparam name="T">The type of object to load.</typeparam>
        /// <returns><c>true</c> if loading was successful, otherwise <c>false</c>.</returns>
        private static bool Load<T>(string path, out T result)
        {
            try
            {
                using (var file = File.OpenText(path))
                    result = (T) CreateSerializer().Deserialize(file, typeof(T));
                return true;
            }
            catch (FileNotFoundException)
            {
#if DEBUG
                Console.WriteLine($"Not loading {typeof(T)}: ›{path}‹ doesn't exist.");
#else
                Console.WriteLine($"Not loading {typeof(T)}: File doesn't exist.");
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine($"** Unable to load {typeof(T)} at ›{path}‹ **");
                Console.WriteLine(e);
                Console.WriteLine();
#else
                Console.WriteLine($"Unable to load {typeof(T)}: {e.Message}");
#endif
            }

            result = default(T);
            return false;
        }

        #endregion

        #region Private Helpers

        // The builder is only used during deserialization, therefore this message doesn't lead to a potential problem
        // of keeping a value alive for too long.
        [SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
        private static AutofacContractResolver CreateContractResolver(GameStateManager manager)
        {
            var builder = new ContainerBuilder();

            // A unit is assumed to be constructable with a SpriteManager as the only argument.
            void RegisterUnit<TUnit>() where TUnit : Unit =>
                builder.Register(c => Unit.Create<TUnit>(manager.Sprite)).As<TUnit>();

            // A building is assumed to be constructable with the SpriteManager and the SoundManager as the only arguments.
            void RegisterBuilding<TBuilding>() where TBuilding : Building =>
                builder.Register(c => Building.Create<TBuilding>(manager.Sprite)).As<TBuilding>();

            RegisterUnit<Bug>();
            RegisterUnit<Virus>();
            RegisterUnit<Nokia>();
            RegisterUnit<Trojan>();
            RegisterUnit<Thunderbird>();
            RegisterUnit<Firefox>();
            RegisterUnit<Bluescreen>();
            RegisterUnit<Settings>();
            
            RegisterBuilding<Cable>();
            RegisterBuilding<Antivirus>();
            RegisterBuilding<CdThrower>();
            RegisterBuilding<ShockField>();
            RegisterBuilding<Ventilator>();
            RegisterBuilding<WifiRouter>();
            RegisterBuilding<CursorShooter>();

            // Register further classes.
            builder.Register(c => new Board(manager.Sprite, true));
            builder.Register(c => new Lane(manager.Sprite));
            builder.Register(c => new UpgradePool(null, manager.Sprite));

            return new AutofacContractResolver(builder.Build());
        }

        private static JsonSerializer CreateSerializer(GameStateManager gameStateManager)
        {
            return new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ContractResolver = CreateContractResolver(gameStateManager)
            };
        }

        private static JsonSerializer CreateSerializer()
        {
            return new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
        }

        private static readonly string sFolder = "SaveFiles" + Path.DirectorySeparatorChar;
        private static string DataPath(int slot) => Path.Combine(sFolder, "data" + slot + ".json");
        private static string InfoPath(int slot) => Path.Combine(sFolder, "info" + slot + ".json");

        private static string SettingsPath => Path.Combine(sFolder, "settings.json");

        private static string StatisticsPath => Path.Combine(sFolder, "statistics.json");

        private static string AchievementsPath => Path.Combine(sFolder, "achievements.json");

        #endregion
    }
}
