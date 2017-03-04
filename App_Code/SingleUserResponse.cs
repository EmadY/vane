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

    public SingleUserResponse(Coord user_s, Coord user_van_m, Coord van_s, Coord van_e, Coord user_e, Route van_r)
    {
        user_start = user_s;
        user_end = user_e;
        user_van_meet = user_van_m;
        van_start = van_s;
        van_end = van_e;
        van_route = van_r;
    }
}
