<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;

class ProfileController extends Controller
{
    public function index(Request $request)
    {   
        return redirect(route('profile.post'));
    }

    public function show(Request $request)
    {
        return $request;
    }
}
