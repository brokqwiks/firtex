<?php

namespace App\Http\Controllers\Auth;

use App\Http\Controllers\Controller;
use App\Models\User;
use Auth;
use Hash;
use Illuminate\Http\Request;
use Session;

class RegisterController extends Controller
{
    public function create()
    {   
        return view('auth/register');
    }

    public function store(Request $request)
    {   
        //Validating the form so that all fields are mandatory, the login and email fields are unique, and the password consists of 8 characters.
        $request->validate([
            'login' =>'required|string|unique:users',
            'email' =>'required|string|email|unique:users',
            'password' =>'required|confirmed|min:8',
        ]);

        //Add data about the user to the database and create his profile avatar.
        $user_img_path = $request->login.'.png';
        $hash_password = Hash::make($request->password);
        $user = User::create([
            'login' => $request->login,
            'email' => $request->email,
            'password' => $hash_password,
            'profile_img' => $user_img_path
        ]);
        copy(storage_path('app/public/users/profile_img/test_img.png'), storage_path('app/public/users/profile_img/'.$user_img_path));

        //Authorize the user in the system and add login data to the session in order to use them later when opening the profile page.
        Auth::login($user);
        Session::put('login', $request->login);
        return  redirect(route('home'));
    }
}

