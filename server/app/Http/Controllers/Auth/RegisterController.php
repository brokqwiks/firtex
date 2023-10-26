<?php

namespace App\Http\Controllers\Auth;

use App\Http\Controllers\Controller;
use App\Models\User;
use App\Models\Profile;
use App\Models\Wallet;
use Auth;
use Hash;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Redirect;
use Session;
use Symfony\Component\Console\Input\Input;

class RegisterController extends Controller
{
    public function create()
    {   
        return view('auth/register');
    }

    public function store(Request $request)
    {   
        if ($request->has('accept')) {
        //Validating the form so that all fields are mandatory, the login and email fields are unique, and the password consists of 8 characters.
        $request->validate([
            'login' =>'required|string|unique:users',
            'email' =>'required|string|email|unique:users',
            'password' =>'required|confirmed|min:8',
        ]);

        //Add data about the user to the database and create his profile avatar.
        $user_img_path = $request->login.'.png';
        $hash_password = Hash::make($request->password);
        $user_auth = User::create([
            'login' => $request->login,
            'email' => $request->email,
            'password' => $hash_password,
        ]);

        Profile::create([
            'login' => $request->login,
            'profile_img' => $user_img_path,
            'description' => 'About',
        ]);

        
        copy(storage_path('app/public/users/profile_img/test_img.png'), storage_path('app/public/users/profile_img/'.$user_img_path));

        //Authorize the user in the system and add login data to the session in order to use them later when opening the profile page.
        Auth::login($user_auth);
        Session::put('login', $request->login);
        return  redirect(route('home'));
        }
        else
        {   
            return back()->withInput();
        }
    }
}

