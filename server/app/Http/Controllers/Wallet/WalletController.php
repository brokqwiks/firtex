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
        $token_user =hash('sha256', $login.$login);
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
                $lines = explode("\n", $data);

                // Инициализация пустого массива для хранения групп:
                $groups = [];

                // Инициализация временного массива:
                $temp = [];

                // Проход по каждой строке:
                foreach ($lines as $line) {
                    // Если текущая строка не пуста:
                    if (!empty(trim($line))) {
                        $temp[] = $line;  // добавить ее во временный массив.
                    }

                    // Если во временном массиве три строки:
                    if (count($temp) == 3) {
                        $groups[] = $temp;  // добавить его в основной массив.
                        $temp = [];  // обнулить временный массив.
                    }
                }

                foreach ($groups as $key => $subarray) {
                    foreach ($subarray as $subkey => $string) {
                        $groups[$key][$subkey] = str_replace("\r", "", $string);
                        // альтернативно можно использовать trim с указанным символом: $array[$key][$subkey] = trim($string, "\r");
                    }
                }
                
                

                foreach($groups as $subarray)
                {
                    if($subarray[0] == $login)
                    {
                        $keys = $subarray;
                        return $keys;
                    }
                }
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
            $data[3] = $data[2];
            $data[2] = $words;
            
            $private_key = $data[1];
            $hash_login = hash('sha256', $data[0]);
            Session::put('_token_user', $hash_login);
            Session::put('private_key' , $private_key);
            return view('wallet/wallet', compact('data', 'create_wallet'));
            }

        }
        else
        {   
            $create_wallet = "false";
            $data = "false";
            $hash_login = hash('sha256', $data[0]);
            Session::put('_token_user', $hash_login);
            return view('wallet/wallet', compact('create_wallet', 'data'));
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

        if($_token_user_session == $_token_user_form)
        {   
            $token_user = hash('sha256', $_token_user_session.$_token_user_form);
            Wallet::create([
                'login' => $token_user,
                'private_key' => $private_key
            ]);
        }
        
        return redirect(route('wallet'));
    }

}
