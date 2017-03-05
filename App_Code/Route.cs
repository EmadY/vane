using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
/// <summary>
/// The Route is a set of points that gives a rough example of how distance will be done.
/// It is usually displayed for only a subsection of the FullRoute, and is not updated in the mean time.
/// </summary>
public class Route
{
    public List<Coord> coords;
    public double avg_speed;
    public double total_dist = 0;
    public Route(List<Coord> cds, double td)
    {
        avg_speed = 10;
        coords = cds;
        total_dist = td;
    }
    public Route()
    {
        // as in empty route.
        coords = new List<Coord>();
    }
}
