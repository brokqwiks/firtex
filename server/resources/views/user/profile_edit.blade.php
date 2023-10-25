<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <link rel="stylesheet" href="{{asset('css/styles_profile_edit.css')}}">
    <link href="https://fonts.googleapis.com/css2?family=Merriweather+Sans:wght@300&family=Odor+Mean+Chey&family=Roboto:wght@100&family=Rubik:wght@300&display=swap" rel="stylesheet">
    <title>Edit Profile</title>
</head>
<body>
    <div class="header">
        <div class="logo-block">
            <a href="{{route('home')}}"><img src="{{asset('img/firtex-logo-black-big.png')}}" id="logo"></a>
        </div>
    </div>
    <div class="menu">
        <div class="input-block">
            <form action="" method="post">
            @csrf
            <div class="input__" id="input__logo">
                <h1>Change Avatar</h1>
            </div>
            <div class="input__" id="input__login">
                <h1 class="text" id="text-login">Login</h1>
                <input type="text" id="input-login" class="input" value="{{$user_data['login']}}" name="login">
            </div>
            <div class="input__" id="input__description">
                <h1 class="text" id="text-description">Description</h1>
                <input type="text" id="input-description" class="input" value="{{$user_data['description']}}" name="description">
            </div>
            <button type="submit" class="button">Save</button>
            </form>
        </div>
    </div>
</body>
</html>