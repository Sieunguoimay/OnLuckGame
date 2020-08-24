<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class PlayingData extends Model
{
    public function currentQuestionIndices(){
        return $this->hasMany("App\CurrentQuestionIndex");
    }
    public function questionPlayingData(){
        return $this->hasMany("App\QuestionPlayingData");
    }
    public function user(){
        return $this->belongTo("App\User");
    }
}
