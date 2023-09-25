<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\User;
use Session;

class HomeController extends Controller
{
    public function index()
    {   
        $user_login = Session::get('login');
        try
        {
            $profile_img = User::where('login', $user_login)->get()->first()->profile_img;
        }
        catch(\Exception $e)
        {
            $profile_img = null;
        }

        return view('home', compact('profile_img'));
    }
}
