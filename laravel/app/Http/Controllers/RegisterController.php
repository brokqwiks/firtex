<?php

namespace App\Http\Controllers;

use App\Models\User;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Hash;

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
        $hash_password = Hash::make($request->password);

        $user = User::create([
            'login' => $request->login,
            'email' => $request->email,
            'password' => $hash_password,
        ]);

        Auth::login($user);

        return redirect(route('home'));
    }
}
