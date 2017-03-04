using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Singelton Manager that interfaces between the 'computation' backend and the 'data' backend.
/// Only one element of it exists at any point in time, and it contains all the data.
/// </summary>
public class Manager
{
    public Dictionary<int, UserResponse> user_resp;
    public Dictionary<int, Route> van_route;



    private static Manager instance_;
    private Manager() { 
        user_resp = new Dictionary<int, UserResponse>();
        van_route = new Dictionary<int, Route>();
    }
    public static Manager getInstance() {
        if(instance_ == null) {
            instance_ = new Manager();
        }
        return instance_;
    }


    public Route get_van_route(int van_id){
        if(van_route.ContainsKey(van_id)){
            return van_route[van_id];
        } else {
            //TODO: implement -- probably load from db
            return null;
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
}