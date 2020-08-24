<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateTypedQuestionsTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('typed_questions', function (Blueprint $table) {
            $table->id();
            $table->bigInteger("pack_id");
            $table->string("question");
            $table->string("answer");
            $table->integer("score")->default(0);
            $table->string("images");
            $table->string("hints");
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
        Schema::dropIfExists('typed_questions');
    }
}
