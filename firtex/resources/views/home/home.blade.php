<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <link rel="stylesheet" href="{{asset('css/styles_home.css')}}">
    <link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Dela+Gothic+One&family=Lilita+One&family=Merriweather+Sans:wght@300&family=Odor+Mean+Chey&family=Rammetto+One&family=Roboto:wght@100&family=Rubik:wght@300&display=swap" rel="stylesheet">
    <title>Firtex</title>
</head>
<style>
    @import url('https://fonts.googleapis.com/css2?family=Bebas+Neue&family=Dela+Gothic+One&family=Questrial&family=Russo+One&display=swap');
  </style>
<body>
    <div class="header">
        @auth
        <div class="profile-block">
            <div class="profile-menu">
                <nav class="menu-list">
                    <a class="button" id="profile__button" href="{{route('profile')}}">
                        Profile
                    </a>
                    <a class="button" id="wallet__button" href="">
                        Wallet
                    </a>
                    <a class="button" id="settings__button" href="{{route('profile.settings')}}">
                        Settings
                    </a>

                    <a class="button" id="exit__button">
                        Exit
                    </a>
                </nav>
                <div class="menu-button" id="menu-button">
                    <img src="{{asset('storage/users/profile_img/brokqwiks.png')}}" id="button__menu" >
                </div>
            </div>
 
        </div>
        @endauth
        <div class="logo-block">
            <a href="" class="home-href"><img src="{{asset('img/logo_firtex_white.png')}}" id="logo"></a>
        </div>
        <div class="header-button" id="header-about">
            <a href="" class="header-href">About</a>
        </div>
        <div class="header-button" id="header-support">
            <a href="" class="header-href">Support</a>
        </div>
    </div>
    @guest
    <div class="menu">
        <div>
            <h1 id="menu-text">Do you want to participate in the beta test of the project?</h1>
        </div>
        <div class="button-menu">
            <a id="login-btn" class="button" href="{{route('login')}}">
                Log In
            </a>
            <a id="singup-btn" class="button" href="{{route('register')}}">
                Sing Up
            </a>
        </div>
    </div>
    @endguest
    <div class="end-menu">
        <div class="profiles-menu">
            <hr id="profiles-line">
            <a href="https://github.com/brokqwiks/firtex"><img src="{{asset('img/icons/home/github-icon.png')}}" class="profile-icon" id="github-icon"></a>
            <a href="https://discord.gg/zt3dedaKMb"><img src="{{asset('img/icons/home/discord-icon.png')}}" class="profile-icon" id="discord-icon"></a>
        </div>
    </div>
    <script src="{{asset('js/main.js')}}"></script>
</body>
</html>