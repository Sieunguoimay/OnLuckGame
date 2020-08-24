<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class QuestionPlayingData extends Model
{
    public function playingData(){
        return $this->belongTo("App\PlayingData");
    }
}
