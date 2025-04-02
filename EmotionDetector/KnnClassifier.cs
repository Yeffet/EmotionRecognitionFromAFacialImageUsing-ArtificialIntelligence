using System;
using System.Collections.Generic;
using System.Linq;  // לשימוש בפעולות כמו OrderBy, GroupBy וכו'

namespace EmotionDetector
{
    public class KnnClassifier
    {
            // חישוב המרחק האוקלידי בין שני מערכי תכונות
            // <param name="features1">מערך תכונות של תמונה אחת (double[])</param>
            // <param name="features2">מערך תכונות של תמונה שניה (double[])</param>
            private static double CalculateDistance(double[] features1, double[] features2)
        {
            double sum = 0;
            for (int i = 0; i < features1.Length; i++)
            {
                double diff = features1[i] - features2[i];
                sum += diff * diff;
            }
            return Math.Sqrt(sum);
        }

        // סיווג תמונה חדשה על פי KNN
        public static string Classify(
            List<Tuple<string, string, double[]>> trainDataset,
            double[] testFeatures,
            int K
        )
        {
            // בניית רשימה לשמירת מרחק + תווית רגש
            List<Tuple<double, string>> distances = new List<Tuple<double, string>>();

            // חישוב מרחק מכל דוגמה בסט האימון
            foreach (var trainSample in trainDataset)
            {
                double distance = CalculateDistance(testFeatures, trainSample.Item3);
                distances.Add(new Tuple<double, string>(distance, trainSample.Item2));
            }

            // מיון לפי מרחק מהקטן לגדול
            var ordered = distances.OrderBy(x => x.Item1).ToList();

            // לקיחת K השכנים הקרובים
            var neighbors = ordered.Take(K);

            // ספירת התדירות של כל רגש
            Dictionary<string, int> frequency = new Dictionary<string, int>();
            foreach (var neighbor in neighbors)
            {
                string emotion = neighbor.Item2;
                if (!frequency.ContainsKey(emotion))
                    frequency[emotion] = 0;
                frequency[emotion]++;
            }

            // בחירת הרגש הנפוץ ביותר
            int maxCount = -1;
            string predicted = null;
            foreach (var kvp in frequency)
            {
                if (kvp.Value > maxCount)
                {
                    maxCount = kvp.Value;
                    predicted = kvp.Key;
                }
            }

            return predicted;
        }
    }
}
