using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using OCR.Services;

namespace OCR
{
    public partial class MainWindow : Window
    {
        private string? _selectedFile;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        // Window Control Methods
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PickFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "All Supported|*.png;*.jpg;*.jpeg;*.bmp;*.tiff;*.docx;*.txt|All files|*.*"
            };

            if (ofd.ShowDialog() == true)
            {
                _selectedFile = ofd.FileName;
                FileNameText.Text = Path.GetFileName(_selectedFile);
                StatusText.Text = "Dosya seçildi. OCR başlatmaya hazır.";
                StatusIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#38ef7d"));
            }
        }

        private async void RunOcr_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedFile))
            {
                MessageBox.Show("Önce dosya seç.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!File.Exists(_selectedFile))
            {
                MessageBox.Show("Dosya bulunamadı!", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                StatusText.Text = "OCR işlemi devam ediyor...";
                StatusIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#667eea"));
                ResultBox.Text = "";

                var text = await OcrService.ProcessFileAsync(_selectedFile, "eng+tur");
                ResultBox.Text = text;

                // Butonları aktifleştir
                CopyButton.IsEnabled = true;
                SaveButton.IsEnabled = true;

                StatusText.Text = "Tamamlandı! Metni kopyalayabilir veya kaydedebilirsiniz.";
                StatusIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#38ef7d"));
            }
            catch (Exception ex)
            {
                StatusText.Text = "Hata oluştu!";
                StatusIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e81123"));
                MessageBox.Show(ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CopyText_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ResultBox.Text) && ResultBox.Text != "OCR sonuçları burada görünecek...")
            {
                Clipboard.SetText(ResultBox.Text);
                StatusText.Text = "✓ Metin panoya kopyalandı!";
            }
        }

        private void SaveTxt_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ResultBox.Text) || ResultBox.Text == "OCR sonuçları burada görünecek...")
            {
                MessageBox.Show("Kaydedilecek metin yok.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var sfd = new SaveFileDialog
            {
                Filter = "Metin Dosyası|*.txt",
                DefaultExt = "txt",
                FileName = "ocr_sonuc"
            };

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(sfd.FileName, ResultBox.Text, Encoding.UTF8);
                    StatusText.Text = $"✓ Dosya kaydedildi: {Path.GetFileName(sfd.FileName)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kayıt hatası: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

