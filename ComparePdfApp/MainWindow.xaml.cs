using Obro.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            var pdfs = Directory.GetFiles(directory, "*.pdf",SearchOption.AllDirectories);
            var list = new List<ComparePdfData>();
            if (pdfs.Any())
            {
                foreach (var pdf in pdfs)
                {
                    // Initialise the MuPDF context. This is needed to open or create documents.

                    using var ctx = new MuPDFContext();

                    // Open a PDF document
                    using var pdfDoc = new MuPDFDocument(ctx, pdf);
                    var fi = new FileInfo(pdf);
                    list.AddRange(pdfDoc.Pages.Select(page => new ComparePdfData {PdfName = fi.Name, PageName = $"Sayfa : {page.PageNumber + 1}", Width = page.Bounds.Width, Height = page.Bounds.Height}));
                }

                foreach (var page in list)
                {
                    if (Math.Abs(list.FirstOrDefault()!.Width - page.Width) > 0 || Math.Abs(list.FirstOrDefault()!.Height - page.Height) > 0)
                    {
                        AddMessage($"{page.Width} x {page.Height}  ----  {page.PdfName} - {page.PageName}", Colors.Orange);
                    }
                    else
                    {
                        AddMessage($"{page.Width} x {page.Height}  ----  {page.PdfName} - {page.PageName}", Colors.Green);
                    }
                }
                var maxWidth = list.Max(c => c.Width);
                var minWidth = list.Min(c => c.Width);
                if (Math.Abs(maxWidth - minWidth) > 0)
                {
                    AddMessage($"Farklı genişlikte sayfalar mevcut {maxWidth} - {minWidth} farklı sayfaları yukarıdaki detaylardan kontrol edebilirsiniz", Colors.Red);
                }
                var maxHeight = list.Max(c => c.Height);
                var minHeignt = list.Min(c => c.Height);
                if (Math.Abs(maxHeight - minHeignt) > 0)
                {
                    AddMessage($"Farklı yükseklikte sayfalar mevcut - {maxHeight} - {minHeignt} farklı sayfaları yukarıdaki detaylardan kontrol edebilirsiniz", Colors.Red);
                }
                AddMessage("Karşılaştırma tamamlandı :)",Colors.Green);
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
            public float Width { get; set; }
            public float Height { get; set; }
        }

        private void ComparePdfClick(object sender, RoutedEventArgs e)
        {
            progress.Inlines.Clear();
            ComparePdf();
        }
    }
}
