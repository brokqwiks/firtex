<?php

use Illuminate\Support\Facades\Route;

/*
|--------------------------------------------------------------------------
| Web Routes
|--------------------------------------------------------------------------
|
| Here is where you can register web routes for your application. These
| routes are loaded by the RouteServiceProvider and all of them will
| be assigned to the "web" middleware group. Make something great!
|
*/

Route::get('/', 'App\Http\Controllers\HomeController@index')->name('home');
Route::get('/reg', 'App\Http\Controllers\Auth\RegisterController@create')->middleware('guest')->name('register');
Route::post('/reg', 'App\Http\Controllers\Auth\RegisterController@store')->middleware('guest');


Route::get('/login', 'App\Http\Controllers\Auth\Login@create')->name('login');
Route::post('/login', 'App\Http\Controllers\Auth\Login@store')->middleware('guest');

Route::post('/logout', 'App\Http\Controllers\Auth\Login@destroy')->middleware('auth')->name('logout');


Route::get('/profile', 'App\Http\Controllers\Auth\ProfileController@index')->middleware('auth')->name('profile');
Route::get('/profile/edit', 'App\Http\Controllers\Auth\ProfileController@edit')->middleware('auth')->name('profile.edit');
Route::post('/profile/edit', 'App\Http\Controllers\Auth\ProfileController@edit_store')->middleware('auth');
Route::get('/profile/settings', 'App\Http\Controllers\Auth\ProfileController@settings')->middleware('auth')->name('profile.settings');

Route::get('/wallet', 'App\Http\Controllers\Wallet\WalletController@index')->middleware('auth')->name('wallet');
Route::post('/wallet', "App\Http\Controllers\Wallet\WalletController@confirm_private_key")->middleware('auth');

Route::get('/wallet/key','App\Http\Controllers\Wallet\WalletController@show')->middleware('auth')->name('wallet.key');
Route::post('/wallet/key','App\Http\Controllers\Wallet\WalletController@show_post')->middleware('auth');

