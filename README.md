# BetonMetraj — AutoCAD C# Plugin

AutoCAD şantiye çizimlerinde **beton metraj hesabı** yapan bir AutoCAD .NET Plugin'i.

## Desteklenen Eleman Tipleri

| Eleman | Tip Seçenekleri | Hesap Formülü |
|--------|----------------|---------------|
| **Temel** | Tekli, Sürekli, Radye, Kaset | V = A × B × H |
| **Kolon** | Dikdörtgen, Daire, L, T | V = Kesit Alanı × Boy |
| **Kiriş** | — | V = bw × (h − döşeme kalınlığı) × L |
| **Döşeme** | Düz, Nervurlu, Asma, Hollow | V = Alan × Kalınlık |
| **Perde Duvar** | — | V = Uzunluk × Yükseklik × Kalınlık |
| **Merdiven** | — | V = Eğimli Plak + Basamak Üçgenleri |

## Gereksinimler

- **AutoCAD 2020+** (Windows)
- **.NET Framework 4.8**
- **Visual Studio 2019+**
- **ClosedXML** (NuGet — Excel raporu için)

## Kurulum

### 1. Projeyi Derleyin

AutoCAD'in kurulu olduğu dizini proje ayarlarına ekleyin:

```xml
<!-- BetonMetraj.csproj içindeki referansları güncelleyin -->
<HintPath>C:\Program Files\Autodesk\AutoCAD 2024\AcCoreMgd.dll</HintPath>
```

Ya da NuGet'ten AutoCAD paketini yükleyin (`.csproj` içindeki yorum satırını etkinleştirin):

```xml
<PackageReference Include="Autodesk.AutoCAD.All" Version="24.0.0" />
```

Ardından:

```bash
dotnet build -c Release
```

### 2. AutoCAD'e Yükleyin

AutoCAD komut satırından:

```
NETLOAD
```

Açılan diyalogdan `BetonMetraj.dll` dosyasını seçin.

**Kalıcı yükleme için** AutoCAD'in `startup.lsp` veya `acad.lsp` dosyasına ekleyin:

```lisp
(command "NETLOAD" "C:/Yol/BetonMetraj.dll")
```

### 3. Kullanım

| Komut | Açıklama |
|-------|----------|
| `BETMETRAJ` | Ana panel'i açar |
| `BETTEMIZLE` | Oturum listesini sıfırlar |
| `BETEXCEL` | Excel raporu oluşturur |

## Panel Kullanımı

1. `BETMETRAJ` komutunu çalıştırın → panel açılır
2. **Çizim Birimi** seçin (mm / cm / m)
3. Eleman butonlarından birini tıklayın → boyut giriş diyaloğu açılır
4. Boyutları girin → **Ekle** tıklayın
5. İsterseniz **Çizimden Seç** butonu ile AutoCAD nesnesini seçerek uzunluk/alan otomatik okunur
6. Tüm elemanlar eklendikten sonra **Excel** veya **TXT** raporu alın

## Formüller Hakkında

### Kiriş
Kirişin döşemeyle iç içe geçen kısmı iki kez sayılmaması için `DöşemeKalınlığı` parametresi girilir:

```
V_kiriş = bw × (h_toplam - h_döşeme) × L
```

### Merdiven
Eğimli plak hacmi + basamak üçgenlerinin toplamı:

```
V_plak = Genişlik × √(n×g² + H_kat²) × t
V_basamaklar = n × 0.5 × h_bas × g_bas × Genişlik
```

## Klasör Yapısı

```
BetonMetraj/
├── Models/
│   ├── ElementType.cs          # Enum tanımları
│   └── ConcreteElement.cs      # Eleman model sınıfları
├── Core/
│   ├── ConcreteCalculator.cs   # Hacim hesaplama motoru
│   ├── ElementSession.cs       # Oturum yönetimi
│   ├── AcadGeometryHelper.cs   # AutoCAD geometri okuma
│   └── ReportGenerator.cs      # CSV / TXT / Excel rapor üretici
├── UI/
│   ├── MetrajPalette.xaml      # Ana WPF panel
│   ├── MetrajPalette.xaml.cs
│   └── Dialogs/
│       ├── TemelDialog.xaml(+cs)
│       ├── KolonDialog.xaml(+cs)
│       ├── KirisDialog.xaml(+cs)
│       ├── DosemeDialog.xaml(+cs)
│       ├── PerdeDuvarDialog.xaml(+cs)
│       └── MerdivenDialog.xaml(+cs)
├── Commands.cs                 # AutoCAD komut tanımları
└── BetonMetraj.csproj
```
