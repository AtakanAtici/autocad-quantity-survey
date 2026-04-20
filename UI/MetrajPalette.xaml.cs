using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using BetonMetraj.Core;
using BetonMetraj.Models;
using BetonMetraj.UI.Dialogs;
using Microsoft.Win32;

namespace BetonMetraj.UI
{
    public partial class MetrajPalette : UserControl
    {
        private readonly ObservableCollection<ConcreteElement> _elemanlar;
        private ElementSession Session => ElementSession.Instance;

        public MetrajPalette()
        {
            InitializeComponent();
            _elemanlar = new ObservableCollection<ConcreteElement>(Session.Elemanlar);
            lstElemanlar.ItemsSource = _elemanlar;
        }

        private DrawingUnit SecilenBirim =>
            cmbBirim.SelectedIndex switch
            {
                0 => DrawingUnit.Millimeter,
                2 => DrawingUnit.Meter,
                _ => DrawingUnit.Centimeter
            };

        // ─── Eleman Ekleme ───────────────────────────────────────────────

        private void BtnTemelEkle_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new TemelDialog();
            if (dlg.ShowDialog() == true && dlg.Sonuc != null)
                EkleVeGuncelle(dlg.Sonuc);
        }

        private void BtnKolonEkle_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new KolonDialog();
            if (dlg.ShowDialog() == true && dlg.Sonuc != null)
                EkleVeGuncelle(dlg.Sonuc);
        }

        private void BtnKirisEkle_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new KirisDialog();
            if (dlg.ShowDialog() == true && dlg.Sonuc != null)
                EkleVeGuncelle(dlg.Sonuc);
        }

        private void BtnDosemeEkle_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new DosemeDialog();
            if (dlg.ShowDialog() == true && dlg.Sonuc != null)
                EkleVeGuncelle(dlg.Sonuc);
        }

        private void BtnPerdeEkle_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new PerdeDuvarDialog();
            if (dlg.ShowDialog() == true && dlg.Sonuc != null)
                EkleVeGuncelle(dlg.Sonuc);
        }

        private void BtnMerdivenEkle_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new MerdivenDialog();
            if (dlg.ShowDialog() == true && dlg.Sonuc != null)
                EkleVeGuncelle(dlg.Sonuc);
        }

        // ─── AutoCAD Seçimi ──────────────────────────────────────────────

        private void BtnAcadSec_Click(object sender, RoutedEventArgs e)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) { Durum("Açık AutoCAD belgesi bulunamadı."); return; }

            var ed = doc.Editor;
            var opts = new PromptEntityOptions("\nBir çizim nesnesi seçin (Line, Polyline, Circle): ");
            opts.SetRejectMessage("\nGeçersiz nesne.");
            opts.AddAllowedClass(typeof(Line), true);
            opts.AddAllowedClass(typeof(Polyline), true);
            opts.AddAllowedClass(typeof(Circle), true);

            var res = ed.GetEntity(opts);
            if (res.Status != PromptStatus.OK) return;

            using var tr = doc.Database.TransactionManager.StartTransaction();
            var entity = tr.GetObject(res.ObjectId, OpenMode.ForRead) as Entity;
            if (entity == null) { tr.Abort(); return; }

            DrawingUnit birim = SecilenBirim;

            switch (entity)
            {
                case Line line:
                    double uzunluk = AcadGeometryHelper.ConvertToMeters(line.Length, birim);
                    Durum($"Seçilen Line uzunluğu: {uzunluk:F3} m — Kiriş veya Perde olarak ekleyin.");
                    break;

                case Polyline pline when pline.Closed:
                    double alan = AcadGeometryHelper.ConvertToMeters(
                                      AcadGeometryHelper.ConvertToMeters(pline.Area, birim), birim);
                    var (b, h) = AcadGeometryHelper.GetBoundingDimensions(pline);
                    b = AcadGeometryHelper.ConvertToMeters(b, birim);
                    h = AcadGeometryHelper.ConvertToMeters(h, birim);
                    Durum($"Polyline: Alan={alan:F3} m², b={b:F3} m, h={h:F3} m");
                    break;

                case Polyline pline:
                    double pUzunluk = AcadGeometryHelper.ConvertToMeters(pline.Length, birim);
                    Durum($"Açık Polyline uzunluğu: {pUzunluk:F3} m");
                    break;

                case Circle circle:
                    double cap = AcadGeometryHelper.ConvertToMeters(circle.Radius * 2.0, birim);
                    Durum($"Daire çapı: {cap:F3} m");
                    break;
            }

            tr.Commit();
        }

        private void BtnAcadPolylineSec_Click(object sender, RoutedEventArgs e)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) { Durum("Açık AutoCAD belgesi bulunamadı."); return; }

            var ed = doc.Editor;
            var opts = new PromptEntityOptions("\nDöşeme alanı için kapalı Polyline seçin: ");
            opts.SetRejectMessage("\nYalnızca kapalı Polyline seçebilirsiniz.");
            opts.AddAllowedClass(typeof(Polyline), true);

            var res = ed.GetEntity(opts);
            if (res.Status != PromptStatus.OK) return;

            using var tr = doc.Database.TransactionManager.StartTransaction();
            var pline = tr.GetObject(res.ObjectId, OpenMode.ForRead) as Polyline;
            if (pline == null || !pline.Closed)
            {
                Durum("Kapalı bir Polyline seçilmedi.");
                tr.Abort();
                return;
            }

            DrawingUnit birim = SecilenBirim;
            double rawAlan = pline.Area;
            double alan = birim switch
            {
                DrawingUnit.Millimeter => rawAlan / 1_000_000.0,
                DrawingUnit.Centimeter => rawAlan / 10_000.0,
                _ => rawAlan
            };

            tr.Commit();

            var dlg = new DosemeDialog();
            dlg.txtAlan.Text = alan.ToString("F3");
            if (dlg.ShowDialog() == true && dlg.Sonuc != null)
            {
                dlg.Sonuc.AcadHandle = res.ObjectId.Handle.ToString();
                EkleVeGuncelle(dlg.Sonuc);
                Durum($"Döşeme eklendi. Alan: {alan:F3} m²");
            }
        }

        // ─── Liste İşlemleri ─────────────────────────────────────────────

        private void BtnSil_Click(object sender, RoutedEventArgs e)
        {
            var secili = new System.Collections.Generic.List<ConcreteElement>();
            foreach (ConcreteElement item in lstElemanlar.SelectedItems)
                secili.Add(item);

            foreach (var item in secili)
            {
                Session.Sil(item);
                _elemanlar.Remove(item);
            }
            OzetGuncelle();
        }

        private void BtnTemizle_Click(object sender, RoutedEventArgs e)
        {
            var r = MessageBox.Show("Tüm elemanlar silinsin mi?", "Onay",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                Session.Temizle();
                _elemanlar.Clear();
                OzetGuncelle();
                Durum("Liste temizlendi.");
            }
        }

        // ─── Rapor ───────────────────────────────────────────────────────

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            if (Session.Elemanlar.Count == 0) { Durum("Listeye eleman ekleyin."); return; }

            var dlg = new SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = "BetonMetraj.xlsx"
            };
            if (dlg.ShowDialog() == true)
            {
                ReportGenerator.ExportToExcel(Session.Elemanlar, dlg.FileName);
                Durum($"Excel kaydedildi: {Path.GetFileName(dlg.FileName)}");
            }
        }

        private void BtnTxt_Click(object sender, RoutedEventArgs e)
        {
            if (Session.Elemanlar.Count == 0) { Durum("Listeye eleman ekleyin."); return; }

            var dlg = new SaveFileDialog
            {
                Filter = "Metin Dosyası (*.txt)|*.txt",
                FileName = "BetonMetraj.txt"
            };
            if (dlg.ShowDialog() == true)
            {
                ReportGenerator.ExportToTxt(Session.Elemanlar, dlg.FileName);
                Durum($"TXT kaydedildi: {Path.GetFileName(dlg.FileName)}");
            }
        }

        // ─── Yardımcılar ─────────────────────────────────────────────────

        private void EkleVeGuncelle(ConcreteElement e)
        {
            Session.Ekle(e);
            _elemanlar.Add(e);
            OzetGuncelle();
            Durum($"{e.ElemanTipi} eklendi: {e.BetonHacmi:F3} m³");
        }

        private void OzetGuncelle()
        {
            double tkk = Session.HacimByTip(ElementType.Temel)
                       + Session.HacimByTip(ElementType.Kolon)
                       + Session.HacimByTip(ElementType.Kiris);
            double dpm = Session.HacimByTip(ElementType.Doseme)
                       + Session.HacimByTip(ElementType.PerdeDuvar)
                       + Session.HacimByTip(ElementType.Merdiven);
            lblTemelKolonKiris.Text = $"{tkk:F2}";
            lblDosemePerdeMerdiven.Text = $"{dpm:F2}";
            lblToplam.Text = $"{Session.ToplamHacim:F2} m³";
        }

        private void Durum(string mesaj) => lblStatus.Text = mesaj;
    }
}
