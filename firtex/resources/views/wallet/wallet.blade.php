<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <link rel="stylesheet" href="{{asset('css/styles_wallet.css')}}">
    <title>Wallet</title>
</head>
<body>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Bebas+Neue&family=Dela+Gothic+One&family=Questrial&family=Russo+One&display=swap');
      </style>
    <div class="header">
        <div class="logo-block">
            <a href="{{route('home')}}" class="home-href"><img src="{{asset('img/logo_firtex_white.png')}}" id="logo"></a>
        </div>
        <div id="wallet__logo">
            <a href="" class="href__wallet" id="firtex__href">Firtex</a><a href="" class="href__wallet" id="line__href">|</a><a href="" class="href__wallet" id="wallet__href">Wallet</a>
        </div>
        @auth
        <div class="profile-block">
            <div class="profile-menu">
                <nav class="menu-list">
                    <a class="button" id="profile__button" href="{{route('profile')}}">
                        Profile
                    </a>
                    <a class="button" id="wallet__button" href="{{route('wallet')}}">
                        Wallet
                    </a>
                    <a class="button" id="settings__button" href="{{route('profile.settings')}}">
                        Settings
                    </a>
                    <form action="{{route('logout')}}" method="post">
                    @csrf
                        <button type="submit" class="button" id="exit__button">Exit</button>
                    </form>
                </nav>
                <div class="menu-button" id="menu-button">
                    <img src="{{asset('storage/users/profile_img/brokqwiks.png')}}" id="button__menu" >
                </div>  
            </div>
        </div>
        @endauth
    </div>
    <div class="wallet-create-block">
        <div id="create-h1-block">
            <h1 id="create-h1">
                Creating your wallet</h1>
        </div>
        <div class="ld-ripple">
            <div></div>
            <div></div>
          </div>
    </div>
    <script src="{{asset('js/main.js')}}"></script>
</body>
</html>