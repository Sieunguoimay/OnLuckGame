<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class Pack extends Model
{
    public function season(){
        return $this->belongTo("App\Season");
    }
    public function mcqQuestions(){
        return $this->hasMany("App\McqQuestion",'pack_id','id');
    }
    public function typedQuestions(){
        return $this->hasMany("App\TypedQuestion",'pack_id','id');
    }
}
