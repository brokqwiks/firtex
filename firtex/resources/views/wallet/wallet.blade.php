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
            <input type="hidden" value="{{$create_wallet}}" id="create__wallet-bool">
            <h1 id="create-h1">
                Creating your wallet
            </h1>
        </div>
        <div class="ld-ripple">
            <div></div>
            <div></div>
          </div>
    </div>

    <div class="wallet-private-key_active">
        <div class="header__private-key-h1">
            <h1 class="header__private-key-text">Your secret phrase</h1>
        </div>
        <input type="hidden" value="{{$data[3]}}" id="private-key-str">
        <div class="private-key">
            @for($el=0; $el < count($data[2]); $el++)
                <div class="hidden-private-key-block" id="hidden-private-key-block__{{$el}}"></div>
                <div class="private-key__element" id="private-key_element{{$el}}"><a class="number-private-key">{{$el + 1}}{{'. '}}</a><a class="private-key-el">{{$data[2][$el]}}</a></div>
            @endfor 
        </div>
        <button class="button2">
            Show
        </button>
        <button class="Btn">
            <svg viewBox="0 0 512 512" class="svgIcon" height="1em"><path d="M288 448H64V224h64V160H64c-35.3 0-64 28.7-64 64V448c0 35.3 28.7 64 64 64H288c35.3 0 64-28.7 64-64V384H288v64zm-64-96H448c35.3 0 64-28.7 64-64V64c0-35.3-28.7-64-64-64H224c-35.3 0-64 28.7-64 64V288c0 35.3 28.7 64 64 64z"></path></svg>
            <p class="text">COPY</p>
            <span class="effect"></span>
          </button>
    </div>
    <script src="{{asset('js/main.js')}}"></script>
</body>
</html>