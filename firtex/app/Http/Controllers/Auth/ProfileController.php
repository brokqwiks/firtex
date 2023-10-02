<?php

namespace App\Http\Controllers\Auth;

use App\Http\Controllers\Controller;
use App\Models\Profile;
use App\Models\User;
use Illuminate\Http\Request;
use Session;

class ProfileController extends Controller
{
    public function index()
    {   
        //We take the login data from the session and run it through the database. We return an array with user data except for the password.
        $user_session = Profile::where('login', Session::get('login'))->get()->first();
        $user_data = [
            'login' => $user_session->login,
            'description' => $user_session->description,
            'profile_img' => $user_session->profile_img,
        ];
        return view('user/profile_collection', compact('user_data'));
    }

    public function edit()
    {   
        $user_session = Profile::where('login', Session::get('login'))->get()->first();
        $user_data = [
            'login' => $user_session->login,
            'description' => $user_session->description,
            'profile_img' => $user_session->profile_img,
        ];
        return view('user/profile_edit', compact('user_data'));
    }

    public function settings()
    {
        return 'settings';
    }

    public function edit_store(Request $request)
    {   
        $user_profile = Profile::where('login', Session::get('login'))->get()->first();
        $user_data = [
            'login' => $user_profile->login,
            'description' => $user_profile->description,
            'profile_img' => $user_profile->profile_img,
        ];

        $form_data = $request->validate([
            'login' => 'required|string',
            'description' => 'required|string'
        ]);

        $user = User::where('login', $user_data['login'])->get()->first();
        
        $user_update = User::find($user->id);
        $profile_update = Profile::find($user_profile->id);
        $user_update->update([
            'login' => $request->login,
        ]);
        $profile_update->update([
            'login' => $request->login,
            'description' => $request->description,
        ]);

        Session::put('login', $request->login);

        return redirect(route('profile'));
    }
}
