using System;
using System.Windows;
using BetonMetraj.Models;

namespace BetonMetraj.UI.Dialogs
{
    public partial class PerdeDuvarDialog : Window
    {
        public PerdeDuvarElement? Sonuc { get; private set; }

        public PerdeDuvarDialog()
        {
            InitializeComponent();
        }

        private void BtnEkle_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtUzunluk.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double uzunluk) || uzunluk <= 0 ||
                !double.TryParse(txtYukseklik.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double yukseklik) || yukseklik <= 0 ||
                !double.TryParse(txtKalinlik.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double kalinlik) || kalinlik <= 0)
            {
                MessageBox.Show("Tüm boyutları doğru girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Sonuc = new PerdeDuvarElement
            {
                Id = Guid.NewGuid().ToString(),
                Etiket = string.IsNullOrWhiteSpace(txtEtiket.Text) ? $"Perde-{DateTime.Now.Ticks % 1000}" : txtEtiket.Text,
                Kat = txtKat.Text,
                Uzunluk = uzunluk,
                Yukseklik = yukseklik,
                Kalinlik = kalinlik
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
