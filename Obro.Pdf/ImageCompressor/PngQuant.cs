﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Medallion.Shell;

namespace Obro.Pdf.ImageCompressor
{

    public class PngQuantOptions
    {
        public (int, int)? QualityMinMax = null;
        public int Speed = 3;
        public bool IEBug = false;
        public int Bit = 256;

        public bool IsValid()
        {

            if (QualityMinMax != null && QualityMinMax.Value.Item1 > QualityMinMax.Value.Item2) return false;

            if (Bit < 0) return false;
            if (Speed < 0) return false;
            if (Speed > 11) return false;
            return true;
        }

        public string AsArgument()
        {
            string iebug = IEBug ? "--iebug " : "";
            string quality = QualityMinMax == null ? "" : $"--quality={QualityMinMax.Value.Item1}-{QualityMinMax.Value.Item2}";
            return $"{Bit} --speed {Speed} {iebug}{quality}";
        }
    }

    public class PngQuant : Compressor
    {
        public PngQuant(PngQuantOptions options = null) : base(new[] { "image/png" })
        {

            options = options ?? new PngQuantOptions();
            if (!options.IsValid()) throw new ArgumentException("Options are not valid!");
        }

        public override async Task<byte[]> CompressImplementation(byte[] input)
        {
            //var pro = Process.Start(_info);
            //using (var outputStream = new MemoryStream())
            //{
            //    await pro.StandardInput.BaseStream.WriteAsync(input, 0, input.Length);
            //    await pro.StandardOutput.BaseStream.CopyToAsync(outputStream);
            //    byte[] output = outputStream.ToArray();
            //    return output;
            //}
            using (MemoryStream ms = new MemoryStream(input), output = new MemoryStream())
            {
                var command = Command.Run("pngquant", "256", "--speed", "11") < ms > output;
                await command.Task.ConfigureAwait(false);
                return output.ToArray();

            }


        }

    }
}
