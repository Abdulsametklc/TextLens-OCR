# TextLens - Akıllı OCR Masaüstü Uygulaması

TextLens, modern arayüz tasarımı ile güçlü OCR (Optik Karakter Tanıma) teknolojisini birleştiren, kurulum gerektirmeyen taşınabilir bir Windows masaüstü uygulamasıdır. Görüntülerden, PDF'lerden ve belgelerden metinleri saniyeler içinde çıkarır.

## 🌟 Özellikler

*   **Tamamen Taşınabilir (Portable):** Tek bir `.exe` dosyası olarak çalışır. Kurulum, Python veya ek kütüphane gerektirmez.
*   **Modern Glassmorphism Arayüz:** WPF ile geliştirilmiş, yarı saydam cam efektleri, gradient arka planlar ve akıcı animasyonlar içeren premium tasarım.
*   **Çoklu Format Desteği:** 
    *   Görseller: PNG, JPG, JPEG, BMP, TIFF, WEBP
    *   Belgeler: DOCX (Word), TXT
*   **Çift Dil Desteği:** Türkçe ve İngilizce metinleri aynı anda yüksek doğrulukla tanır.
*   **Akıllı Pano ve Kayıt:** Sonuçları tek tıkla panoya kopyalayın veya `.txt` dosyası olarak kaydedin.
*   **Gömülü OCR Motoru:** Tesseract OCR motoru ve dil dosyaları uygulamanın içine gömülmüştür, internet bağlantısı gerektirmez.

## 🛠 Kullanılan Teknolojiler ve Yöntemler

Bu proje, modern .NET ekosistemi ve yazılım mimarisi prensipleri kullanılarak geliştirilmiştir.

### Core Stack
*   **Framework:** .NET 8 (Desktop Runtime)
*   **UI (Arayüz):** Windows Presentation Foundation (WPF) / XAML
*   **Dil:** C# 12

### Kütüphaneler & Araçlar
*   **Tesseract.NET:** Google'ın Tesseract OCR motorunun .NET wrapper'ı.
*   **DocumentFormat.OpenXml:** Word (.docx) belgelerini okumak için.
*   **System.Reflection:** Gömülü kaynak yönetimi için.

### Mimari ve Teknik Detaylar
*   **Self-Contained Single File Deployment:** Uygulama, .NET çalışma zamanını (runtime) ve tüm bağımlılıkları (DLL'ler) içinde barındıran sıkıştırılmış tek bir dosya olarak derlenmiştir.
*   **Embedded Resources (Gömülü Kaynaklar):** `tessdata` (dil öğrenme verileri) `.exe` dosyasının içine gömülmüştür. Uygulama çalışma zamanında bu verileri akıllıca geçici dizine (`%TEMP%`) çıkararak Tesseract'ın kullanmasını sağlar.
*   **Service-Oriented Architecture:** OCR işlemleri `OcrService` adında bağımsız, statik ve asenkron bir servis katmanında yönetilir. Bu, arayüzün donmasını engeller ve kodun okunabilirliğini artırır.
*   **Async/Await Pattern:** Tüm I/O ve OCR işlemleri asenkron olarak çalıştırılır, bu sayede akıcı bir kullanıcı deneyimi sağlanır.

## 🚀 Kurulum ve Kullanım

Uygulama kurulum gerektirmez.

1.  `OCR.exe` dosyasını indirin veya kopyalayın.
2.  Çift tıklayarak çalıştırın.
3.  **"Dosya Seç"** butonu ile görsel veya belgenizi yükleyin.
4.  **"OCR Başlat"** butonuna basın ve sonucu görün!

---
*Geliştirici: Abdulsamet KILIÇ*
