using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Coord class forms the basis of our distance calculations.
/// Contains it's own dist function, call as Coord.dist(Coord1, Coord2); retuns distance in meters.
/// Class is complete.
/// </summary>
public class Coord
{
    public double lat;
    public double lon;
    static public double R = 6373000; // Radius of the Eath in meters
    static public double IGNORE_DIST = 20; // any two points within 40 meters are considered the same point.
    public Coord(double latitude, double longitude)
    {
        lat = latitude; lon = longitude;
    }

    // Returns distance between two points in meters
    public static double dist(Coord p1, Coord p2) {
        double dlon = (p2.lon - p1.lon) * Math.PI/180;
        double dlat = (p2.lat - p1.lat) * Math.PI/180;

        double a = Math.Pow(Math.Sin(dlat/2), 2) + Math.Cos(p1.lat) * Math.Cos(p2.lat) * Math.Pow(Math.Sin(dlon/2), 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));

        return R * c;
    }
}
