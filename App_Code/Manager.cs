using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;


/// <summary>
/// Singelton Manager that interfaces between the 'computation' backend and the 'data' backend.
/// Only one element of it exists at any point in time, and it contains all the data.
/// </summary>
public class Manager
{
    public Dictionary<int, UserResponse> user_resp;
    public Dictionary<int, FullRoute> van_route;

    private static Manager instance_;
    private Manager() {
        user_resp = new Dictionary<int, UserResponse>();
        van_route = new Dictionary<int, FullRoute>();
    }
    public static Manager getInstance() {
        if(instance_ == null) {
            instance_ = new Manager();
        }
        return instance_;
    }

    public Route get_van_route(int van_id){
        var db = Database.Open("vane");

        // we want to get van_route. First and foremost, check that this van_id exists
        if(db.QueryValue("SELECT Count (*) FROM recent_reading WHERE van_id = @0", van_id) == 0) {
            return new Route();
        }

        // check that the last check in time was less than 30 seconds.
        // TODO(emad): since we're not gonna have live data to demo, so avoid doing this, now
        var q = db.QuerySingle("SELECT lon, lat, time FROM recent_reading WHERE van_id = @0", van_id);
        Coord last_seen = new Coord(Convert.ToDouble(q.lat), Convert.ToDouble(q.lon));
        db.Close();

        // now check if this van's data has already been loaded - should be true.
        if(van_route.ContainsKey(van_id)){
            return van_route[van_id].get_sub_route(last_seen);
        } else {
            FullRoute fr = new FullRoute(van_id);
            van_route[van_id] = fr;
            return fr.get_sub_route(last_seen);
        }
    }

    public SingleUserResponse get_user_resp(int user_id, Coord curr_start, Coord curr_end){
        if(user_resp.ContainsKey(user_id) && user_resp[user_id].check_valid(curr_start, curr_end)) {
            return user_resp[user_id].get_resp();
        } else {
            UserResponse resp = new UserResponse(user_id, curr_start, curr_end);
            user_resp[user_id] = resp;
            return resp.get_resp();
        }
    }

    public void new_reading(Coord c, int van_id){
        if(van_route.ContainsKey(van_id)){
            van_route[van_id].update_new_loc(c);
        } else {
            FullRoute fr = new FullRoute(van_id);
            van_route[van_id] = fr;
            van_route[van_id].update_new_loc(c);
        }

    }
}
