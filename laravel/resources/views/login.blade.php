@extends('layouts.main')
@section('content')
    <div class='container'>
        <form action="{{route('login.post')}}" method="post">
            <div class="mb-3">
              <label for="login" class="form-label">Login</label>
              <input type="text" class="form-control" id="exampleInputEmail1" aria-describedby="emailHelp" name="login">
            </div>
            <div class="mb-3">
              <label for="password" class="form-label">Password</label>
              <input type="password" class="form-control" id="exampleInputPassword1" name="password">
            </div>
            <button type="submit" class="btn btn-primary">Submit</button>
          </form>
    </div>
@endsection