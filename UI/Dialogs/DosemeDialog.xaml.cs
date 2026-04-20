using System;
using System.Windows;
using BetonMetraj.Models;

namespace BetonMetraj.UI.Dialogs
{
    public partial class DosemeDialog : Window
    {
        public DosemeElement? Sonuc { get; private set; }

        public DosemeDialog()
        {
            InitializeComponent();
        }

        private void cmbTip_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (nervurPanel == null) return;
            nervurPanel.Visibility = cmbTip.SelectedIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnEkle_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtAlan.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double alan) || alan <= 0 ||
                !double.TryParse(txtKalinlik.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double kalinlik) || kalinlik <= 0)
            {
                MessageBox.Show("Alan ve Kalınlık değerlerini doğru girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DosemeTipi tip = (DosemeTipi)cmbTip.SelectedIndex;
            double nervurAraligi = 0, nervurGenisligi = 0;

            if (tip == DosemeTipi.Nervurlu)
            {
                if (!double.TryParse(txtNervurAraligi.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out nervurAraligi) || nervurAraligi <= 0 ||
                    !double.TryParse(txtNervurGenisligi.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out nervurGenisligi) || nervurGenisligi <= 0)
                {
                    MessageBox.Show("Nervur aralığı ve genişliğini girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            Sonuc = new DosemeElement
            {
                Id = Guid.NewGuid().ToString(),
                Etiket = string.IsNullOrWhiteSpace(txtEtiket.Text) ? $"Döşeme-{DateTime.Now.Ticks % 1000}" : txtEtiket.Text,
                Kat = txtKat.Text,
                DosemeTipi = tip,
                Alan = alan,
                Kalinlik = kalinlik,
                NervurAraligi = nervurAraligi,
                NervurGenisligi = nervurGenisligi
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
