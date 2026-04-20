using System;
using BetonMetraj.Models;

namespace BetonMetraj.Core
{
    /// <summary>
    /// Her eleman tipi için beton hacmi hesaplama motoru.
    /// Tüm ölçüler metre (m), hacimler metreküp (m³) cinsindendir.
    /// </summary>
    public static class ConcreteCalculator
    {
        public static double HesaplaTemel(TemelElement e)
        {
            switch (e.TemelTipi)
            {
                case TemelTipi.TekliTemel:
                    // V = A × B × H
                    return e.A * e.B * e.H;

                case TemelTipi.SurekliTemel:
                    // V = A × H × Uzunluk
                    return e.A * e.H * e.Uzunluk;

                case TemelTipi.RadyeTemel:
                    // V = Alan × H  (A=genişlik, B=uzunluk)
                    return e.A * e.B * e.H;

                case TemelTipi.KazetteTemel:
                    // Kaset temel: tam plak - boşluklar (yaklaşık %70 doluluk)
                    return e.A * e.B * e.H * 0.70;

                default:
                    return e.A * e.B * e.H;
            }
        }

        public static double HesaplaKolon(KolonElement e)
        {
            double kesitAlani;
            switch (e.KesitTipi)
            {
                case KolonKesiti.Daire:
                    kesitAlani = Math.PI * (e.Cap / 2.0) * (e.Cap / 2.0);
                    break;

                case KolonKesiti.L_Sekli:
                    // L kesiti: dikdörtgen - çıkıntı (yaklaşık)
                    // B × H - (B/2 × H/2)
                    kesitAlani = (e.B * e.H) - (e.B / 2.0 * e.H / 2.0);
                    break;

                case KolonKesiti.T_Sekli:
                    // T kesiti: dikdörtgen - iki köşe
                    kesitAlani = (e.B * e.H) - 2.0 * (e.B / 4.0 * e.H / 4.0);
                    break;

                default: // Dikdortgen
                    kesitAlani = e.B * e.H;
                    break;
            }
            return kesitAlani * e.Boy;
        }

        public static double HesaplaKiris(KirisElement e)
        {
            // Kirişin döşeme altında kalan kısmı hesaplanır (döşeme ile çift sayım önlenir)
            double netYukseklik = e.H - e.DosemeKalinligi;
            if (netYukseklik < 0) netYukseklik = 0;
            return e.Bw * netYukseklik * e.Uzunluk;
        }

        public static double HesaplaDoseme(DosemeElement e)
        {
            switch (e.DosemeTipi)
            {
                case DosemeTipi.Nervurlu:
                    // Nervurlu döşeme: plak + nervurlar
                    // Plak hacmi + nervur hacmi (basitleştirilmiş)
                    double plakHacmi = e.Alan * (e.Kalinlik * 0.4); // plak kısmı
                    double nervurHacmi = e.Alan / e.NervurAraligi
                                        * e.NervurGenisligi
                                        * (e.Kalinlik * 0.6)
                                        * e.NervurAraligi;
                    return plakHacmi + nervurHacmi;

                default: // Düz döşeme / Asma / Hollow
                    return e.Alan * e.Kalinlik;
            }
        }

        public static double HesaplaPerdeDuvar(PerdeDuvarElement e)
        {
            // V = Uzunluk × Yükseklik × Kalınlık
            return e.Uzunluk * e.Yukseklik * e.Kalinlik;
        }

        public static double HesaplaMerdiven(MerdivenElement e)
        {
            // Eğimli plak hacmi
            double egimUzunluk = Math.Sqrt(
                Math.Pow(e.BasamakSayisi * e.BasamakGenisligi, 2) +
                Math.Pow(e.KatYuksekligi, 2));

            double plakHacmi = e.Genislik * egimUzunluk * e.PlakKalinligi;

            // Basamak üçgenleri hacmi
            double basamakHacmi = e.BasamakSayisi
                                  * 0.5
                                  * e.BasamakYuksekligi
                                  * e.BasamakGenisligi
                                  * e.Genislik;

            return plakHacmi + basamakHacmi;
        }

        /// <summary>
        /// Verilen elemana göre uygun hesaplama metodunu çağırır ve hacmi atar.
        /// </summary>
        public static void Hesapla(ConcreteElement eleman)
        {
            switch (eleman)
            {
                case TemelElement t:
                    t.BetonHacmi = HesaplaTemel(t);
                    break;
                case KolonElement k:
                    k.BetonHacmi = HesaplaKolon(k);
                    break;
                case KirisElement ki:
                    ki.BetonHacmi = HesaplaKiris(ki);
                    break;
                case DosemeElement d:
                    d.BetonHacmi = HesaplaDoseme(d);
                    break;
                case PerdeDuvarElement p:
                    p.BetonHacmi = HesaplaPerdeDuvar(p);
                    break;
                case MerdivenElement m:
                    m.BetonHacmi = HesaplaMerdiven(m);
                    break;
            }
        }
    }
}
