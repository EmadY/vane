using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;

/// <summary>
/// The Route is a set of points that gives a rough example of how distance will be done.
/// The elements of coords are sorted in a way that makes it reasonable to assume that the path
/// is from element #0 to the last element.
/// This class is now complete.
/// </summary>
public class Route
{
    public List<Coord> coords;
    public List<double> dists;
    public double total_dist = 0;
    public Route(Coord start, Coord end)
    {
        coords = new List<Coord>();
        coords.Add(start);
        coords.Add(end);

        dists = new List<double>();
        dists.Add(Coord.dist(start, end));

        total_dist = dists[0];
    }

    public void add_coord(Coord c){

        // we need to determine where to put the coordinate.
        // Put it in the location that minimizes total route length. it closest to the one before and after it.

        if(coords.Count == 2) {
            // just add it in the middle.

            dists[0] = Coord.dist(coords[0], c);
            dists.Add(Coord.dist(coords[1], c));
            total_dist = dists[0] + dists[1];

            coords.Insert(1, c);

            return;
        }


        int best_i = 1;
        double lowest_dist = total_dist - dists[0] + Coord.dist(c, coords[0]) + Coord.dist(c, coords[1]); 
        
        for(int i = 2; i < coords.Count; i ++) {
            double curr_dist = total_dist - dists[i - 1] + Coord.dist(c, coords[i - 1]) + Coord.dist(c, coords[i]);
            if(curr_dist < lowest_dist) {
                best_i = i; lowest_dist = curr_dist;
            }
        }
        
        total_dist = lowest_dist;
        dists[best_i - 1] = Coord.dist(c, coords[best_i - 1]);
        dists.Insert(best_i, Coord.dist(c, coords[best_i]));

        coords.Insert(best_i, c);
    } 

}
