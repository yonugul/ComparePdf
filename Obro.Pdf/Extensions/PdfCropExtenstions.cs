using Obro.Pdf.ImageCompressor;
using Obro.Pdf.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Obro.Pdf.Extensions
{
    public static class PdfCropExtenstions
    {
        public static async Task<MemoryStream> CropPdfArea(this byte[] pdfBytes, int page, Rectangle rect)
        {
            try
            {
                // Initialise the MuPDF context. This is needed to open or create documents.
                using (var ctx = new MuPDFContext())
                {
                    using (var pdfDoc = new MuPDFDocument(ctx, pdfBytes, InputFileTypes.PDF))
                    {
                        return await pdfDoc.CropPdfArea(page, rect);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<MemoryStream> CropPdfArea(this MuPDFDocument pdfDoc, int page, Rectangle rect)
        {
            try
            {
                Compressor pngQuant = new PngQuant(new PngQuantOptions { QualityMinMax = (5, 10) });

                // Initialise the MuPDF context. This is needed to open or create documents.
                using (var ms = new MemoryStream())
                {
                    var pdfPage = pdfDoc.Pages[page - 1];
                    pdfDoc.WriteImage(pdfPage.PageNumber, rect, 6, PixelFormats.RGB, ms, RasterOutputFileTypes.PNG);
                    using (var compressedMs = new MemoryStream(await pngQuant.Compress(ms.GetBuffer())))
                    {
                        return compressedMs;
                    }

                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static async Task<MemoryStream> CropPdfArea(this byte[] pdfBytes, PdfPosition position)
        {
            try
            {
                // Initialise the MuPDF context. This is needed to open or create documents.
                using (var ctx = new MuPDFContext())
                {
                    using (var pdfDoc = new MuPDFDocument(ctx, pdfBytes, InputFileTypes.PDF))
                    {
                        return await pdfDoc.CropPdfArea(position);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<MemoryStream> CropPdfArea(this MuPDFDocument pdfDoc, PdfPosition position)
        {
            try
            {
                Compressor pngQuant = new PngQuant(new PngQuantOptions { QualityMinMax = (5, 10) });

                // Initialise the MuPDF context. This is needed to open or create documents.
                using (var ms = new MemoryStream())
                {
                    var pdfPage = pdfDoc.Pages[position.PageNumber - 1];
                    var xmlPosition = position.ToXmlPosition(pdfPage.Bounds.Width, pdfPage.Bounds.Height);
                    var rect = new Rectangle(xmlPosition.Llx, xmlPosition.Ury, xmlPosition.Urx, xmlPosition.Lly);
                    pdfDoc.WriteImage(pdfPage.PageNumber, rect, 6, PixelFormats.RGB, ms, RasterOutputFileTypes.PNG);
                    using (var compressedMs = new MemoryStream(await pngQuant.Compress(ms.GetBuffer())))
                    {
                        return compressedMs;
                    }

                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static XmlPosition ToXmlPosition(this PdfPosition position, double pageWidth, double pageHeight)
        {
            if (position == null)
            {
                return null;
            }

            var coefX = pageWidth / position.ImageWidth;
            var coefY = pageHeight / position.ImageHeight;

            var model = new XmlPosition
            {
                PageNumber = position.PageNumber,
                Llx = position.AxisX * coefX,
                Urx = (position.AxisX + position.Width) * coefX,
                Ury = position.AxisY * coefY,
                Lly = (position.AxisY + position.Height) * coefY
            };

            return model;
        }
    }
}
