<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <link rel="stylesheet" href="{{asset('css/styles_home.css')}}">
    <link href="https://fonts.googleapis.com/css2?family=Merriweather+Sans:wght@300&family=Odor+Mean+Chey&family=Roboto:wght@100&family=Rubik:wght@300&display=swap" rel="stylesheet">
    <title>Home</title>
</head>
<body>
    <div class="header">
        <div class=buttons-menu>
            <div class="button" id="about-btn">
                <a href="" class="href-btn">About</a>
            </div>
            <div class="button" id="support-btn">
                <a href="" class="href-btn">Support</a>
            </div>
            <div class="button" id="sell-buy-btn">
                <a href="" class="href-btn">Sell & Buy</a>
            </div>
            @guest
            <div class="button" id="login-singup-btns">
                <div class="button-auth" id="login-btn">
                    <a href="{{route("login")}}" class="button-auth-href" id="href-login">Log In</a>
                </div>
                <div class="button-auth" id="singup-btn">
                    <a href="{{route("register")}}" class="button-auth-href" id="href-singup">Sing Up</a>
                </div>
            </div>
            @endguest
            @auth
            <div id="profile-block">
                <div id="profile-btn">
                    <a href="profile" id="profile-href">
                        <img src="{{asset('storage/users/profile_img/'.$profile_img)}}" id="user-img">
                    </a>
                </div>
            </div>
            @endauth
        </div>
        <div class="logo-block">
            <a href="{{route('home')}}"><img src="{{asset('img/firtex-logo-black.png')}}" id="logo-black"></a>
        </div>
    </div>
    <script src="{{asset('js/home_page.js')}}"></script>
</body>
</html>