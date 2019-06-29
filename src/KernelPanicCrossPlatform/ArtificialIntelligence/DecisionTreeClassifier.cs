using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning.DecisionTrees;
using System.Data;
using System.IO;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Filters;

namespace KernelPanic.ArtificialIntelligence
{
    class DecisionTreeClassifier
    {
        private DataTable mDataSet;

        public DecisionTree TrainModel()
        {
        // split dataset into features (xTrain) and labels (yTrain)  
        double[][] xTrain = mDataSet.ToJagged<double>("Bitcoins",
            "Bug", "Virus", "Trojaner", "Nokia", "Thunderbird", "Settings",
        "Firefox", "Bluescreen", "Kabel", "Mauszeigerschütze", "CD-Werfer",
        "Antivirusprogramm", "Lüftung", "Wifi-Router", "Schockfeld");
        string[] labels = mDataSet.ToArray<String>("Aktion");
        var codebook = new Codification("Aktion", labels);
        int[] yTrain = codebook.Transform("Aktion", labels);

        // train decision tree model
        C45Learning teacher = new C45Learning();
        DecisionTree model = teacher.Learn(xTrain, yTrain);

        // training statistics
        int[] predicted = model.Decide(xTrain);
        double error = new ZeroOneLoss(yTrain).Loss(predicted);
        Console.WriteLine("training error: " + error);

        return model;
        }

        public void ReaderCSV(string filename)
        {
            DataTable resulTable = new DataTable();
            StreamReader reader = new StreamReader(File.OpenRead(filename));
            List<string> fileContent = new List<string>();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (!String.IsNullOrWhiteSpace(line))
                {
                    fileContent.Add(line);
                }
            }

            String[] features = fileContent[0].Split(',');
            foreach (var feature in features)
            {
                resulTable.Columns.Add(feature);
            }

            for (int i = 1; i < fileContent.Count; i++)
            {
                String[] featuresContent = fileContent[i].Split(',');
                DataRow row = resulTable.NewRow();
                for (int j = 0; j < resulTable.Columns.Count; j++)
                {
                    row[resulTable.Columns[j]] = featuresContent[j];
                }
                resulTable.Rows.Add(row);
            }

            mDataSet = resulTable;
        }
    }
}
