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
        
        $file = fopen("C:/OpenServer/domains/test/blockchain/data/data.txt", "r+");
        $data = fgets($file);
        fclose($file);


        $data_connect = ["create_wallet"];
        $url = 'http://localhost:9000';

        $ch = curl_init($url);
	    curl_setopt($ch, CURLOPT_POSTFIELDS, $data_connect);
	    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	    $response = curl_exec($ch);
	    curl_close($ch);

        return view('home/home', compact('profile_img', 'data'));
    }
}
