using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning.DecisionTrees;
using System.Data;
using System.IO;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.MachineLearning.DecisionTrees.Rules;
using Accord.Math;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Filters;

namespace KernelPanic.ArtificialIntelligence
{
    class DecisionTreeClassifier
    {
        private DataTable mDataSet;
        private Codification mCodebook;
        private DecisionTree mModel;

        /// <summary>
        /// Train a decision tree model based on its training set (mDataset)
        /// </summary>
        public void TrainModel()
        {
        // split dataset into features (xTrain) and labels (yTrain)  
        double[][] xTrain = mDataSet.ToJagged<double>("Bitcoins",
            "Bug", "Virus", "Trojaner", "Nokia", "Thunderbird", "Settings",
        "Firefox", "Bluescreen", "Kabel", "Mauszeigerschütze", "CD-Werfer",
        "Antivirusprogramm", "Lüftung", "Wifi-Router", "Schockfeld");
        string[] labels = mDataSet.ToArray<String>("Aktion");
        mCodebook = new Codification("Aktion", labels);
        int[] yTrain = mCodebook.Transform("Aktion", labels);

        // train decision tree model
        C45Learning teacher = new C45Learning();
        mModel = teacher.Learn(xTrain, yTrain);

        // training statistics
        int[] predicted = mModel.Decide(xTrain);
        double error = new ZeroOneLoss(yTrain).Loss(predicted);
        Console.WriteLine("training error: " + error);
        }

        /// <summary>
        /// Read csv file and store content as DataTable, e.g.:
        /// ==============================
        /// Bitcoins,Trojaner,Aktion
        /// 3,5,CD-Werfer
        /// ==============================
        /// to
        /// ==============================
        /// Bitcoins | Trojaner | Aktion
        /// ------------------------------
        ///     3          5     CD-Werfer
        /// ==============================
        /// </summary>
        /// <param name="filename">path to file</param>
        public void ReaderCsv(string filename)
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

        /// <summary>
        /// Translate tree into set of rules, e.g.:
        ///                     /   \
        /// Feature 1      <10 /     \ >= 10
        ///                   /       \
        ///                  no       yes
        /// Will be translated to:
        /// no : (1 <  10)
        /// yes: (1 >= 10)
        /// </summary>
        /// <returns>The set of rules</returns>
        public override String ToString()
        {
            DecisionTree model = this.Model;
            DecisionSet rules = mModel.ToRules();
            String result = rules.ToString(mCodebook, "Aktion",
                System.Globalization.CultureInfo.InvariantCulture);
            return result;
        }

        public Codification Codebook { get => mCodebook; set => mCodebook = value; }
        public DecisionTree Model { get => mModel; set => mModel = value; }
        public DecisionSet Rules { get => mModel.ToRules();}
    }
}
