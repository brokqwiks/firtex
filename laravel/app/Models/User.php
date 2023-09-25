<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Foundation\Auth\User as Authenticatable; 

class User extends Authenticatable
{
    use HasFactory;
    protected $table = 'users';
    protected $guarded = false;

    public function storage_users()
    {
        $storage_users = config('filesystems.links.storage_users');

        return $storage_users;
    }
}
