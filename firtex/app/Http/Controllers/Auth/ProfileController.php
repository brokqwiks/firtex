<?php

namespace App\Http\Controllers\Auth;

use App\Http\Controllers\Controller;
use App\Models\User;
use Illuminate\Http\Request;
use Session;

class ProfileController extends Controller
{
    public function index()
    {
        $user_session = User::where('login', Session::get('login'))->get()->first();
        $user_data = [
            'login' => $user_session->login,
            'email' => $user_session->email,
            'profile_img' => $user_session->profile_img,
        ];
        return view('user/profile_collection', compact('user_data'));
    }

    public function edit()
    {
        return 'edit';
    }

    public function settings()
    {
        return 'settings';
    }
}
