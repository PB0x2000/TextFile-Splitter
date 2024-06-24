using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace TextFile_Splitter
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.WriteLine("Enter Max Lines per File: ");
            int limit = int.Parse(Console.ReadLine());
            foreach (string arg in args)
            {
                if (File.Exists(arg))
                {
                    SplitTextFile(arg, limit);
                }
            }
        }
        private static void SplitTextFile(string filePath, int maxLines)
        {
            string baseFileName = Path.GetFileNameWithoutExtension(filePath);
            string outputDirectory = Path.Combine(Path.GetDirectoryName(filePath), baseFileName + "_Split");
            Directory.CreateDirectory(outputDirectory);

            long totalLines = File.ReadLines(filePath).LongCount();
            long processedLines = 0;
            int filePart = 1;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (StreamReader reader = new StreamReader(filePath))
            {
                bool endOfFile = false;

                while (!endOfFile)
                {
                    string partFileName = $"{baseFileName}_Part{filePart}.txt";
                    string partFilePath = Path.Combine(outputDirectory, partFileName);

                    using (StreamWriter writer = new StreamWriter(partFilePath))
                    {
                        for (int i = 0; i < maxLines; i++)
                        {
                            if (reader.ReadLine() is string line)
                            {
                                writer.WriteLine(line);
                                processedLines++;
                            }
                            else
                            {
                                endOfFile = true;
                                break;
                            }
                        }
                    }

                    if (stopwatch.ElapsedMilliseconds > 1000 || endOfFile)
                    {
                        UpdateProgress(processedLines, totalLines, filePart);
                        stopwatch.Restart();
                    }

                    filePart++;
                }
            }

            Console.WriteLine($"\nSplitting completed. Files are saved in: {outputDirectory}");
        }

        private static void UpdateProgress(long processed, long total, int part)
        {
            Console.CursorLeft = 0;
            Console.Write($"Processed file part: {part}, Lines: {processed}/{total} ({(double)processed / total:P})");
            Console.Title = $"Splitting Progress: {(double)processed / total:P}";
        }
    }
}
