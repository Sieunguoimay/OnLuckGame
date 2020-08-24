<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class McqQuestion extends Model
{    
    public function pack(){
        return $this->belongTo("App\Pack");
    }
}
