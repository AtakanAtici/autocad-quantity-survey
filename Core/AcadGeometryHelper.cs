using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace BetonMetraj.Core
{
    /// <summary>
    /// AutoCAD nesnelerinden geometrik bilgi çıkarma yardımcıları.
    /// </summary>
    public static class AcadGeometryHelper
    {
        /// <summary>
        /// Seçilen bir Polyline veya Line'ın uzunluğunu döner (çizim birimi, metre kabul edilir).
        /// </summary>
        public static double GetLength(Entity entity)
        {
            switch (entity)
            {
                case Line line:
                    return line.Length;

                case Polyline pline:
                    return pline.Length;

                case Polyline2d pline2d:
                    return pline2d.Length;

                case Arc arc:
                    return arc.Length;

                case Circle circle:
                    return 2.0 * Math.PI * circle.Radius;

                default:
                    return 0.0;
            }
        }

        /// <summary>
        /// Kapalı Polyline'ın alanını döner (m²).
        /// </summary>
        public static double GetArea(Entity entity)
        {
            switch (entity)
            {
                case Polyline pline when pline.Closed:
                    return pline.Area;

                case Circle circle:
                    return Math.PI * circle.Radius * circle.Radius;

                case Hatch hatch:
                    return hatch.Area;

                default:
                    return 0.0;
            }
        }

        /// <summary>
        /// Dikdörtgen Polyline'dan genişlik ve yükseklik çıkarır.
        /// İlk kenar = b, ikinci kenar = h olarak döner.
        /// </summary>
        public static (double b, double h) GetBoundingDimensions(Polyline pline)
        {
            if (pline.NumberOfVertices < 3)
                return (0, 0);

            Point3d p0 = pline.GetPoint3dAt(0);
            Point3d p1 = pline.GetPoint3dAt(1);
            Point3d p2 = pline.GetPoint3dAt(2);

            double edge1 = p0.DistanceTo(p1);
            double edge2 = p1.DistanceTo(p2);

            double b = Math.Min(edge1, edge2);
            double h = Math.Max(edge1, edge2);
            return (b, h);
        }

        /// <summary>
        /// Circle entity'den çap değerini döner (m).
        /// </summary>
        public static double GetDiameter(Circle circle) => circle.Radius * 2.0;

        /// <summary>
        /// İki Line entity arasındaki mesafeyi hesaplar (yükseklik için).
        /// </summary>
        public static double GetHeightBetweenLines(Line bottom, Line top)
        {
            return Math.Abs(top.StartPoint.Z - bottom.StartPoint.Z);
        }

        /// <summary>
        /// AutoCAD çizim birimi katsayısı uygular.
        /// Eğer çizim mm'de yapılmışsa → /1000, cm'de → /100, m'de → /1
        /// </summary>
        public static double ConvertToMeters(double value, DrawingUnit unit)
        {
            switch (unit)
            {
                case DrawingUnit.Millimeter: return value / 1000.0;
                case DrawingUnit.Centimeter: return value / 100.0;
                case DrawingUnit.Meter:      return value;
                default:                     return value / 100.0; // cm varsayılan
            }
        }
    }

    public enum DrawingUnit
    {
        Millimeter,
        Centimeter,
        Meter
    }
}
