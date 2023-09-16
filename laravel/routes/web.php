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

Route::get('/user', fn() => 'profile')->middleware('auth')->name('profile');

Route::get('/', 'App\Http\Controllers\HomeController@index')->name('home');
Route::get('/reg', 'App\Http\Controllers\RegisterController@create')->middleware('guest')->name('register');
Route::post('/reg', 'App\Http\Controllers\RegisterController@store')->middleware('guest');

Route::get('/login', 'App\Http\Controllers\Login@create')->name('login');
Route::post('/login', 'App\Http\Controllers\Login@store')->middleware('guest');
