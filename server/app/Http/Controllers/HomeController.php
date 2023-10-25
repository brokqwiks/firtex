<?php

namespace App\Http\Controllers;

use App\Http\Controllers\Controller;
use App\Models\Profile;
use Illuminate\Http\Request;
use App\Models\User;
use Session;

class HomeController extends Controller
{
    public function index()
    {   
        //Load the page and pass the variable with the profile picture of the authorized user.
        $user_login = Session::get('login');
        try
        {
            $profile_img = Profile::where('login', $user_login)->get()->first()->profile_img;
        }
        catch(\Exception $e)
        {
            $profile_img = null;
        }

        return view('home/home', compact('profile_img',));
    }
}
