@extends('layouts.forms')

@section('title-page')
Registration
@endsection

@section('style')
<link rel="stylesheet" href="{{asset('css/styles_reg.css')}}">
@endsection

@section('content')
                @section('action-form')
                "{{route('register')}}"
                @endsection
                @section('error-login')
                "Login is busy"
                @endsection
                @section('error-password')
                "The password is too small"
                @endsection
                <div>
                  <h1 class="name-input" id="email-name">Email</h1>
                </div> 
                <div class="input-selector" id="email">
                  <input type="email" id="email-input" class="input" name="email"
                  @if($errors->has('email'))
                    placeholder="Email is busy"
                  @endif>
                </div>
                <div>
                  <h1 class="name-input" id="confirm-password-name">Confirm Password</h1>
                </div> 
                <div class="input-selector" id="confirm-password">
                  <input type="password" id="confirm-password-input" class="input" name="password_confirmation"
                  @if($errors->has('password'))
                    placeholder="Passwords don't match"
                  @endif>
                </div>
                @section('button')
                Sing Up
                @endsection
@endsection