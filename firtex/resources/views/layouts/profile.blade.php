<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <link rel="stylesheet" href="{{asset('css/styles_profile.css')}}">
    <link href="https://fonts.googleapis.com/css2?family=Merriweather+Sans:wght@300&family=Odor+Mean+Chey&family=Roboto:wght@100&family=Rubik:wght@300&display=swap" rel="stylesheet">
    <title>Document</title>
</head>
<body>
    <div class="header">
        <div id="profile-img-block">
            <div id="profile-img-frame">
                <img src="{{asset('storage/users/profile_img/'.$user_data['profile_img'])}}" class="user-profile-img" alt="">
            </div>
        </div>
        <div id="edit-login">
            <a href="{{route('profile.edit')}}"><img src="{{asset('img/icons/edit_icon.png')}}" id="edit-icon"></a>
        </div>
        <div id="settings-block">
            <a href="{{route('profile.settings')}}"><img src="{{asset('img/icons/settings_icon.png')}}" id="settings-icon"></a>
        </div>
        <div id="nick-block">
            <a href="" class="user-login">{{$user_data['login']}}</a>
        </div>
    </div>
    <div class="logo-block">
        <div class="logo">
            <a href="{{route('home')}}" class="logo-img"><img src="{{asset('img/firtex-logo-black-big.png')}}" class="logo-img"></a>
        </div>
    </div>
    <div class="menu">
        <div class="select-item">
            <div class="item" id="collections-button">
                <a href="" class="name-item" id="collections-item">Collections</a>
                <hr class="line-item" id="collections-line">
            </div>
            <div class="item" id="about-button">
                <a href="" class="name-item" id="about-item">About</a>
                <hr class="line-item" id="about-line">
            </div>
            <div class="item" id="created-button">
                <a href="" class="name-item" id="created-item">Created</a>
                <hr class="line-item" id="created-line">
            </div>
        </div>
    </div>
</body>
</html>