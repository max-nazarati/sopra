using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace KernelPanic
{
    class StorageManager
    {
        private DataContractSerializer mSerializer;
        internal static readonly string folder = "SaveFiles\\";

        internal static string[] files = new string[5];

        public void SaveGame(String fileName, AGameState gameState)
        {
            Directory.CreateDirectory(folder);
            fileName = folder + fileName;
            mSerializer = new DataContractSerializer(typeof(InGameState));
            var settings = new XmlWriterSettings { Indent = true };
            var writer = XmlWriter.Create(fileName, settings);
            mSerializer.WriteObject(writer, gameState);
            writer.Close();
        }

        public InGameState LoadGame(String fileName, GameStateManager stateManager)
        {
            fileName = folder + fileName;
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
