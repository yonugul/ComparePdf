using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace ComparePdfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ComparePdf()
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var pdfs = Directory.GetFiles(directory, "*.pdf", SearchOption.AllDirectories);
            var list = new List<ComparePdfData>();
            if (pdfs.Any())
            {
                foreach (var pdf in pdfs)
                {
                    var fi = new FileInfo(pdf);
                    // Initialise the MuPDF context. This is needed to open or create documents.
                    //var pdfDoc = new PdfDocument(fi.OpenRead());
                    var pdfDoc = PdfReader.Open(pdf, PdfDocumentOpenMode.Import);
                    //using var ctx = new MuPDFContext();

                    // Open a PDF document
                    //using var pdfDoc = new MuPDFDocument(ctx, pdf);

                    var pageNo = 1;
                    foreach (var page in pdfDoc.Pages)
                    {
                        list.Add(new ComparePdfData
                        {
                            PdfName = fi.Name,
                            PageName = $"Sayfa : {pageNo}",
                            Width = page.Width.Value,
                            Height = page.Height.Value
                        });
                        pageNo++;
                    }
                    //list.AddRange(pdfDoc.Pages.Select(page => new ComparePdfData {PdfName = fi.Name, PageName = $"Sayfa : {page.PageNumber + 1}", Width = page.Bounds.Width, Height = page.Bounds.Height}));
                }
                var maxWidth = list.Max(c => c.Width);
                var minWidth = list.Min(c => c.Width);
                var hasDifferentWidth = Math.Abs(maxWidth - minWidth) > 0;
                var maxHeight = list.Max(c => c.Height);
                var minHeignt = list.Min(c => c.Height);
                var hasDifferentHeight = Math.Abs(maxHeight - minHeignt) > 0;
                var mostWidth = list.GroupBy(i => i.Width).OrderByDescending(grp => grp.Count())
                    .Select(grp => grp.Key).First();
                var mostHeight = list.GroupBy(i => i.Height).OrderByDescending(grp => grp.Count())
                    .Select(grp => grp.Key).First();
                if (hasDifferentHeight || hasDifferentWidth)
                {
                    foreach (var page in list)
                    {
                        if (Math.Abs(mostWidth - page.Width) > 0 || Math.Abs(mostHeight - page.Height) > 0)
                        {
                            AddMessage($"{page.Width} x {page.Height}  ----  {page.PdfName} - {page.PageName}", Colors.Orange);
                        }
                        else
                        {
                            AddMessage($"{page.Width} x {page.Height}  ----  {page.PdfName} - {page.PageName}", Colors.Green);
                        }
                    }
                    if (hasDifferentWidth)
                    {
                        AddMessage($"Farklı genişlikte sayfalar mevcut {maxWidth} - {minWidth} farklı sayfaları yukarıdaki detaylardan kontrol edebilirsiniz", Colors.Red);
                    }

                    if (hasDifferentHeight)
                    {
                        AddMessage($"Farklı yükseklikte sayfalar mevcut - {maxHeight} - {minHeignt} farklı sayfaları yukarıdaki detaylardan kontrol edebilirsiniz", Colors.Red);
                    }
                }
                else
                {
                    AddMessage("Farklı boyutlarda sayfa bulunamadı", Colors.Green);
                }

                AddMessage("Karşılaştırma tamamlandı :)", Colors.Green);
            }
            else
            {
                AddMessage("Eee hani pdfler eklemedin? :( ", Colors.Red, true);
            }
        }
        private void AddMessage(string text, Color? c = null, bool clearPrevious = false)
        {
            if (clearPrevious)
            {
                progress.Inlines.Clear();
            }
            else
            {
                progress.Inlines.Add(new LineBreak());
            }
            var textcolor = c ?? Colors.Green;

            var newText = new Run
            {
                Text = text,
                Foreground = new SolidColorBrush(textcolor)
            };
            progress.Inlines.Add(newText);
            Application.Current.Dispatcher?.Invoke(DispatcherPriority.Background, new Action(UpdateLayout));
            scrollViewer.ScrollToBottom();
        }

        public class ComparePdfData
        {
            public string? PdfName { get; set; }
            public string? PageName { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }

        private void ComparePdfClick(object sender, RoutedEventArgs e)
        {
            progress.Inlines.Clear();
            ComparePdf();
        }
    }
}
