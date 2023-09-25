@extends('layouts.forms')

@section('title-page')
Log In
@endsection

@section('style')
<link rel="stylesheet" href="{{asset('css/styles_login.css')}}">
@endsection

@section('action-form')
"{{route("login")}}"
@endsection

@section('button')
Sing Up
@endsection

@section('error-login')
    "Incorrect login or password"
@endsection

@section('content')
    <div class="checkbox-block">
        <input type="checkbox" id="login-checkbox" name="remember">
        <h1 class="text" id="checkbox-text">Remember me</h1>
        <div class="forgot-password">
            <a href="" class="text" id="forgot-password-text">Forgot password?</a>
        </div>
    </div>
    
@endsection
