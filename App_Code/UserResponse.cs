using System;
using System.Collections.Generic;
using System.Web;
/// <summary>
/// A class that contains all the SingleUserResponse that we are willing to show the user. 
/// Takes a user_id and start and end coords, and does computations using Itani's functions to
/// populate resps.
///
/// Has a check_valid function to avoid causing recomputations when the user hasn't even moved.
///
/// 'Next path' is covered via get_resp
/// </summary>
public class UserResponse
{
    public List<SingleUserResponse> resps;
    public Coord user_start;
    public Coord user_end;
    public int user_id;
    public int which_index;
    public UserResponse(int uid, Coord curr_start, Coord curr_end)
    { 
        compute_all(uid, curr_start, curr_end);
    }
    public void compute_all(int uid, Coord curr_start, Coord curr_end) {
        user_id = uid;
        user_start = curr_start;
        user_end = curr_end;
        which_index = 0;

        resps = new List<SingleUserResponse>();

        //TODO: Call itani's function that will populate resps.
        Manager mng = Manager.getInstance();
        int min_id = 0;
        int min_dist = 9999;
        mng.wake_up_routes();
        foreach (var k in mng.van_route.Keys) {
            
            check_efficient(mng.get_van_route(k));
        }
        resps.Sort(delegate(SingleUserResponse resp1, SingleUserResponse resp2) {
            // Criteria to Delegate
            // Minimize Total Time
            // Minimize Total Walking distance
            return resp1.get_total_time().CompareTo(resp2.get_total_time());
        });
    }
    public SingleUserResponse get_resp() {
        if(resps.Count == 0) return null;
        var to_ret = resps[which_index];    
        which_index = (which_index + 1) % resps.Count;
        return to_ret;
    }
    public bool check_valid(Coord curr_start, Coord curr_end) {
        if(Coord.dist(curr_start, user_start) > 50) return false;
        if(Coord.dist(curr_end, user_end) > 50) return false;
        return true;
    }
    public void check_efficient(Route route) {
       int st = get_intersection_s(route);
       int en = get_intersection_e(route);
       if (st != -1 && en != -1) {
            SingleUserResponse rsp = new SingleUserResponse(user_start, route.coords[st], 
            route.coords[0], route.coords[en], user_end, route);
            resps.Add(rsp);
       }
   }
   public int get_intersection_s(Route route) {
       double min_d = 99999999;
       int min_ind = -1;
       double curr_distt = 0;
       for (var i = 0; i < route.coords.Count; i++) {
           if (i > 0) curr_distt = curr_distt + Coord.dist(route.coords[i], route.coords[i-1]);
           double tmp = Coord.dist(user_start, route.coords[i]);
           bool improve = tmp < min_d;
           bool doable = (tmp/1.4 < curr_distt/route.avg_speed);
           if (improve && doable) {
               min_d = tmp;
               min_ind = i;
           }
       }
       return min_ind;
   }
   
   public int get_intersection_e(Route route) {
       double min_d = 99999999;
       int min_ind = -1;
       double curr_distt = 0;
       for (var i = 0; i < route.coords.Count; i++) {
           if (i > 0) curr_distt = curr_distt + Coord.dist(route.coords[i], route.coords[i-1]);
           double tmp = Coord.dist(user_end, route.coords[i]);
           bool improve = tmp < min_d;
           //bool doable = (tmp/1.4 < curr_distt/route.avg_speed);
           if (improve) {//} && doable) {
               min_d = tmp;
               min_ind = i;
           }
       }
       return min_ind;
   }
}
