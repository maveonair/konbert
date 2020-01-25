using ATL;
using System;
using System.IO;
using Xunit;

namespace Konbert.AudioProcessing.Tests
{
    public class AudioProcessorTests
    {
        [Fact]
        public void Start_ShouldConvertAudioFile_WhenGivenMp3()
        {
            var inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Fixtures", "mp3", "Jahzzar_-_05_-_Siesta.mp3");
            var outputPath = Directory.GetCurrentDirectory();


            var audioProcessor = new AudioProcessor();
            audioProcessor.FileProcessed += (sender, processedAudioFilePath) =>
            {
                Assert.Equal(inputFilePath, processedAudioFilePath);


                var outputFilePath = Path.Combine(outputPath, "0.WAV");
                Assert.True(File.Exists(outputFilePath));

                var track = new Track(outputFilePath);
                Assert.Equal(32000, track.SampleRate);
                Assert.Equal("Mono (1/0.0)", track.ChannelsArrangement.Description);
            };

            audioProcessor.Start(new string[] { inputFilePath }, outputPath);
        }
    }
}
