<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateQuestionPlayingDataTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('question_playing_data', function (Blueprint $table) {
            $table->id();
            $table->bigInteger("playing_data_id");
            $table->bigInteger("season_id");
            $table->bigInteger("pack_id");
            $table->bigInteger("question_id");
            $table->char("status")->default('l');
            $table->string("started")->nullable();
            $table->string("ended")->nullable();
            $table->integer("used_hint_count")->default(0);
            $table->integer("score")->default(0);
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('question_playing_data');
    }
}
