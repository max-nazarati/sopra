using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace KernelPanic
{
    class StorageManager
    {
        private DataContractSerializer mSerializer;
        internal const string Folder = "SaveFiles\\";

        internal static string[] Files { get; } = new string[5];

        public void SaveGame(String fileName, AGameState gameState)
        {
            Directory.CreateDirectory(Folder);
            fileName = Folder + fileName;
            mSerializer = new DataContractSerializer(typeof(InGameState));
            var settings = new XmlWriterSettings { Indent = true };
            var writer = XmlWriter.Create(fileName, settings);
            mSerializer.WriteObject(writer, gameState);
            writer.Close();
        }

        public InGameState LoadGame(String fileName, GameStateManager stateManager)
        {
            fileName = Folder + fileName;
            FileStream fs = new FileStream(fileName, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            mSerializer = new DataContractSerializer(typeof(InGameState));
            InGameState deserializedGameState = (InGameState)mSerializer.ReadObject(reader, true);
            var state = new InGameState(stateManager);
            // TODO: Revise the deserialization process.
            // state.mPlayerB.Bitcoins = deserializedGameState.mPlayerB.Bitcoins;
            // state.mPlayerA.Bitcoins = deserializedGameState.mPlayerA.Bitcoins;
            fs.Close();

            return state;
        }
    }
}
