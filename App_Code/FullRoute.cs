using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;

/// <summary>
/// The Route is a set of points that gives a rough example of how distance will be done.
/// The elements of coords are sorted in a way that makes it reasonable to assume that the path
/// is from element #0 to the last element.
/// This class is now complete.
/// </summary>
public class FullRoute
{
    public List<Coord> coords;
    public List<double> dists;
    int van_id;
    public double total_dist = 0;
    public bool correct_direction = true;   // determines whether we are moving in the correct direction.
                                            // ie from start -> end or from end -> start.
    public int old_loc_ind = -1;

    public FullRoute(int vid)
    {
        coords = new List<Coord>();
        dists = new List<double>();
        van_id = vid;

        var db = Database.Open("vane");
        var q = db.QuerySingle("SELECT start_lat, start_lon, end_lat, end_lon FROM route_endpoints WHERE van_id = @0", vid);
        Coord start = new Coord(Convert.ToDouble(q.start_lat), Convert.ToDouble(q.start_lon));
        Coord end = new Coord(Convert.ToDouble(q.end_lat), Convert.ToDouble(q.end_lon));

        coords.Add(start);
        coords.Add(end);

        dists = new List<double>();
        dists.Add(Coord.dist(start, end));

        total_dist = dists[0];


        var qs = db.Query("SELECT lat, lon FROM routes WHERE van_id = @0", vid);
        foreach(var sq in qs) {
            Coord nc = new Coord(Convert.ToDouble(sq.lat), Convert.ToDouble(sq.lon));
            verbose_attempt_add(nc);
        }

        db.Close();
    }


    // attemps to insert the coordinate in the entire list.
    // The attemp will only succeed if there isn't a near enough point anywhere else. In this case.
    // 1. The index at which it is placed is returned.
    // 2. The data is entered into the db.
    // Otherwise, we just return were it would have been placed (index of 'almost equal' point).
    public int verbose_attempt_add(Coord c) {

        // we need to determine where to put the coordinate.
        // Put it in the location that minimizes total route length. it closest to the one before and after it.
        // But only if it's more than IGNORE_DIST from any other point.

        if(coords.Count == 2) {
            double dist_to_0 = Coord.dist(coords[0], c);
            if(dist_to_0 < Coord.IGNORE_DIST) return 1;
            double dist_to_1 = Coord.dist(coords[1], c);
            if(dist_to_1 < Coord.IGNORE_DIST) return 1;

            // just add it in the middle.
            dists[0] = Coord.dist(coords[0], c);
            dists.Add(Coord.dist(coords[1], c));
            total_dist = dists[0] + dists[1];

            coords.Insert(1, c);

            return 1;
        }


        int best_i = 1;
        double dist_to_before = Coord.dist(c, coords[0]);
        if(dist_to_before < Coord.IGNORE_DIST) return 1;
        double dist_to_after = Coord.dist(c, coords[1]);
        if(dist_to_before < Coord.IGNORE_DIST) return 2;

        double lowest_dist = total_dist - dists[0] + dist_to_after + dist_to_before;

        for(int i = 2; i < coords.Count; i ++) {
            dist_to_before = Coord.dist(c, coords[i-1]);
            if(dist_to_before < Coord.IGNORE_DIST) return i;
            dist_to_after = Coord.dist(c, coords[i]);
            if(dist_to_after < Coord.IGNORE_DIST) return i+1;

            double curr_dist = total_dist - dists[i - 1] + dist_to_after + dist_to_before;
            if(curr_dist < lowest_dist) {
                best_i = i; lowest_dist = curr_dist;
            }
        }

        total_dist = lowest_dist;
        dists[best_i - 1] = Coord.dist(c, coords[best_i - 1]);
        dists.Insert(best_i, Coord.dist(c, coords[best_i]));

        coords.Insert(best_i, c);

        var db = Database.Open("vane");
        db.Execute("INSERT INTO routes (van_id, lat, lon, time) VALUES (@0, @1, @2, @3)", van_id, c.lat.ToString(), c.lon.ToString(), DateTime.Now);
        db.Close();

        return best_i;
    }


    // returns the index of where it would be placed if it were going to be placed.
    // This is used to determine which direction we are moving in (ask about last two locations and
    // check chagne in i).
    public int theoretic_add(Coord c){
        if(coords.Count == 2) {
            return 1;
        }

        int best_i = 1;
        double dist_to_before = Coord.dist(c, coords[0]);
        double dist_to_after = Coord.dist(c, coords[1]);

        double lowest_dist = total_dist - dists[0] + dist_to_after + dist_to_before;

        for(int i = 2; i < coords.Count; i ++) {
            dist_to_before = Coord.dist(c, coords[i-1]);
            dist_to_after = Coord.dist(c, coords[i]);

            double curr_dist = total_dist - dists[i - 1] + dist_to_after + dist_to_before;
            if(curr_dist < lowest_dist) {
                best_i = i; lowest_dist = curr_dist;
            }
        }

        return best_i;
    }

    // called everytime a new coord is recieved. It tries to add it to the route, then sees if it can determine the direction from it.
    public void update_new_loc(Coord c) {
        int new_loc_ind = verbose_attempt_add(c);
        if(old_loc_ind != -1) {
            if(new_loc_ind > old_loc_ind) { correct_direction = true;}
            else if(new_loc_ind < old_loc_ind) {correct_direction = false;}
        }
        old_loc_ind = new_loc_ind;

        var db = Database.Open("vane");
        db.Execute("Update recent_reading SET lat = @0, lon = @1, time = @2 WHERE van_id = @3", c.lat.ToString(), c.lon.ToString(), DateTime.Now, van_id);
        db.Close();
    }


    public Route get_sub_route(Coord c) {
        // given the current start location, get the small route that teh van will take.
        List<Coord> route = new List<Coord>();
        double sub_dist = 0;

        int c_add_loc = theoretic_add(c);
        if(correct_direction){
            // start at c and end at last point
            for(int i = c_add_loc; i < coords.Count; i++){
                route.Add(coords[i]);
                if(i != coords.Count - 1){ sub_dist += dists[i]; }
            }
        } else {
            // start at c and end at last point
            for(int i = 0; i <= c_add_loc; i++){
                route.Add(coords[i]);
                if(i != c_add_loc){ sub_dist += dists[i]; }
            }
        }

        return new Route(route, sub_dist);
    }

}
