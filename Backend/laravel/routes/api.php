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
Route::get('onluck/getuser','OnluckController@GetUser');
Route::get('onluck/checkuptodateuserdata','OnluckController@CheckUptodateUserData');
Route::get('onluck/notifyserverongamestart','OnluckController@NotifyServerOnGameStart');

Route::post('onluck/createseason','OnluckController@CreateSeason');
Route::post('onluck/createpack','OnluckController@CreatePack');
Route::post('onluck/createquestion','OnluckController@CreateQuestion');
Route::get('onluck/createquestion','OnluckController@CreateQuestion');
Route::get('onluck/getseasons','OnluckController@GetSeasons');
Route::get('onluck/getpacks','OnluckController@GetPacks');
Route::get('onluck/getquestions','OnluckController@GetQuestions');
Route::post('onluck/deleteseason','OnluckController@DeleteSeason');
Route::post('onluck/deletepack','OnluckController@DeletePack');
Route::post('onluck/deletequestion','OnluckController@DeleteQuestion');
Route::post('onluck/updateseason','OnluckController@UpdateSeason');
Route::post('onluck/updatepack','OnluckController@UpdatePack');
Route::post('onluck/updatequestion','OnluckController@UpdateQuestion');

Route::get('onluck/createonluckmetadata','OnluckController@CreateOnluckMetadata');
Route::get('onluck/getonluckmetadata','OnluckController@GetOnluckMetadata');
Route::post('onluck/storemetadata','OnluckController@StoreMetadata');
Route::get('onluck/changeseason','OnluckController@ChangeSeason');
Route::get('onluck/activateseason','OnluckController@ActivateSeason');
Route::get('onluck/downloadgamedata','OnluckController@DownloadGameData');

Route::get('onluck/getplayingdata','OnluckController@GetPlayingData');
Route::post('onluck/storeplayingdata','OnluckController@StorePlayingData');
Route::post('onluck/storeactivedata','OnluckController@StoreActiveData');


Route::get('onluck/getquestionbyid','OnluckController@GetQuestionById');
