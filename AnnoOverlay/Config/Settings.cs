using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;

namespace AnnoOverlay
{
    public class Settings : BaseViewModel
    {
        public GameAddresses GameAddresses { get; set; }

        public Parameters Parameters { get; set; }

        public int[] IslandLinkOffsets { get; set; }

        public Settings()
        {
        }

        #region store and load settings

        /// <summary>
        /// Map islands according to config
        /// </summary>
        public void MapIslands()
        {
            MainWindow.viewModel.IslandLinks = new Dictionary<int, int>();
            MainWindow.viewModel.IslandLinksByZone = new Dictionary<int, int>[IslandLinkOffsets.Length];
            MainWindow.viewModel.IslandFactoryCount = new Dictionary<int, Dictionary<int, int>>();

            foreach (int offset in IslandLinkOffsets)
            {
                MainWindow.viewModel.IslandLinksByZone[Array.IndexOf(IslandLinkOffsets, offset)] = new Dictionary<int, int>();

                for (int i = 0; i < 60; i++)
                {
                    if (0x108 + (i * 16) > 0x608)
                        break;

                    MainWindow.viewModel.IslandLinks.Add(offset + (i * 64), 0x108 + (i * 16));
                    MainWindow.viewModel.IslandLinksByZone[Array.IndexOf(IslandLinkOffsets, offset)].Add(offset + (i * 64), 0x108 + (i * 16));
                }
            }
        }

        /// <summary>
        /// Remap islands
        /// </summary>
        public void MapIslands(int selectedId)
        {
            var links = MainWindow.viewModel.IslandLinks;

            // remove all links of this session
            foreach (var item in MainWindow.viewModel.IslandLinks.Where(keyValuePair => keyValuePair.Key.ToString("X").Remove(0, 3) == MainWindow.viewModel.ActiveIslandId.ToString("X").Remove(0, 3)).ToList())
                MainWindow.viewModel.IslandLinks.Remove(item.Key);

            // set new islandLinkOffset
            int offset = Array.Find(MainWindow.viewModel.Settings.IslandLinkOffsets, i => 
                i.ToString("X").Remove(0, 3) == MainWindow.viewModel.ActiveIslandId.ToString("X").Remove(0, 3));
            int index = Array.IndexOf(IslandLinkOffsets, offset);
            MainWindow.viewModel.IslandLinksByZone[index] = new Dictionary<int, int>();

            // create new links
            for (int i = -60; i < 60; i++)
            {
                if (selectedId + (i * 16) == 0x108)
                {
                    offset = MainWindow.viewModel.ActiveIslandId + (i * 64);
                    break;
                }
            }

            for (int i = 0; i < 60; i++)
            {
                if (0x108 + (i * 16) > 0x608)
                    break;

                MainWindow.viewModel.IslandLinks.Add(offset + (i * 64), 0x108 + (i * 16));
                MainWindow.viewModel.IslandLinksByZone[index].Add(offset + (i * 64), 0x108 + (i * 16));
            }
        }

        public void MapIslands(int offset, int islandId)
        {
            foreach (var item in MainWindow.viewModel.IslandLinks.Where(keyValuePair => keyValuePair.Key.ToString("X").Remove(0, 3) == MainWindow.viewModel.ActiveIslandId.ToString("X").Remove(0, 3)).ToList())
                MainWindow.viewModel.IslandLinks.Remove(item.Key);

            // zone offsets
            // alte welt = 0
            // neue welt = 1
            // kap = 2
            // arktis = 3
            // enbesa = 4

            var zoneOffset = Array.Find(MainWindow.viewModel.Settings.IslandLinkOffsets, i => i.ToString("X").Remove(0, 3) == islandId.ToString("X").Remove(0, 3));
            int zoneIndex = Array.IndexOf(IslandLinkOffsets, zoneOffset);
            MainWindow.viewModel.IslandLinksByZone[zoneIndex] = new Dictionary<int, int>();

            while (offset > 0x108)
            {
                offset -= 0x10;
                islandId -= 0x40;
            }

            while (offset < 0x608)
            {
                if (zoneIndex == 4 && offset == 0x2B8)
                {
                    MainWindow.viewModel.IslandLinks.Add(16771, offset);
                    MainWindow.viewModel.IslandLinksByZone[4].Add(16771, offset);
                }
                else
                {
                    MainWindow.viewModel.IslandLinks.Add(islandId, offset);
                    MainWindow.viewModel.IslandLinksByZone[zoneIndex].Add(islandId, offset);
                }

                offset += 0x10;
                islandId += 0x40;
            }

        }

        /// <summary>
        /// This method is used to create new config files. 
        /// </summary>
        /// <param name="gameVersion">The game version the saved file is targeted</param>
        public void Save(string gameVersion)
        {
            // Get %AppData% folder path and config file
            string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AnnoOverlay");
            string configFile = Path.Combine(configDir, $"{gameVersion}.xml");

            // Create directory and file if needed
            if (!Directory.Exists(configDir))
                Directory.CreateDirectory(configDir);
            File.Create(configFile).Dispose();

            using (StreamWriter streamWriter = new StreamWriter(configFile))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                xmlSerializer.Serialize(streamWriter, this);
            }
        }

        /// <summary>
        /// Loads the last saved user config if available
        /// </summary>
        /// <param name="gameVersion">The game version for which the config should be loaded</param>
        /// <returns>Returns loaded user config or default settings if none exist</returns>
        public Settings Load(string gameVersion)
        {
            // Get %AppData% folder path and config file
            string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AnnoOverlay");
            string configFile = Path.Combine(configDir, $"{gameVersion}.xml");

            try
            {
                if (!File.Exists(configFile))
                {
                    try
                    {
                        string url = $"https://drlippe.github.io/AnnoOverlay/versions/{gameVersion}.xml";

                        using (WebClient client = new WebClient())
                        {
                            if (!Directory.Exists(configDir))
                                Directory.CreateDirectory(configDir);
                            client.DownloadFile(url, configFile);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                using (StreamReader streamReader = new StreamReader(configFile))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                    return xmlSerializer.Deserialize(streamReader) as Settings;
                }
            }
            catch (Exception)
            {
                return new Settings();
            }

        }
        #endregion
    }
}
