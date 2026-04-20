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
            var inv = System.Globalization.CultureInfo.InvariantCulture;
            var any = System.Globalization.NumberStyles.Any;

            bool ok1 = double.TryParse(txtGenislik.Text.Replace(",", "."), any, inv, out double genislik) && genislik > 0;
            bool ok2 = double.TryParse(txtKatYuksekligi.Text.Replace(",", "."), any, inv, out double katY) && katY > 0;
            bool ok3 = double.TryParse(txtPlakKalinligi.Text.Replace(",", "."), any, inv, out double plak) && plak > 0;
            bool ok4 = int.TryParse(txtBasamakSayisi.Text, out int sayi) && sayi > 0;
            bool ok5 = double.TryParse(txtBasamakYuksekligi.Text.Replace(",", "."), any, inv, out double basY) && basY > 0;
            bool ok6 = double.TryParse(txtBasamakGenisligi.Text.Replace(",", "."), any, inv, out double basG) && basG > 0;

            if (!ok1 || !ok2 || !ok3 || !ok4 || !ok5 || !ok6)
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
