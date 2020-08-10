<?php

use Illuminate\Support\Facades\Route;

/*
|--------------------------------------------------------------------------
| Web Routes
|--------------------------------------------------------------------------
|
| Here is where you can register web routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| contains the "web" middleware group. Now create something great!
|
*/

Route::get('/', function () {
    return view('welcome');
});

Route::get('/admin',function(){
    return view("admin");
});

Route::get('/testmail',function(){
    $user = new stdClass();
    $user->email = "pthuduong97@gmail.com";
    $user->name = "Pham Thuy Duong";
    $user->verification_code = 12345;
    return new App\Mail\VerificationMail($user);
});