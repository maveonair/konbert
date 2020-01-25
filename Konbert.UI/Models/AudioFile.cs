using ATL;

namespace Konbert.UI.Models
{
    public class AudioFile
    {
        public string FilePath { get; }

        public string Artist
        {
            get
            {
                return metaData.Artist;
            }
        }

        public string Title
        {
            get
            {
                return metaData.Title;
            }
        }

        public string Album
        {
            get
            {
                return metaData.Album;
            }
        }

        private readonly Track metaData;

        public AudioFile(string filePath)
        {
            FilePath = filePath;
            metaData = new Track(FilePath);
        }
    }
}
