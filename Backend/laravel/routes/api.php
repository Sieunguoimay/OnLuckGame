<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;

/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| is assigned the "api" middleware group. Enjoy building your API!
|
*/

// Route::middleware('auth:api')->get('/user', function (Request $request) {
//     return $request->user();
// });

Route::get('onluck','OnluckController@GetInfo');
Route::post('onluck/signin','OnluckController@SignIn');
Route::post('onluck/signup','OnluckController@SignUp');
Route::get('onluck/signup','OnluckController@SignUp');
Route::get('onluck/login','OnluckController@LogIn');
Route::get('onluck/getusers','OnluckController@GetUsers');
Route::get('onluck/deleteuser','OnluckController@DeleteUser');
Route::get('onluck/resendverificaionemail','OnluckController@ResendVerificationEmail');
Route::get('onluck/verifyemail','OnluckController@VerifyEmail');
Route::post('onluck/uploadphoto','OnluckController@UploadPhoto');
Route::post('onluck/rename','OnluckController@Rename');
Route::get('onluck/getscoreboard','OnluckController@GetScoreboard');
