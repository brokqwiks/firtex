<?php

namespace App\Http\Controllers\Wallet;

use App\Http\Controllers\Controller;
use App\Models\User;
use App\Models\Wallet;
use Illuminate\Http\Request;
use Session;

class WalletController extends Controller
{
    public function index()
    {
        $user = Wallet::where('login', Session::get('login'))->get()->first();
    }

    public function create()
    {
        $user_profile = User::where('login', Session::get('login'))->get()->first();

        date_default_timezone_set('Europe/Moscow');
        if($user_profile->id !== 1)
        {
            $hash_id = (string)(int)$user_profile->id - 1;
        }
        else
        {
            $hash_id = 0;
        }
        $hash_user = $user_profile->login;
        $hash_date = date("Y:m:d:H:m:s");

        $Hash_data = hash('sha256', $hash_id.$hash_user.$hash_date);
        Wallet::create([
            'login' => $user_profile->login,
            'address' => $Hash_data,
        ]);
    }
}
