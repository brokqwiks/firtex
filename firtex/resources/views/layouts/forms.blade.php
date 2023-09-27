<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>@yield('title-page')</title>
    @yield('style')
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Merriweather+Sans:wght@300&family=Odor+Mean+Chey&family=Roboto:wght@100&family=Rubik:wght@300&display=swap" rel="stylesheet">
  </head>
  <body>

        <div class="header">
            <div class="logo-selector">
                <div class="logo">
                  <img src="{{asset('img/logo_firtex_white.png')}}" class="logo-white">
                  <a href="{{route('home')}}" class="home-page-href"></a>
                </div>
            </div>
            <div class="btn-selector">
                <div class="login-btn">
                    <a href="{{route('login')}}" class="login-href">Log In</a>
                </div>
                <div class="singup-btn">
                    <a href="{{route('register')}}" class="singup-href">Sing Up</a>
                </div> 
            </div>
            <div class='form-selector'>
              <form action=@yield('action-form') id='form-register' method="post" novalidate>
                @csrf
                <div>
                  <h1 class="name-input" id="login-name">Login</h1>
                </div> 
                <div class="input-selector" id="login">
                  <input type="text" id="login-input" class="input" name="login"
                  @if($errors->has('login'))
                  placeholder=@yield('error-login')
                  @endif>
                </div>
                <div>
                    <h1 class="name-input" id="password-name">Password</h1>
                  </div> 
                  <div class="input-selector" id="password">
                    <input type="password" id="password-input" class="input" name="password"
                    @if($errors->has('password'))
                      placeholder=@yield('error-password')
                    @endif>
                  </div>
                    @yield('content')
                    <button type="submit" id="submit-btn">@yield("button")</button>
              </form>
            </div>
        </div>
  </body>
</html>
              
