using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BetonMetraj.Models;
using ClosedXML.Excel;

namespace BetonMetraj.Core
{
    /// <summary>
    /// CSV, TXT ve Excel formatlarında beton metraj raporu üretir.
    /// </summary>
    public static class ReportGenerator
    {
        public static void ExportToCsv(IEnumerable<ConcreteElement> elemanlar, string dosyaYolu)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Eleman Tipi;Etiket;Kat;Beton Hacmi (m³);Not");

            foreach (var e in elemanlar)
            {
                sb.AppendLine($"{e.ElemanTipi};{e.Etiket};{e.Kat};{e.BetonHacmi:F4};{e.Not}");
            }

            double toplam = elemanlar.Sum(x => x.BetonHacmi);
            sb.AppendLine($";;TOPLAM;{toplam:F4};");

            File.WriteAllText(dosyaYolu, sb.ToString(), Encoding.UTF8);
        }

        public static void ExportToTxt(IEnumerable<ConcreteElement> elemanlar, string dosyaYolu)
        {
            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("       BETON METRAJ RAPORU");
            sb.AppendLine($"       {DateTime.Now:dd.MM.yyyy HH:mm}");
            sb.AppendLine("========================================");
            sb.AppendLine();

            var gruplar = elemanlar.GroupBy(x => x.ElemanTipi);

            foreach (var grup in gruplar)
            {
                sb.AppendLine($"--- {grup.Key} ---");
                sb.AppendLine($"{"Etiket",-20} {"Kat",-10} {"Hacim (m³)",12}");
                sb.AppendLine(new string('-', 44));

                foreach (var e in grup)
                {
                    sb.AppendLine($"{e.Etiket,-20} {e.Kat,-10} {e.BetonHacmi,12:F3}");
                }

                double grupToplam = grup.Sum(x => x.BetonHacmi);
                sb.AppendLine(new string('-', 44));
                sb.AppendLine($"{"Alt Toplam:",-30} {grupToplam,12:F3} m³");
                sb.AppendLine();
            }

            double genelToplam = elemanlar.Sum(x => x.BetonHacmi);
            sb.AppendLine("========================================");
            sb.AppendLine($"{"GENEL TOPLAM:",-30} {genelToplam,12:F3} m³");
            sb.AppendLine("========================================");

            File.WriteAllText(dosyaYolu, sb.ToString(), Encoding.UTF8);
        }

        public static void ExportToExcel(IEnumerable<ConcreteElement> elemanlar, string dosyaYolu)
        {
            var liste = elemanlar.ToList();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Beton Metrajı");

            // Başlık
            ws.Cell(1, 1).Value = "BETON METRAJ RAPORU";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Range(1, 1, 1, 6).Merge();

            ws.Cell(2, 1).Value = $"Tarih: {DateTime.Now:dd.MM.yyyy HH:mm}";
            ws.Range(2, 1, 2, 6).Merge();

            // Sütun başlıkları
            int baslikSatir = 4;
            ws.Cell(baslikSatir, 1).Value = "Sıra";
            ws.Cell(baslikSatir, 2).Value = "Eleman Tipi";
            ws.Cell(baslikSatir, 3).Value = "Etiket";
            ws.Cell(baslikSatir, 4).Value = "Kat";
            ws.Cell(baslikSatir, 5).Value = "Beton Hacmi (m³)";
            ws.Cell(baslikSatir, 6).Value = "Not";

            var baslikRange = ws.Range(baslikSatir, 1, baslikSatir, 6);
            baslikRange.Style.Font.Bold = true;
            baslikRange.Style.Fill.BackgroundColor = XLColor.DarkBlue;
            baslikRange.Style.Font.FontColor = XLColor.White;
            baslikRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Veri satırları
            int satir = baslikSatir + 1;
            int sira = 1;
            var grupRenkleri = new Dictionary<ElementType, XLColor>
            {
                { ElementType.Temel,      XLColor.LightYellow },
                { ElementType.Kolon,      XLColor.LightBlue },
                { ElementType.Kiris,      XLColor.LightGreen },
                { ElementType.Doseme,     XLColor.LightPink },
                { ElementType.PerdeDuvar, XLColor.LightCoral },
                { ElementType.Merdiven,   XLColor.Lavender }
            };

            foreach (var grup in liste.GroupBy(x => x.ElemanTipi))
            {
                XLColor renk = grupRenkleri.TryGetValue(grup.Key, out var r) ? r : XLColor.White;

                foreach (var e in grup)
                {
                    ws.Cell(satir, 1).Value = sira++;
                    ws.Cell(satir, 2).Value = e.ElemanTipi.ToString();
                    ws.Cell(satir, 3).Value = e.Etiket;
                    ws.Cell(satir, 4).Value = e.Kat;
                    ws.Cell(satir, 5).Value = e.BetonHacmi;
                    ws.Cell(satir, 5).Style.NumberFormat.Format = "0.000";
                    ws.Cell(satir, 6).Value = e.Not;

                    ws.Range(satir, 1, satir, 6).Style.Fill.BackgroundColor = renk;
                    satir++;
                }

                // Grup toplamı
                double grupToplam = grup.Sum(x => x.BetonHacmi);
                ws.Cell(satir, 4).Value = $"{grup.Key} Toplamı:";
                ws.Cell(satir, 4).Style.Font.Bold = true;
                ws.Cell(satir, 5).Value = grupToplam;
                ws.Cell(satir, 5).Style.NumberFormat.Format = "0.000";
                ws.Cell(satir, 5).Style.Font.Bold = true;
                satir++;
            }

            // Genel toplam
            satir++;
            ws.Cell(satir, 4).Value = "GENEL TOPLAM:";
            ws.Cell(satir, 4).Style.Font.Bold = true;
            ws.Cell(satir, 4).Style.Font.FontSize = 12;
            ws.Cell(satir, 5).Value = liste.Sum(x => x.BetonHacmi);
            ws.Cell(satir, 5).Style.NumberFormat.Format = "0.000";
            ws.Cell(satir, 5).Style.Font.Bold = true;
            ws.Cell(satir, 5).Style.Font.FontSize = 12;
            ws.Cell(satir, 5).Style.Fill.BackgroundColor = XLColor.Orange;

            ws.Columns().AdjustToContents();
            wb.SaveAs(dosyaYolu);
        }
    }
}
