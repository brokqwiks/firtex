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
    @if($create_wallet != 'false')
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
            <h2 class="header__private-key-text" id="header__errors-text">A mistake was made somewhere</h2>
        </div>
        
        <div class="private-key">
            @if($data != 'false')
            @for($el=0; $el < count($data[2]); $el++)
            <input type="hidden" value="{{$data[3]}}" id="private-key-str">
            <div class="hidden-private-key-block" id="hidden-private-key-block__{{$el}}"></div>
            <div class="private-key__element" id="private-key_element{{$el}}"><a class="number-private-key">{{$el + 1}}{{'. '}}</a><a class="private-key-el">{{$data[2][$el]}}</a></div>
            <input type="text" class="confirm-private-key" id="confirm-private-key__el{{$el}}" name="confirm-el-{{$el}}">
            @endfor
            @endif
            <button class="send-btn">Send</button> 
            <button class="send-btn" type="submit" id="back-btn">
                Back
            </button>
        </div>
        
        <button class="button2">
            Show
        </button>

        <button class="Btn">
            <p class="text">COPY</p>
            <span class="effect"></span>
        </button>
        <button class="next-btn" type="submit">
            Next
        </button>
    </div>
    @endif
    <form action="{{route('wallet.key')}}" method="post">
    @csrf
    <input type="hidden" value="{{hash('sha256', $data[0])}}" name="_token_user">
    <button id="create-finish" type="submit">Your wallet is created!</button>
    </form>
    
    <div class="active-menu">
        <h1 id="active-menu-text">Active menu</h1>
        <div class="checkmark">
            <svg width="224" height="224" viewBox="0 0 224 224" fill="none" xmlns="http://www.w3.org/2000/svg">
            <circle cx="112" cy="112" r="107" stroke="#68C81C" stroke-width="10"/>
            <path d="M54 109L99.0454 149.383C99.434 149.731 100.025 149.722 100.403 149.362L170 83" stroke="#68C81C" stroke-width="12" stroke-linecap="round"/>
            </svg>
        </div>
    </div>

    <div class="wallet-menu">
        <div class="balance-menu">
            <a href="" id="balance">0.00 FRTX</a>
        </div>

        <div class="address-menu">
            <h1 class="wallet-text" id="address-text">Your Firtex Address</h1>
            <div id="address-block">
            <a href="@" id="address">{{$data_user_wallet['address']}}</a>
            </div>
        </div>
    </div>

    <script src="{{asset('js/main.js')}}"></script>
</body>
</html>