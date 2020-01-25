using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using Konbert.AudioProcessing;
using Konbert.UI.Models;
using Microsoft.Win32;

namespace Konbert.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string AudioFilesFilter = "Audio Files|*.mp3;*.m4a;*.wma";

        private readonly AudioProcessor audioProcessor;

        public ObservableCollection<AudioFile> SourceFilePaths { get; }

        public MainWindow()
        {
            InitializeComponent();

            SourceFilePaths = new ObservableCollection<AudioFile>();
            DataContext = this;

            audioProcessor = new AudioProcessor();
            audioProcessor.FileProcessed += Processor_FileProcessed;
        }

        private void Processor_FileProcessed(object sender, string filePath)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.IsIndeterminate = false;
                ProgressBar.Value += 1;
                ProcessingLabel.Content = $"Converting {filePath}";
            });
        }

        private void AddFilesButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDilaog = new OpenFileDialog
            {
                Filter = AudioFilesFilter,
                Title = "Select Audio Files",
                Multiselect = true
            };

            if (openFileDilaog.ShowDialog() == true)
            {
                foreach (var filePath in openFileDilaog.FileNames)
                {
                    SourceFilePaths.Add(new AudioFile(filePath));
                }
            }

            ToggleConvertButton();
        }

 
        private void RemoveFilesButton_Click(object sender, RoutedEventArgs e)
        {
            var audioFilesToRemove = new List<AudioFile>(); 

            foreach (var selectedItem in SourceFilesListView.SelectedItems)
            {
                var audioFile = selectedItem as AudioFile;
                audioFilesToRemove.Add(audioFile);
            }

            foreach (var audioFile in audioFilesToRemove)
            {
                SourceFilePaths.Remove(audioFile);
            }

            ToggleConvertButton();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Version 0.1.0", "About Konbert");
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButtonsEnablement(false);

            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.IsIndeterminate = true;

            ProcessingLabel.Content = "";

            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var inputFilePaths = SourceFilePaths.Select(f => f.FilePath);

                var processingTask = Task.Run(() =>
                {
                    audioProcessor.Start(inputFilePaths, folderBrowserDialog.SelectedPath);
                });

                processingTask.ContinueWith((t) =>
                {
                    ToggleButtonsEnablement(true);

                    ProgressBar.Value = 0;
                    ProgressBar.Visibility = Visibility.Collapsed;

                    ProcessingLabel.Content = "Converting finished";
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void ToggleConvertButton()
        {
            if (SourceFilePaths.Count == 0)
            {
                ConvertButton.IsEnabled = false;
            } else
            {
                ConvertButton.IsEnabled = true;
            }
        }

        private void ToggleButtonsEnablement(bool isEnabled)
        {
            AddFilesButton.IsEnabled = isEnabled;
            RemoveFilesButton.IsEnabled = isEnabled;
            
            ToggleConvertButton();
        }

    }
}
