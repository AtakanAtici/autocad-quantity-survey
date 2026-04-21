using System;
using System.Windows;
using BetonMetraj.Models;

namespace BetonMetraj.UI.Dialogs
{
    public partial class BlokDialog : Window
    {
        private double _alan;
        public ConcreteElement? Sonuc { get; private set; }

        public BlokDialog(double alan, string blokBilgisi)
        {
            InitializeComponent();
            _alan = alan;
            lblIcerik.Text = blokBilgisi;
            lblAlan.Text = $"{alan:F3} m²";
            txtBlokAdi.Text = $"Blok_{DateTime.Now:HHmmss}";
            Hesapla();
            txtKalınlik.TextChanged += (s, e) => Hesapla();
        }

        private void Hesapla()
        {
            if (double.TryParse(txtKalınlik.Text.Replace(",", "."), out double kalinlik))
            {
                double hacim = _alan * kalinlik;
                lblHacim.Text = $"{hacim:F3} m³";
            }
        }

        private void BtnEkle_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtKalınlik.Text.Replace(",", "."), out double kalinlik) || kalinlik <= 0)
            {
                MessageBox.Show("Geçerli bir kalınlık girin.");
                return;
            }

            double hacim = _alan * kalinlik;

            Sonuc = new ConcreteElement
            {
                ElemanTipi = ElementType.Blok,
                Ad = txtBlokAdi.Text,
                BetonHacmi = hacim,
                Aciklama = $"Alan: {_alan:F2}m², Kalınlık: {kalinlik:F2}m"
            };

            DialogResult = true;
        }
    }
}
