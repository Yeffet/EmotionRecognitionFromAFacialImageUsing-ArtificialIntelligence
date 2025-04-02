using System;
using System.Collections.Generic;
using System.IO;
using EmotionDetector;

class Program
{
    static void Main()
    {
        Console.WriteLine("Starting Emotion Detector...");

        // נתיב למאגר התמונות ולתיקיית הפלט
        string trainPath = @"C:\AMIT\YG\EmotionDetectorProject\train";
        string testPath = @"C:\AMIT\YG\EmotionDetectorProject\test";

        string outputPath = @"C:\\AMIT\\YG\\EmotionDetectorProject\\output";

        string trainJsonFile = Path.Combine(outputPath, "features_train.json");
        string testJsonFile = Path.Combine(outputPath, "features_test.json");

        // בדיקת תקינות המבנה
        DatasetManager.VerifyDataset(trainPath);
        DatasetManager.VerifyDataset(testPath);

        // חילוץ התכונות מהתמונה לקובץ ה jason
        FeatureExtractor.ExtractFeatures(trainPath, trainJsonFile);
        FeatureExtractor.ExtractFeatures(testPath, testJsonFile);

        // טוען את הנתונים מה json 
        List<Tuple<string, string, double[]>> trainDataset = JsonHandler.LoadFeatures(trainJsonFile);
        List<Tuple<string, string, double[]>> testDataset = JsonHandler.LoadFeatures(testJsonFile);

        // 5. בוא נריץ את KNN על כל תמונה ב־testDataset
        int K = 9;  // למשל K=5
        int correctCount = 0; // כמה ניחושים נכונים
        for (int i = 0; i < testDataset.Count; i++)
        {
            // דגימה אחת מסט הבדיקה
            var testSample = testDataset[i];
            string testImageName = testSample.Item1;   // שם הקובץ
            string actualEmotion = testSample.Item2;   // הרגש האמיתי
            double[] testFeatures = testSample.Item3;  // התכונות של התמונה החדשה

            // ניבוי הרגש בעזרת KNN
            string predictedEmotion = KnnClassifier.Classify(trainDataset, testFeatures, K);

            // השוואה לרגש האמיתי
            if (predictedEmotion == actualEmotion)
            {
                correctCount++;
                Console.WriteLine($"Image: {testImageName} | Actual: {actualEmotion}, Predicted: {predictedEmotion} ✅");
            }
            else
            {
                Console.WriteLine($"Image: {testImageName} | Actual: {actualEmotion}, Predicted: {predictedEmotion} ❌");
            }
        }

        // 6. חישוב דיוק האלגוריתם
        double accuracy = (double)correctCount / testDataset.Count * 100.0;
        Console.WriteLine($"\nKNN Accuracy = {accuracy}% (K={K})");

        Console.WriteLine("Process completed. Press any key to exit.");
        Console.ReadKey();
    

    Console.WriteLine("Process completed. Press any key to exit.");
        Console.ReadKey();
    }
}

