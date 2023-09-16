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

Route::get('/post', 'App\Http\Controllers\PostController@index')->name('post.index');
Route::get('/post/{post}', 'App\Http\Controllers\PostController@show')->name('post.show');
Route::get('/post/create', 'App\Http\Controllers\PostController@create');
Route::post('/post', 'App\Http\Controllers\PostController@store')->name('post.store');

Route::get('/user', 'App\Http\Controllers\UserController@index')->name('user.index');
Route::get('/user/create', 'App\Http\Controllers\UserController@create');
Route::post('/user', 'App\Http\Controllers\UserController@store')->name('user.store');
Route::get('/user/login', 'App\Http\Controllers\UserController@login');
Route::post('/user', 'App\Http\Controllers\UserController@store_login')->name('user.store_login');

Route::get('/', 'App\Http\Controllers\HomeController@index')->name('home');
Route::get('/reg', 'App\Http\Controllers\RegisterController@create')->middleware('guest')->name('register');
Route::post('/reg', 'App\Http\Controllers\RegisterController@store')->middleware('guest');

Route::get('/user', 'App\Http\Controllers\UserController@index')->middleware('auth')->name('user_home');









