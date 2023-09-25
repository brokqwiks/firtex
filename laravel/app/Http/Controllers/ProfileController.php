<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Session;

class ProfileController extends Controller
{
    public function index()
    {
        $login = Session::get('login');
    }
}
