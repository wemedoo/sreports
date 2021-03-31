using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntheticPatientGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var samples = new double[10000];

            Normal.Samples(samples, 50, 10);
            int i = 0;
            foreach (double sample in samples)
            {
                Console.WriteLine(sample);
                if (sample < 0)
                {
                    i++;
                }
            }

            Console.WriteLine("=====================BINOMINAL===================");
            var samplesBi1 = new int[1];
            Binomial.Samples(samplesBi1, 0.9, 1000);
            foreach (int s in samplesBi1)
            {
                if (s == 0)
                {
                    i++;
                }
                Console.WriteLine(s);
            }
            Console.WriteLine("=====================MULTINOMINAL===================");

            double[] weights = { 0.5, 0.1, 0.2, 0.1, 0.1 };
            Multinomial multinomial = new Multinomial(weights, 10000);
            var test = multinomial.Samples().Take(1);
            foreach(int[] t in test)
            {
               foreach(int e in t)
                {
                    Console.WriteLine(e);
                    Console.WriteLine();
                }
            }
        }
    }
}
