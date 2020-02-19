using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AudioPOC
{
    class Program
    {
        //For some reason optimzations break everything???
        [MethodImpl(MethodImplOptions.NoOptimization)]
        static void Main(string[] args)
        {
            Console.WriteLine("input audio option");
            int opt = int.Parse(Console.ReadLine());
            Console.Clear();

            List<WaveFileReader> streams = new List<WaveFileReader>();
            long sampleStart;

            switch (opt)
            {
                case 1:
                    sampleStart = 200704;
                    streams.Add(new WaveFileReader("Resources/set1.wav"));
                    streams.Add(new WaveFileReader("Resources/set2.wav"));
                    break;
                case 2:
                    sampleStart = 329728;
                    streams.Add(new WaveFileReader("Resources/back1.wav"));
                    streams.Add(new WaveFileReader("Resources/full1.wav"));
                    break;
                case 3:
                    sampleStart = 556896;
                    streams.Add(new WaveFileReader("Resources/solo.wav"));
                    streams.Add(new WaveFileReader("Resources/duo.wav"));
                    break;
                case 4:
                    sampleStart = 616448;
                    streams.Add(new WaveFileReader("Resources/b1.wav"));
                    streams.Add(new WaveFileReader("Resources/b2.wav"));
                    streams.Add(new WaveFileReader("Resources/b3.wav"));
                    break;
                case 5:
                    sampleStart = 200704;
                    streams.Add(new WaveFileReader("Resources/b01.wav"));
                    streams.Add(new WaveFileReader("Resources/b02.wav"));
                    streams.Add(new WaveFileReader("Resources/b03.wav"));
                    break;
                default:
                    throw new Exception("Invalid option!");
            }

            LoopStreams waveProvider = new LoopStreams(streams.ToArray(), sampleStart, 0);

            WaveOut waveOut = new WaveOut();
            waveOut.Init(waveProvider);
            waveOut.Play();
            Thread t = new Thread(() =>
            {
                int pos = 0;
                while (true)
                {
                    Console.ReadKey(true);
                    pos++;

                    if (pos < 0)
                        pos = waveProvider.ArrayLength - 1;
                    if (waveProvider.ArrayLength <= pos)
                        pos = 0;

                    waveProvider.ArrayPosition = pos;
                }
            });
            t.Start();
            while (true)
            {
                Console.Write($"\r{streams.First().CurrentTime.ToString(@"mm\:ss")}/{streams.First().TotalTime.ToString(@"mm\:ss")}");
            }
        }
    }
}
