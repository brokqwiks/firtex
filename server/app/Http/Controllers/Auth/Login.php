<?php

namespace App\Http\Controllers\Auth;

use App\Http\Controllers\Controller;
use Auth;
use Illuminate\Http\Request;
use Session;

class Login extends Controller
{
    public function create()
    {
        return view('auth/login');
    }

    public function store(Request $request)
    {   
        if ($request->has('accept')) {
            //We are validating the form so that the login and password fields are mandatory.
            $credentials = $request->validate([
                'login' =>'required|string',
                'password' =>'required|string',
            ]);
    
            //Check if there is such data in the database, if not, then reload the page and notify the user about the error.
            if(! Auth::attempt($credentials, $request->boolean('remember')))
            {   
                return back()
                ->withErrors([
                    'login' => 'Invalid login or password'
                ]);
            }
            
            $request->session()->regenerate();
    
            //Add login data to the session in order to use them later when opening the profile page
            Session::put('login', $request->login);
    
            return redirect(route('home'));
        }
        else
        {
            return back()->withInput($request->only('login','password'));
        }
    }

    public function destroy(Request $request)
    {
        Auth::logout();

        $request->session()->invalidate();

        $request->session()->regenerateToken();

        return redirect()->route('home');
    }
}
