<?php

namespace App\Http\Controllers\Wallet;

use App\Http\Controllers\Controller;
use App\Models\User;
use App\Models\Wallet;
use Exception;
use Illuminate\Http\Request;
use Session;

class WalletController extends Controller
{
    public function index(Request $request)
    {   
        $login = hash('sha256',Session::get('login'));
        $token_user = hash('sha256', $login.$login);
        $user = Wallet::where('login', $token_user)->get()->first();
        if($user == null)
        {   

            function data_send(){
                
                    $login = Session::get('login');
                    
                    $data_connect = ["create_wallet", $login];
                    
                    sleep(0.5);
                    
                    $url = 'http://localhost:7000';
                    
                    $ch = curl_init($url);
                    curl_setopt($ch, CURLOPT_POSTFIELDS, $data_connect);
                    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
                    $response = curl_exec($ch);
                    curl_close($ch);
                    
                    sleep(1);
                    
                
                $login = Session::get('login');
                $file = "C:/OpenServer/domains/test/blockchain/data/data.txt";
                $data = file_get_contents($file);
                // Инициализация пустого массива для хранения групп:

                // Инициализация временного массива:

                // Проход по каждой строке:

                $lines = explode("\r\n", $data);
                $array = [];

                foreach($lines as $line) {
                    $array[] = $line;
                }

                array_pop($array);

                return $array;
            }

            $data = data_send();
            $create_wallet = 'true';

            if($data == null)
            {
                while($data == null)
                {
                    $data = data_send();
                }
            }

            if($data != null){  
            $words = explode(" ", $data[2]);
            $data[4] = $data[3];
            $data[3] = $data[2];
            $data[2] = $words;
            


            $private_key = $data[1];
            $address = $data[4];
            $hash_login = hash('sha256', $data[0]);
            Session::put('_token_user', $hash_login);
            Session::put('private_key' , $private_key);
            Session::put('address', $address);
            Session::put('data', $data);
            return view('wallet/wallet', compact('data', 'create_wallet'));
            }

        }
        else
        {   
            $user = Wallet::where('login','=', $token_user)->get()->first();

            $data_user_wallet = [
                'address' => $user->address,
                'private_key' => $user->private_key,
                'login' => $user->login,
            ];

            $create_wallet = "false";
            $data = "false";
            return view('wallet/wallet', compact('create_wallet', 'data_user_wallet', 'data'));
        }
    }

    public function show()
    {
        return view('wallet/wallet');
    }

    public function show_post(Request $request)
    {   
        $_token_user_session = Session::get('_token_user');
        $_token_user_form = $request->_token_user;
        $private_key = Session::get('private_key');
        $address = Session::get('address');

        if($_token_user_session == $_token_user_form)
        {   
            $token_user = hash('sha256', $_token_user_session.$_token_user_form);
            Wallet::create([
                'login' => $token_user,
                'private_key' => $private_key,
                'address' => $address,
            ]);
        }
        
        return redirect(route('wallet'));
    }

}
