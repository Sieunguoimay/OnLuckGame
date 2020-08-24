<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class Season extends Model
{
    public function packs(){
        return $this->hasMany("App\Pack");
    }
}
