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

        $url = "127.0.0.1:9000";

        $data_connect = ["create_wallet"];
        $url = 'http://localhost:9000';

        $ch = curl_init($url);
	// устанавлваем даные для отправки
	    curl_setopt($ch, CURLOPT_POSTFIELDS, $data_connect);
	// флаг о том, что нужно получить результат
	    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	// отправляем запрос
	    $response = curl_exec($ch);
	// закрываем соединение
	    curl_close($ch);

        return view('home/home', compact('profile_img', 'data'));
    }
}
