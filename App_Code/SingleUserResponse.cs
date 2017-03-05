using System;
using System.Collections.Generic;
using System.Web;
/// <summary>
/// This class represents the data returned for a single screen in the mobile app.
/// It's entire function is to serve as a good template for JSON.
/// This class is now complete.
/// </summary>
public class SingleUserResponse
{
    public Coord user_start;
    public Coord user_van_meet;
    public Coord van_start;
    public Coord van_end;
    public Coord user_end;
    
    public Route van_route;
    public int time_to_pickup = 0;
    public int time_for_van = 0;
    public int time_to_location = 0;
    public SingleUserResponse(Coord user_s, Coord user_van_m, Coord van_s, Coord van_e, Coord user_e, Route van_r,
                              int ttp, int tfv, int ttl)
    {
        user_start = user_s;
        user_end = user_e;
        user_van_meet = user_van_m;
        van_start = van_s;
        van_end = van_e;
        van_route = van_r;
        time_to_location = ttl;
        time_for_van = tfv;
        time_to_pickup = ttp;
    }
    public SingleUserResponse(Coord user_s, Coord user_van_m, Coord van_s, Coord van_e, Coord user_e, Route van_r)
    {
        user_start = user_s;
        user_end = user_e;
        user_van_meet = user_van_m;
        van_start = van_s;
        van_end = van_e;
        van_route = van_r;
        
        time_to_location = (int)((Coord.dist(user_s, user_van_m)+Coord.dist(van_e, user_e))/(1.4*60) + 1);
        time_for_van = (int)(Coord.dist(user_van_m, van_e)/(van_r.avg_speed*60))+1;
        time_to_pickup = time_to_location + time_for_van;
    }
    public int get_WalkTime() {
        return time_to_location + time_to_pickup;
    }
    public int get_total_time() {
        return time_to_location +  time_for_van + time_to_pickup;
    }
}
