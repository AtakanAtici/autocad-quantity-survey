using System;
using System.Windows;
using BetonMetraj.Models;

namespace BetonMetraj.UI.Dialogs
{
    public partial class TemelDialog : Window
    {
        public TemelElement? Sonuc { get; private set; }

        public TemelDialog()
        {
            InitializeComponent();
        }

        private void cmbTemelTipi_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (uzunlukPanel == null) return;

            bool surekliMi = cmbTemelTipi.SelectedIndex == 1;
            uzunlukPanel.Visibility = surekliMi ? Visibility.Visible : Visibility.Collapsed;

            if (surekliMi)
            {
                lblA.Content = "A - Genişlik (m):";
                lblB.Content = "B - (Kullanılmaz)";
                txtB.IsEnabled = false;
            }
            else
            {
                lblA.Content = "A - Genişlik (m):";
                lblB.Content = "B - Uzunluk (m):";
                txtB.IsEnabled = true;
            }
        }

        private void BtnEkle_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtA.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double a) || a <= 0)
            {
                MessageBox.Show("Geçerli bir A (Genişlik) değeri girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(txtH.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double h) || h <= 0)
            {
                MessageBox.Show("Geçerli bir H (Yükseklik) değeri girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TemelTipi tip = (TemelTipi)cmbTemelTipi.SelectedIndex;
            double b = 0, uzunluk = 0;

            if (tip == TemelTipi.SurekliTemel)
            {
                if (!double.TryParse(txtUzunluk.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out uzunluk) || uzunluk <= 0)
                {
                    MessageBox.Show("Geçerli bir Uzunluk değeri girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                if (!double.TryParse(txtB.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out b) || b <= 0)
                {
                    MessageBox.Show("Geçerli bir B (Uzunluk) değeri girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            Sonuc = new TemelElement
            {
                Id = Guid.NewGuid().ToString(),
                Etiket = string.IsNullOrWhiteSpace(txtEtiket.Text) ? $"Temel-{DateTime.Now.Ticks % 1000}" : txtEtiket.Text,
                Kat = txtKat.Text,
                TemelTipi = tip,
                A = a,
                B = b,
                H = h,
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
