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
        $login = Session::get('login');
        $user = Wallet::where('login', $login)->get()->first();
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

            $words = explode(" ", $data[2]);
            $data[3] = $data[2];
            $data[2] = $words;
            
            Session::put("private-key", $data[2]);
            return view('wallet/wallet', compact('data', 'create_wallet'));

        }
        else
        {
            return view('wallet/wallet');
        }
    }

    public function confirm_private_key(Request $request)
    {   
        $succes_data = [];
        $errors_data = [];
        $private_key = Session::get('private-key');
        $data = $request->all();
        $str = 'confirm-el-';

        dump($data);
        dump($private_key);

        for ($i=0; $i < count($data) - 1; $i++) { 
            $data_element = $data[$str.strval($i)];
            $element_private_key = $private_key[$i];
            if($data_element == $element_private_key)
            {
                $succes_data[$i] = 'succes';
            }
            else {
                $errors_data[$i] = $data_element;
            }
            }
        
        
        if(count($errors_data) > 0)
        {
            return view('wallet/wallet', compact('errors_data'));
        }
        elseif(count($succes_data) == 18)
        {   
            $succes = "succes";
            return redirect(route('wallet'))->with(['succes' => $succes]);
        }

    }
}
