namespace BetonMetraj.Models
{
    public enum ElementType
    {
        Temel,
        Kolon,
        Kiris,
        Doseme,
        PerdeDuvar,
        Merdiven,
        Blok
    }

    public enum TemelTipi
    {
        TekliTemel,
        SurekliTemel,
        RadyeTemel,
        KazetteTemel
    }

    public enum KolonKesiti
    {
        Dikdortgen,
        Daire,
        L_Sekli,
        T_Sekli
    }

    public enum DosemeTipi
    {
        DuzDoseme,
        Nervurlu,
        Asma,
        Hollow
    }
}
