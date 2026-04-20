using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using BetonMetraj.UI;

// AutoCAD'e DLL'i yüklerken bu attribute okunur
[assembly: CommandClass(typeof(BetonMetraj.Commands))]
[assembly: ExtensionApplication(typeof(BetonMetraj.BetonMetrajApp))]

namespace BetonMetraj
{
    /// <summary>
    /// Plugin yüklendiğinde çalışır (Initialize) ve kaldırıldığında (Terminate).
    /// </summary>
    public class BetonMetrajApp : IExtensionApplication
    {
        private static PaletteSet? _paletteSet;

        public void Initialize()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            doc?.Editor.WriteMessage("\n[BetonMetraj] Plugin yüklendi. 'BETMETRAJ' komutunu çalıştırın.\n");
        }

        public void Terminate()
        {
            _paletteSet?.Dispose();
        }

        public static void PaletteGoster()
        {
            if (_paletteSet == null)
            {
                _paletteSet = new PaletteSet("Beton Metrajı")
                {
                    MinimumSize = new System.Drawing.Size(380, 600),
                    Style = PaletteSetStyles.ShowAutoHideButton
                           | PaletteSetStyles.ShowCloseButton
                };

                var palette = new MetrajPalette();
                var host = new System.Windows.Forms.Integration.ElementHost
                {
                    Child = palette,
                    Dock = System.Windows.Forms.DockStyle.Fill
                };

                _paletteSet.Add("Metraj", host);
            }

            _paletteSet.Visible = true;
        }
    }

    /// <summary>
    /// AutoCAD komutları. AutoCAD komut satırından çalıştırılır.
    /// </summary>
    public class Commands
    {
        /// <summary>
        /// Komut: BETMETRAJ
        /// Beton Metraj panelini açar.
        /// </summary>
        [CommandMethod("BETMETRAJ", CommandFlags.Modal)]
        public void BetonMetraj()
        {
            BetonMetrajApp.PaletteGoster();
        }

        /// <summary>
        /// Komut: BETTEMIZLE
        /// Mevcut oturum listesini sıfırlar.
        /// </summary>
        [CommandMethod("BETTEMIZLE", CommandFlags.Modal)]
        public void BetonTemizle()
        {
            Core.ElementSession.Instance.Temizle();
            var doc = Application.DocumentManager.MdiActiveDocument;
            doc?.Editor.WriteMessage("\n[BetonMetraj] Tüm elemanlar temizlendi.\n");
        }

        /// <summary>
        /// Komut: BETEXCEL
        /// Mevcut listeyi Excel'e dışa aktarır (dosya seçim diyaloğu açar).
        /// </summary>
        [CommandMethod("BETEXCEL", CommandFlags.Modal)]
        public void BetonExcel()
        {
            if (Core.ElementSession.Instance.Elemanlar.Count == 0)
            {
                var doc = Application.DocumentManager.MdiActiveDocument;
                doc?.Editor.WriteMessage("\n[BetonMetraj] Listeye eleman ekleyin.\n");
                return;
            }

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = "BetonMetraj.xlsx"
            };

            if (dlg.ShowDialog() == true)
            {
                Core.ReportGenerator.ExportToExcel(Core.ElementSession.Instance.Elemanlar, dlg.FileName);
                Application.DocumentManager.MdiActiveDocument?.Editor
                    .WriteMessage($"\n[BetonMetraj] Excel kaydedildi: {dlg.FileName}\n");
            }
        }
    }
}
