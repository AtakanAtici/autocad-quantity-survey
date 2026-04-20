using System.Collections.Generic;
using System.Linq;
using BetonMetraj.Models;

namespace BetonMetraj.Core
{
    /// <summary>
    /// Oturum boyunca eklenen tüm beton elemanlarını tutar.
    /// Singleton pattern.
    /// </summary>
    public sealed class ElementSession
    {
        private static ElementSession? _instance;
        public static ElementSession Instance => _instance ??= new ElementSession();

        private ElementSession() { }

        public List<ConcreteElement> Elemanlar { get; } = new List<ConcreteElement>();

        public void Ekle(ConcreteElement e)
        {
            ConcreteCalculator.Hesapla(e);
            Elemanlar.Add(e);
        }

        public void Sil(ConcreteElement e) => Elemanlar.Remove(e);

        public void Temizle() => Elemanlar.Clear();

        public double ToplamHacim => Elemanlar.Sum(x => x.BetonHacmi);

        public double HacimByTip(ElementType tip) =>
            Elemanlar.Where(x => x.ElemanTipi == tip).Sum(x => x.BetonHacmi);

        public IEnumerable<IGrouping<ElementType, ConcreteElement>> GrupByTip() =>
            Elemanlar.GroupBy(x => x.ElemanTipi);
    }
}
