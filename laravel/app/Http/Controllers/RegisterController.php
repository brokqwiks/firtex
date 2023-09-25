<?php

namespace App\Http\Controllers;

use App\Models\User;
use Faker\Provider\ar_EG\Company;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Hash;
use Session;

class RegisterController extends Controller
{
    public function create()
    {   
        return view('register');
    }

    public function store(Request $request)
    {   
        //validate form
        $request->validate([
            'login' =>'required|string|unique:users',
            'email' =>'required|string|email|unique:users',
            'password' =>'required|confirmed|min:8',
        ]);

        //register logic
        $user_img_path = $request->login.'.jpg';
        $hash_password = Hash::make($request->password);
        $user = User::create([
            'login' => $request->login,
            'email' => $request->email,
            'password' => $hash_password,
            'profile_img' => $user_img_path
        ]);
        copy(storage_path('app/public/users/profile_img/test_img.jpg'), storage_path('app/public/users/profile_img/'.$user_img_path));
        Auth::login($user);
        Session::put('login', $request->login);
        return  redirect(route('home'));
    }
}
