using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;

namespace Konbert.AudioProcessing
{
    public class AudioProcessor
    {
        public event EventHandler<string> FileProcessed;

        private const int OutputRate = 32000; // 32kHz
        private const string OutputFileExtension = "WAV";

        public void Start(IEnumerable<string> inputFilesPath, string outputPath)
        {
            var fileNameNumber = 0;
            foreach (var inputFilePath in inputFilesPath)
            {
                var fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                var outputFilePath = Path.Combine(outputPath, $"{fileNameNumber}.{OutputFileExtension}");

                ProcessAudioFile(inputFilePath, outputFilePath);
                OnFileProcessed(inputFilePath);

                fileNameNumber++;
            }
        }

        private void ProcessAudioFile(string inputFilePath, string outputFilePath)
        {
            using var reader = new AudioFileReader(inputFilePath);

            var mono = new StereoToMonoSampleProvider(reader);
            var outputFormat = new WaveFormat(OutputRate, mono.WaveFormat.Channels);

            using var resampler = new MediaFoundationResampler(mono.ToWaveProvider(), outputFormat);
            WaveFileWriter.CreateWaveFile16(outputFilePath, resampler.ToSampleProvider());
        }

        private void OnFileProcessed(string inputFilePath)
        {
            var handler = FileProcessed;
            handler?.Invoke(this, inputFilePath);
        }
    }
}
