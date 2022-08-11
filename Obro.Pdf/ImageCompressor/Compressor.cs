using System.Linq;
using System.Threading.Tasks;
using MimeDetective;

namespace Obro.Pdf.ImageCompressor
{
    public abstract class Compressor
    {
        public string[] MimeTypes { get; } 

        protected Compressor(string[] mimeTypes)
        {
            MimeTypes = mimeTypes;
        }


        public Task<byte[]> Compress(byte[] stream)
        {
            if (!MimeTypes.Contains(stream.GetFileType().Mime)) return Task.FromResult(stream);
            return CompressImplementation(stream);
        }

        public abstract Task<byte[]> CompressImplementation(byte[] stream);

    }
}
