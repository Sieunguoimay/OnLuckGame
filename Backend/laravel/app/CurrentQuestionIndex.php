<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class CurrentQuestionIndex extends Model
{
    public function playingData(){
        return $this->belongTo("App\PlayingData");
    }
}
