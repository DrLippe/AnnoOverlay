using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace AnnoOverlay.Helpers
{
    public class IslandReader
    {
        public Thread ScanThread { get; private set; }

        public Process GameProcess { get; private set; }

        public string gameVersion { get; private set; }

        public IslandReader()
        {
            ScanThread = new Thread(Run)
            {
                IsBackground = true,
                Name = GetType().FullName
            };
        }

        private void Run(object param)
        {
            CancellationToken cancellationToken = (CancellationToken)param;

            while (true)
            {
                if (cancellationToken.WaitHandle.WaitOne(100))
                    break;

                if (GameProcess != null)
                {
                    // Ask if the overlay should be shut down on game exit
                    if (GameProcess.HasExited && MainWindow.settings.GameAddresses.IslandIdPointer != null)
                    {
                        if (Application.Current.Dispatcher.CheckAccess())
                        {
                            MainWindow.closingWindow.Show();
                        }
                        else
                        {
                            Application.Current.Dispatcher.BeginInvoke(
                              DispatcherPriority.Background,
                              new Action(() => {
                                  MainWindow.closingWindow.Show();
                              }));
                        }

                        // Clear process information
                        GameProcess = null;
                    }

                    ReadMemory();

                    continue;
                }

                try
                {
                    GetProcess();
                }
                catch (Exception)
                {
                }
            }
        }

        private void ReadMemory()
        {
            while(GameProcess != null  && MainWindow.settings != null)
            {
                if (GameProcess.HasExited)
                    break;
                ReadActiveIsland();
            }
        }

        private void ReadActiveIsland()
        {
            // get island id
            short islandId = 0;
            NativeMethods.ReadPointerPath(GameProcess, MainWindow.settings.GameAddresses.IslandIdPointer, ref islandId);

            MainWindow.viewModel.ActiveIslandId = islandId;

            // magically get island offset
            int[] offsets = MainWindow.settings.GameAddresses.IslandPopulationPointer;
            if (MainWindow.viewModel.IslandLinks.ContainsKey(islandId))
                offsets[4] = MainWindow.viewModel.IslandLinks[islandId];

            // get island population
            byte[] islandPopulation = new byte[100];
            NativeMethods.ReadPointerPath(GameProcess, offsets, ref islandPopulation);

            int inhabitantOffset = MainWindow.viewModel.UseFullHouse ? 0xC : 0x04;

            int[] population = new int[MainWindow.viewModel.Parameters.PopulationLevels.Length];
            for (int i = 0; i < population.Length; i++)
                population[i] = 0;

            foreach (int guid in MainWindow.settings.GameAddresses.IslandPopulationPosPtr) {
                // for each guid position
                int thisGuid = BitConverter.ToInt32(islandPopulation, guid);
                int[] array = Constants.populationGuids.Keys.ToArray();

                int i = Array.IndexOf(array, thisGuid);
                if (i > -1)
                    population[i] = BitConverter.ToInt32(islandPopulation, guid + inhabitantOffset);
            }

            for (int i = 0; i < population.Length; i++)
                MainWindow.viewModel.Parameters.PopulationLevels[i].Amount = population[i];

            Calculate();
        }

        private void GetProcess()
        {
            // Set game process
            GameProcess = Process.GetProcessesByName("Anno1800").FirstOrDefault();

            // Check game version
            if (GameProcess != null)
            {
                gameVersion = String.Format(@"{0}.{1}.{2}.{3}",
                    GameProcess.MainModule.FileVersionInfo.FileMajorPart,
                    GameProcess.MainModule.FileVersionInfo.FileMinorPart,
                    GameProcess.MainModule.FileVersionInfo.FileBuildPart,
                    GameProcess.MainModule.FileVersionInfo.FilePrivatePart);

                // Configure data structures
                MainWindow.settings = new Settings().Load(gameVersion);
                MainWindow.viewModel.Settings = MainWindow.settings;

                MainWindow.viewModel.Parameters = MainWindow.settings.Parameters;
                MainWindow.settings.MapIslands();
                MainWindow.settings.GameAddresses.GameVersion = gameVersion;
            }
        }

        private void Calculate()
        {
            foreach (Product product in MainWindow.viewModel.Parameters.Products)
            {
                product.Amount = 0;
            }

            foreach (PopulationLevel populationLevel in MainWindow.viewModel.Parameters.PopulationLevels)
            {
                foreach (Need need in populationLevel.Needs)
                {
                    if (need.Tpmin == null)
                        continue;

                    Product product = Array.Find(MainWindow.viewModel.Parameters.Products, (p) => p.Guid == need.Guid);
                    Factory producer = Array.Find(MainWindow.viewModel.Parameters.Factories, (f) => f.Guid == product.Producers[0]);

                    product.Amount += populationLevel.Amount * (double)need.Tpmin / (double)producer.Tpmin;
                }
            }
        }
    }
}