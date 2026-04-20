using System;
using System.Windows;
using BetonMetraj.Models;

namespace BetonMetraj.UI.Dialogs
{
    public partial class KirisDialog : Window
    {
        public KirisElement? Sonuc { get; private set; }

        public KirisDialog()
        {
            InitializeComponent();
        }

        private void BtnEkle_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtBw.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double bw) || bw <= 0 ||
                !double.TryParse(txtH.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double h) || h <= 0 ||
                !double.TryParse(txtUzunluk.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double uzunluk) || uzunluk <= 0)
            {
                MessageBox.Show("bw, h ve Uzunluk değerlerini doğru girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double.TryParse(txtDoseme.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double dosemeK);

            Sonuc = new KirisElement
            {
                Id = Guid.NewGuid().ToString(),
                Etiket = string.IsNullOrWhiteSpace(txtEtiket.Text) ? $"Kiriş-{DateTime.Now.Ticks % 1000}" : txtEtiket.Text,
                Kat = txtKat.Text,
                Bw = bw,
                H = h,
                DosemeKalinligi = dosemeK,
                Uzunluk = uzunluk
            };

            DialogResult = true;
            Close();
        }

        private void BtnIptal_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
