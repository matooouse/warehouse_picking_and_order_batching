using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using warehouse_picking_core;

namespace warehouse_picking_console
{
    class Program
    {
        static void Main(string[] args)
        {
            int nbBlock;
            int nbAisle;
            int aisleLenght;
            int nbRun;
            int pickingListSize;
            string algoUsed;
            if (args.Length == 6)
            {
                nbBlock = ConvertToInt(args[0]);
                nbAisle = ConvertToInt(args[1]);
                aisleLenght = ConvertToInt(args[2]);
                nbRun = ConvertToInt(args[3]);
                pickingListSize = ConvertToInt(args[4]);
                algoUsed = args[5];
            }
            else
            {
                nbBlock = ConvertToInt(ConfigurationManager.AppSettings["nbBlock"]);
                nbAisle = ConvertToInt(ConfigurationManager.AppSettings["nbAisle"]);
                aisleLenght = ConvertToInt(ConfigurationManager.AppSettings["aisleLenght"]);
                nbRun = ConvertToInt(ConfigurationManager.AppSettings["nbRun"]);
                pickingListSize = ConvertToInt(ConfigurationManager.AppSettings["pickingListSize"]);
                algoUsed = ConfigurationManager.AppSettings["algoUsed"];
            }
            var solversName = algoUsed.Split(';');
            List<float> accumulated = null;
            for (int i = 0; i < nbRun; i++)
            {
                var problem = WarehousePickingCoreGenerator.GenerateProblem(nbBlock, nbAisle, aisleLenght,
                    pickingListSize);
                var solvers =
                    solversName.Select(
                        x => WarehousePickingCoreGenerator.GenerateSolver(x, problem.Item1, problem.Item2));
                var solutions = solvers.Select(s => (float) s.Solve().Length()).ToList();
                if (accumulated == null)
                {
                    accumulated = solutions;
                }
                else
                {
                    for (int j = 0; j < accumulated.Count; j++)
                    {
                        accumulated[j] += solutions[j];
                    }
                }
            }
            Debug.Assert(accumulated != null, "accumulated != null");
            for (int j = 0; j < accumulated.Count; j++)
            {
                accumulated[j] /= (float)nbRun;
            }
            var res = String.Join(";", accumulated);
            Console.WriteLine(res);
            Console.ReadLine();
        }

        private static int ConvertToInt(string param)
        {
            if (param.Trim().ToLower().Equals("rand") ||
                param.Trim().ToLower().Equals("random") ||
                param.Trim().ToLower().Equals("rnd"))
            {
                var rnd = new Random();
                return rnd.Next(1, 25);
            }
            try
            {
                return int.Parse(param);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not convert in int " + param + "default to 1");
                return 1;
            }
        }
    }
}
