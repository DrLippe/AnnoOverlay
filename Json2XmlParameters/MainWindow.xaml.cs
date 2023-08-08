using AnnoOverlay;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Json2XmlParameters
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".json";
            dlg.Filter = "JSON Files (*.json)|*.json|JavaScript Files (*.js)|*.js";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                inputFile.Text = filename;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Parameters parameters;

            using (var stream = File.OpenRead(inputFile.Text))
            {
                var text = new StreamReader(stream).ReadToEnd();
                parameters = JsonConvert.DeserializeObject<Parameters>(text);
            }

            // Get %AppData% folder path and config file
            var dirName = System.IO.Path.GetDirectoryName(inputFile.Text);
            var fileName = System.IO.Path.Combine(dirName, System.IO.Path.GetFileNameWithoutExtension(inputFile.Text) + ".xml");

            using (var stream = File.Create(fileName))
            {
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Parameters));
                    xmlSerializer.Serialize(streamWriter, parameters);
                }
            }
        }
    }
}
