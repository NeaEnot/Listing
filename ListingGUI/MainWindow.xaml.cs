using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace ListingGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonInput_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();

            if (dlg.ShowDialog() == true)
            {
                string folder = dlg.SelectedPath;
                tbInput.Text = folder;
            }
        }

        private void buttonOutput_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dlg.FileName;
                tbOutput.Text = file;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<FileInfo> files = getAllFiles(tbInput.Text);
                List<FileInfo> operatingFiles = new List<FileInfo>();

                if (tbExts.Text == "*")
                {
                    operatingFiles.AddRange(files);
                }
                else
                {
                    foreach (string extension in tbExts.Text.Split())
                    {
                        operatingFiles.AddRange(files.Where(file => file.Extension == extension).ToList());
                    }
                }

                operatingFiles.Sort((f1, f2) => string.Compare(f1.FullName, f2.FullName));

                using (StreamWriter writer = new StreamWriter(tbOutput.Text)) ; // Чистим файл, если он уже есть

                foreach (FileInfo file in operatingFiles)
                {
                    string filename = file.FullName.Substring(tbInput.Text.Length);

                    Console.WriteLine(filename);

                    string text = "";

                    using (StreamReader reader = new StreamReader(file.FullName))
                    {
                        text = reader.ReadToEnd();
                    }

                    using (StreamWriter writer = new StreamWriter(tbOutput.Text, true))
                    {
                        writer.WriteLine("/*" + filename + "*/");
                        writer.WriteLine(text);
                    }
                }

                System.Windows.MessageBox.Show("READY", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<FileInfo> getAllFiles(string path)
        {
            List<FileInfo> files = new List<FileInfo>();
            DirectoryInfo directory = new DirectoryInfo(path);

            files.AddRange(directory.GetFiles());

            var directories = directory.GetDirectories();
            foreach (var dir in directories)
            {
                bool isIgnore = false;

                foreach (string ign in tbIgnore.Text.Split())
                {
                    if (dir.Name == ign)
                    {
                        isIgnore = true;
                        break;
                    }
                }

                if (isIgnore)
                {
                    continue;
                }

                files.AddRange(getAllFiles(dir.FullName));
            }

            return files;
        }
    }
}
