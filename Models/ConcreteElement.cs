using System.Collections.Generic;

namespace BetonMetraj.Models
{
    /// <summary>
    /// Bir beton yapı elemanını temsil eder.
    /// </summary>
    public class ConcreteElement
    {
        public string Id { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public string Etiket { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public ElementType ElemanTipi { get; set; }

        /// <summary>AutoCAD Layer adı</summary>
        public string Layer { get; set; } = string.Empty;

        /// <summary>AutoCAD Handle (eleman kimliği)</summary>
        public string AcadHandle { get; set; } = string.Empty;

        /// <summary>Hesaplanan beton hacmi (m³)</summary>
        public double BetonHacmi { get; set; }

        /// <summary>Formüle özgü boyutlar</summary>
        public Dictionary<string, double> Boyutlar { get; set; } = new Dictionary<string, double>();

        /// <summary>Kat/Konum bilgisi</summary>
        public string Kat { get; set; } = string.Empty;

        /// <summary>Kullanıcı notu</summary>
        public string Not { get; set; } = string.Empty;

        public override string ToString()
        {
            string adGoster = !string.IsNullOrEmpty(Ad) ? Ad : Etiket;
            return $"{ElemanTipi} - {adGoster} [{BetonHacmi:F3} m³]";
        }
    }

    public class TemelElement : ConcreteElement
    {
        public TemelTipi TemelTipi { get; set; }

        /// <summary>Genişlik (m)</summary>
        public double A { get; set; }

        /// <summary>Uzunluk (m)</summary>
        public double B { get; set; }

        /// <summary>Yükseklik/Kalınlık (m)</summary>
        public double H { get; set; }

        /// <summary>Radye/Sürekli için çevre uzunluğu (m) - opsiyonel</summary>
        public double Uzunluk { get; set; }

        public TemelElement()
        {
            ElemanTipi = ElementType.Temel;
        }
    }

    public class KolonElement : ConcreteElement
    {
        public KolonKesiti KesitTipi { get; set; }

        /// <summary>Genişlik b (m)</summary>
        public double B { get; set; }

        /// <summary>Derinlik h (m)</summary>
        public double H { get; set; }

        /// <summary>Daire kesit için çap (m)</summary>
        public double Cap { get; set; }

        /// <summary>Kolon boyu (m)</summary>
        public double Boy { get; set; }

        public KolonElement()
        {
            ElemanTipi = ElementType.Kolon;
        }
    }

    public class KirisElement : ConcreteElement
    {
        /// <summary>Genişlik bw (m)</summary>
        public double Bw { get; set; }

        /// <summary>Toplam yükseklik h (m)</summary>
        public double H { get; set; }

        /// <summary>Döşeme ile örtüşen kısım yüksekliği (m) - çift sayım önleme</summary>
        public double DosemeKalinligi { get; set; }

        /// <summary>Açıklık / uzunluk (m)</summary>
        public double Uzunluk { get; set; }

        public KirisElement()
        {
            ElemanTipi = ElementType.Kiris;
        }
    }

    public class DosemeElement : ConcreteElement
    {
        public DosemeTipi DosemeTipi { get; set; }

        /// <summary>Alan (m²)</summary>
        public double Alan { get; set; }

        /// <summary>Döşeme kalınlığı (m)</summary>
        public double Kalinlik { get; set; }

        /// <summary>Nervurlu için nervur aralığı (m)</summary>
        public double NervurAraligi { get; set; }

        /// <summary>Nervur genişliği (m)</summary>
        public double NervurGenisligi { get; set; }

        public DosemeElement()
        {
            ElemanTipi = ElementType.Doseme;
        }
    }

    public class PerdeDuvarElement : ConcreteElement
    {
        /// <summary>Perde uzunluğu (m)</summary>
        public double Uzunluk { get; set; }

        /// <summary>Perde yüksekliği (m)</summary>
        public double Yukseklik { get; set; }

        /// <summary>Perde kalınlığı (m)</summary>
        public double Kalinlik { get; set; }

        public PerdeDuvarElement()
        {
            ElemanTipi = ElementType.PerdeDuvar;
        }
    }

    public class MerdivenElement : ConcreteElement
    {
        /// <summary>Merdiven genişliği (m)</summary>
        public double Genislik { get; set; }

        /// <summary>Merdiven uzunluğu / kat yüksekliği (m)</summary>
        public double KatYuksekligi { get; set; }

        /// <summary>Plak kalınlığı (m)</summary>
        public double PlakKalinligi { get; set; }

        /// <summary>Basamak sayısı</summary>
        public int BasamakSayisi { get; set; }

        /// <summary>Basamak yüksekliği (m)</summary>
        public double BasamakYuksekligi { get; set; }

        /// <summary>Basamak genişliği / rıht (m)</summary>
        public double BasamakGenisligi { get; set; }

        public MerdivenElement()
        {
            ElemanTipi = ElementType.Merdiven;
        }
    }
}
