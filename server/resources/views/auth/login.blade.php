<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <meta http-equiv="X-UA-Compatible" content="ie=edge">
  <link rel="stylesheet" href="{{asset('css/styles_login.css')}}">
  <title>Login</title>
</head>
<body>
  <style>
    @import url('https://fonts.googleapis.com/css2?family=Bebas+Neue&family=Dela+Gothic+One&family=Questrial&family=Russo+One&display=swap');
  </style>
  <div class="main-block">
    <div class="data-input">
      <div class="choice-menu">
        <div class="menu-element" id="login-element">
            <a href="" id="login-text">Log In</a>
        </div>
        <div class="menu-element" id="singup-element">
            <a href="{{route('register')}}" id="singup-text">Sing Up</a>
        </div>
        <hr id="vl">
      </div>

      <div class="input-block">
        <form action="{{route('login')}}" id="form-data" method="post">
          @csrf
          <h1 class="text-input" id="login-input-text">Login</h1>
          <input type="text" class="input" id="login-input" name="login" value="{{ old('login')}}">
          <h1 class="text-input" id="password-input-text">Password</h1>
          <input type="password" class="input" id="password-input" name="password" value="{{old('login')}}">
          <h2 class="text-checkbox" id="text-checkbox-remember">Remember me</h2>
          <div id="content-remember">
            <label class="checkBox">
              <input id="ch1" type="checkbox" name="remember">
              <div class="transition"></div>
            </label>
          </div>
          <h2 class="text-checkbox" id="text-checkbox-accept">Accept the <a href="">Privacy Policy</a></h2>
          <div id="content-privacy-policy">
            <label class="checkBox">
              <input id="ch1" type="checkbox" name="accept">
              <div class="transition"></div>
            </label>
          </div>
          <input type="submit" id="go-btn" value="Go">
        </form>
      </div>

    </div>
  </div>
  <div class="second-block">
    <div class="logo-block">
      <a href="{{route('home')}}" class="logo"><img src="{{asset('img/logo_firtex_white.png')}}"></a>
    </div>
    <div class="welcome-block">
      <h1 id="welcome-text">Welcome to the Firtex</h1>
    </div>
  </div>
  <script src="{{asset('js/login.js')}}"></script>
</body>
</html>
              
