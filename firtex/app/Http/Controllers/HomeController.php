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

        $url = "127.0.0.1:7000";

        $data = ["1234", "3", '41414', '1'];

        $curl = curl_init($url);
        curl_setopt($curl, CURLOPT_POST, true);
        curl_setopt($curl, CURLOPT_POSTFIELDS, http_build_query($data));
        curl_setopt($curl, CURLOPT_RETURNTRANSFER, true);
        $response = curl_exec($curl);
        curl_close($curl);

        return view('home/home', compact('profile_img', 'data'));
    }
}
