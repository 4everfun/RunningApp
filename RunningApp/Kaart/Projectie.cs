/*
Deze klasse bevat conversiemethodes tussen
 - geografische coordinaten (op de gebruikelijke WGS84 ellipsoide)
 - coordinaten in de RD (Rijksdriehoekmeting)-projectie van de Topografische Dienst Nederland/Het Kadaster

De methodes zijn gebaseerd op de beschrijving in:
    F.H. Schreutelkamp en G.L. Strang van Hees:
    "Benaderingsformules voor de transformatie tussen RD- en WGS84-kaartcoordinaten"
    in _Geodesia_  *43* (2001), pp. 64-69.
*/

using Android.Graphics;   // vanwege PointF
using Android.Locations;  // vanwege Location

namespace Kaart
{
    public class Projectie
    {
        private const double fi0 = 52.15517440;
        private const double lam0 = 5.38720621;
        private const double x0 = 155000.00;
        private const double y0 = 463000.00;

        // Conversie van RD naar Geografisch
        // Parameter rd bevat X- en Y-coordinaat in RD-projectie
        // Resultaat bevat de latitude (breedtegraad, bijvoorbeeld 52 graden Noorderbreedte)
        //              en de longitude (lengtegraad, bijvoorbeeld 5 graden Oosterlengte)

        public static Location RD2Geo(PointF rd)
        {
            double x = (rd.X - x0) * 1E-5;
            double y = (rd.Y - y0) * 1E-5;

            double x2 = x * x;
            double x3 = x2 * x;
            double x4 = x3 * x;
            double x5 = x4 * x;
            double y2 = y * y;
            double y3 = y2 * y;
            double y4 = y3 * y;

            double fi = fi0 +
                      (3235.65389 * y
                      - 32.58297 * x2
                      - 0.24750 * y2
                      - 0.84978 * x2 * y
                      - 0.06550 * y3
                      - 0.01709 * x2 * y2
                      - 0.00738 * x
                      + 0.00530 * x4
                      - 0.00039 * x2 * y3
                      + 0.00033 * x4 * y
                      - 0.00012 * x * y
                      ) / 3600;

            double lam = lam0 +
                       (5260.52916 * x
                       + 105.94684 * x * y
                       + 2.45656 * x * y2
                       - 0.81885 * x3
                       + 0.05594 * x * y3
                       - 0.05607 * x3 * y
                       + 0.01199 * y
                       - 0.00256 * x3 * y2
                       + 0.00128 * x * y4
                       + 0.00022 * y2
                       - 0.00022 * x2
                       + 0.00026 * x5
                       ) / 3600;

            Location loc = new Location("");
            loc.Latitude = fi;
            loc.Longitude = lam;
            return loc;
        }


        // Conversie van Geografisch naar RD
        // Parameter geo bevat de latitude (breedtegraad, bijvoorbeeld 52 graden Noorderbreedte)
        //                  en de longitude (lengtegraad, bijvoorbeeld 5 graden Oosterlengte)
        // Resultaat bevat X- en Y-coordinaat in RD-projectie

        public static PointF Geo2RD(Location geo)
        {
            double fi = geo.Latitude;
            double lam = geo.Longitude;

            double dFi = 0.36 * (fi - fi0);
            double dLam = 0.36 * (lam - lam0);

            double dFi2 = dFi * dFi;
            double dFi3 = dFi2 * dFi;
            double dLam2 = dLam * dLam;
            double dLam3 = dLam2 * dLam;
            double dLam4 = dLam3 * dLam;

            double x = x0
                     + 190094.945 * dLam
                     - 11832.228 * dFi * dLam
                     - 114.221 * dFi2 * dLam
                     - 32.391 * dLam3
                     - 0.705 * dFi
                     - 2.340 * dFi3 * dLam
                     - 0.608 * dFi * dLam3
                     - 0.008 * dLam2
                     + 0.148 * dFi2 * dLam3;

            double y = y0
                     + 309056.544 * dFi
                     + 3638.893 * dLam2
                     + 73.077 * dFi2
                     - 157.984 * dFi * dLam2
                     + 59.788 * dFi3
                     + 0.433 * dLam
                     - 6.439 * dFi2 * dLam2
                     - 0.032 * dFi * dLam
                     + 0.092 * dLam4
                     - 0.054 * dFi * dLam4;

            PointF rd = new PointF((float)x, (float)y);
            return rd;
        }
    }
}

