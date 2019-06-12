using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace KernelPanic
{
    class StorageManager
    {
        private DataContractSerializer mSerializer;

        public void SaveGame(String fileName, AGameState gameState)
        {
            Console.WriteLine("Serializing InGameState.");
            mSerializer = new DataContractSerializer(typeof(InGameState));
            var settings = new XmlWriterSettings { Indent = true };
            var writer = XmlWriter.Create(fileName, settings);
            mSerializer.WriteObject(writer, gameState);
            writer.Close();
            Console.WriteLine("Finished serializing.");
        }

        public InGameState LoadGame(String fileName, GameStateManager stateManager)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            mSerializer = new DataContractSerializer(typeof(InGameState));
            InGameState deserializedGameState = (InGameState)mSerializer.ReadObject(reader, true);
            var state = new InGameState(stateManager);
            state.mPlayerB.Bitcoins = deserializedGameState.mPlayerB.Bitcoins;
            state.mPlayerA.Bitcoins = deserializedGameState.mPlayerA.Bitcoins;
            fs.Close();

            return state;
        }
    }
}
