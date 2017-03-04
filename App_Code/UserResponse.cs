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

    private int which_index;

    public UserResponse(int uid, Coord curr_start, Coord curr_end)
    { 
        compute_all(uid, curr_start, curr_end);
    }

    public void compute_all(int uid, Coord curr_start, Coord curr_end) {
        user_id = uid;
        user_start = curr_start;
        user_end = curr_end;
        which_index = 0;

        //TODO: Call itani's function that will populate resps.

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
}
