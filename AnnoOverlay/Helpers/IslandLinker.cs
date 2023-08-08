using AnnoOverlay.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AnnoOverlay
{
    public class IslandPopulationLevel
    {
        public int Guid { get; set; }
        public int Amount { get; set; }
    }

    public class Island
    {
        public int Offset { get; set; }
        public IslandPopulationLevel[] PopulationLevels { get; set; }

        public Island()
        {
            PopulationLevels = new IslandPopulationLevel[MainWindow.viewModel.Parameters.PopulationLevels.Length];

            for (int i = 0; i < PopulationLevels.Length; i++)
            {
                PopulationLevels[i] = new IslandPopulationLevel()
                {
                    Guid = MainWindow.viewModel.Parameters.PopulationLevels[i].Guid,
                    Amount = 0
                };
            }
        }
    }

    public class IslandLinker
    {
        public Thread ScanThread { get; private set; }

        private List<Island> islands;

        public IslandLinker()
        {
            ScanThread = new Thread(Run)
            {
                IsBackground = true,
                Name = GetType().FullName
            };
        }
        public void Run(object param)
        {
            CancellationToken cancellationToken = (CancellationToken)param;

            while (true)
            {
                islands = new List<Island>();
                int[] offsets = (int[])MainWindow.settings.GameAddresses.IslandPopulationPointer.Clone();

                int islandOffset = 0x108;
                int inhabitantOffset = 0x08;

                while (islandOffset < 0x608)
                {
                    offsets[4] = islandOffset;

                    // get size of population based on provided offsets
                    var populationPosPtr = MainWindow.settings.GameAddresses.IslandPopulationPosPtr;
                    int size = populationPosPtr[1];
                    int lastPtr = populationPosPtr.Last();

                    byte[] islandPopulation = new byte[lastPtr + size];

                    // get island population
                    NativeMethods.ReadPointerPath(MainWindow.islandReader.GameProcess, offsets, ref islandPopulation);

                    Island island = new Island() { Offset = islandOffset };

                    int[] population = new int[island.PopulationLevels.Length];
                    for (int i = 0; i < population.Length; i++)
                        population[i] = 0;

                    foreach (int guid in MainWindow.settings.GameAddresses.IslandPopulationPosPtr)
                    {
                        // for each guid position
                        int thisGuid = BitConverter.ToInt32(islandPopulation, guid);
                        int[] array = Constants.populationGuids.Keys.ToArray();

                        int i = Array.IndexOf(array, thisGuid);
                        if (i > -1)
                            population[i] = BitConverter.ToInt32(islandPopulation, guid + inhabitantOffset);
                    }

                    // check if island has at least some population
                    if (population.Sum() > 0)
                    {
                        for (int i = 0; i < population.Length; i++)
                            island.PopulationLevels[i].Amount = population[i];

                        islands.Add(island);
                    }

                    islandOffset += 0x10;
                }

                MainWindow.viewModel.Islands = islands.ToArray();

                if (cancellationToken.WaitHandle.WaitOne(1000))
                    break;
            }

        }
    }
}
