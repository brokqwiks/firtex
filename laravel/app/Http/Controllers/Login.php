<?php

namespace App\Http\Controllers;

use App\Providers\RouteServiceProvider;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Session;

class Login extends Controller
{
    public function create()
    {
        return view('login');
    }

    public function store(Request $request)
    {   
        //validate form
        $credentials = $request->validate([
            'login' =>'required|string',
            'password' =>'required|string',
        ]);

        //logic login
        if(! Auth::attempt($credentials, $request->boolean('remember')))
        {   
            return back()
            ->withErrors([
                'login' => 'Invalid login or password'
            ]);
        }
        
        $request->session()->regenerate();
        Session::put('login', $request->login);

        return redirect(route('home'));

    }  
}
