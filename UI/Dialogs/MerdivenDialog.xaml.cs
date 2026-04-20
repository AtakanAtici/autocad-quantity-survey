using System;
using System.Windows;
using BetonMetraj.Models;

namespace BetonMetraj.UI.Dialogs
{
    public partial class MerdivenDialog : Window
    {
        public MerdivenElement? Sonuc { get; private set; }

        public MerdivenDialog()
        {
            InitializeComponent();
        }

        private void BtnEkle_Click(object sender, RoutedEventArgs e)
        {
            bool valid =
                double.TryParse(txtGenislik.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double genislik) && genislik > 0 &&
                double.TryParse(txtKatYuksekligi.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double katY) && katY > 0 &&
                double.TryParse(txtPlakKalinligi.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double plak) && plak > 0 &&
                int.TryParse(txtBasamakSayisi.Text, out int sayi) && sayi > 0 &&
                double.TryParse(txtBasamakYuksekligi.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double basY) && basY > 0 &&
                double.TryParse(txtBasamakGenisligi.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double basG) && basG > 0;

            if (!valid)
            {
                MessageBox.Show("Tüm değerleri doğru girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Sonuc = new MerdivenElement
            {
                Id = Guid.NewGuid().ToString(),
                Etiket = string.IsNullOrWhiteSpace(txtEtiket.Text) ? $"Merdiven-{DateTime.Now.Ticks % 1000}" : txtEtiket.Text,
                Kat = txtKat.Text,
                Genislik = genislik,
                KatYuksekligi = katY,
                PlakKalinligi = plak,
                BasamakSayisi = sayi,
                BasamakYuksekligi = basY,
                BasamakGenisligi = basG
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
