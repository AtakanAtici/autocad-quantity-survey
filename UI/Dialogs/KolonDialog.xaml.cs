using System;
using System.Windows;
using BetonMetraj.Models;

namespace BetonMetraj.UI.Dialogs
{
    public partial class KolonDialog : Window
    {
        public KolonElement? Sonuc { get; private set; }

        public KolonDialog()
        {
            InitializeComponent();
        }

        private void cmbKesit_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (capPanel == null) return;

            bool daireMi = cmbKesit.SelectedIndex == 1;
            capPanel.Visibility = daireMi ? Visibility.Visible : Visibility.Collapsed;
            txtB.IsEnabled = !daireMi;
            txtH.IsEnabled = !daireMi;

            if (daireMi)
            {
                lblB.Content = "b - (Kullanılmaz)";
                lblH.Content = "h - (Kullanılmaz)";
            }
            else
            {
                lblB.Content = "b - Genişlik (m):";
                lblH.Content = "h - Derinlik (m):";
            }
        }

        private void BtnEkle_Click(object sender, RoutedEventArgs e)
        {
            KolonKesiti kesit = (KolonKesiti)cmbKesit.SelectedIndex;

            if (!double.TryParse(txtBoy.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double boy) || boy <= 0)
            {
                MessageBox.Show("Geçerli bir Boy değeri girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double b = 0, h = 0, cap = 0;

            if (kesit == KolonKesiti.Daire)
            {
                if (!double.TryParse(txtCap.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out cap) || cap <= 0)
                {
                    MessageBox.Show("Geçerli bir Çap değeri girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                if (!double.TryParse(txtB.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out b) || b <= 0 ||
                    !double.TryParse(txtH.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out h) || h <= 0)
                {
                    MessageBox.Show("Geçerli b ve h değerleri girin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            Sonuc = new KolonElement
            {
                Id = Guid.NewGuid().ToString(),
                Etiket = string.IsNullOrWhiteSpace(txtEtiket.Text) ? $"Kolon-{DateTime.Now.Ticks % 1000}" : txtEtiket.Text,
                Kat = txtKat.Text,
                KesitTipi = kesit,
                B = b,
                H = h,
                Cap = cap,
                Boy = boy
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
