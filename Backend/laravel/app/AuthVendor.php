<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class AuthVendor extends Model
{
    public function user(){
        return $this->belongTo('App\User');
    }
}
