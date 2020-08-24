<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class TypedQuestion extends Model
{
    public function pack(){
        return $this->belongTo("App\Pack");
    }
    public function questionPlayingData(){
        return $this->hasMany("App\QuestionPlayingData");
    }
}
