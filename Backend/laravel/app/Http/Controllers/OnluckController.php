<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Season;
use App\Pack;
use App\TypedQuestion;
use App\McqQuestion;
use App\User;
use App\AuthVendor;

use App\PlayingData;
use App\QuestionPlayingData;
use App\CurrentQuestionIndex;

use App\Mail\VerificationMail;
use Illuminate\Support\Facades\Mail;
use Illuminate\Support\Facades\Storage;
class OnluckController extends Controller
{
    public $DEFAULT_PROFILE_PICTURE = "/assets/icons/default_profile_picture.png";
    public $DEFAULT_VENDOR_NAME = "sieunguoimay";
    public $DEFAULT_PASSWORD = "vuduydu";



    function GetInfo(Request $request){
        $info = array();

        if($request->has("a"))
            $info["data"] = $request->query("a");

        $info["name"] = "Onluck";
        $info["dev"]="Vu Duy Du";
        $info["company"]="Sieunguoimay";
        $info["website"]="http://sieunguoimay.website";


        return json_encode($info);
    }

    public function SignIn(Request $request){
        $response = array();
        $response["status"] = "OK";

        if($request->has("email")){
            $email = $request->get("email");
            if($request->has("vendor")){
                $vendorName = $request->get("vendor");

                //check this email in the database
                $user = User::where("email",$email)->first();
                $vendor = null;
                if($user!=null){
                    //if found: do nothing?
                    $vendor = AuthVendor::where([["user_id",'=',$user->id],["vendor_name",'=',$vendorName]])->first();
                    // if($vendor==null){
                    //     //save it to the vendor table
                    //     $shouldCreateNewVendor = true;
                    // }
                    // $response["status"] = "User existed";
                }else{
                    //else: create new record in table: User
                    $user = new User();
                    $user->email = $email;
                    $user->password = $this->DEFAULT_PASSWORD;
                    //create new record in vendor table
                    $shouldCreateNewVendor = true;
                }

                $user->uptodate_token = time();
                $user->verification_code = 0;
                $user->last_active_vendor_name = $vendorName;
                $user->save();

                $playing_data_uptodate_token = -1;
                $playingData = PlayingData::select('uptodate_token')->where('user_id',$user->id)->first();
                if($playingData!=null)
                    $playing_data_uptodate_token = $playingData->uptodate_token;

                if($vendor == null){

                    $vendor = new AuthVendor();
                    $vendor->vendor_name = $vendorName;
                    $vendor->user_name = $request->has("name")?$request->get("name"):"No Name";
                    $vendor->user_id = $user->id;
                }else{
                    // $response['data']['profile_picture']=$vendor->profile_picture;
                }
                $vendor->profile_picture = $this->saveImageToStorage($request->file('profile_picture'),"profile_picture_".$user->email);
                $vendor->save();
                $response["data"]=[
                    'user_id'=>$user->id,
                    'profile_picture'=>$vendor->profile_picture,
                    'uptodate_token'=>$user->uptodate_token,
                    'playing_data_uptodate_token'=>$playing_data_uptodate_token
                ];
            }else{
                //this is not a valid signin at all.
                $response["status"]="The vendor parameter is missing. You're Signing in. which means you're using a third party vendor. If this vendor parameter is missing, there must be something wrong.";
            }
        }else{
            $response["status"]="email parameter is missing";
        }

        return json_encode($response);
    }
    public function SignUp(Request $request){
        $response = array();
        $response["status"] = "OK";

        if($request->has("email")){
            $email = $request->get("email");
            $user = User::where("email",$email)->first();
            if($user!=null){
                //bro you're signing up. but your email has already existed.
                //you must have signed up/signed in before.

                //let's see if you have signed up (not sign in) before or not.
                //if yes. then I must say sorry. 
                //else, then it's OK. I will create new vendor with password for you.
                $vendor = AuthVendor::where([["user_id",'=',$user->id],["vendor_name",'=',$this->DEFAULT_VENDOR_NAME]])->first();
                if($vendor!=null){
                    //sorry bro
                    $response["status"] = "User already existed";
                    return json_encode($response);
                }else{
                    //You're living fine: most probably you have signed in before.
                    //thus your email has aready been verified
                }


            }else{
                //Ok you're here. where the signing up actually happens
                $user = new User();
                $user->email = $email;
                $user->verification_code = time();//rand(10000,99999);
            }

            //you're good to go.
            $password = $request->get("password");
            if($password==null){
                //invalid registration. this line must not be entered with the help of frontend.
                $response["status"] = "Password parameter missing";
                return json_encode($response);
            }
            $userName = $request->has("name")?$request->get("name"):"No Name";

            $user->password = $password;
            $user->last_active_vendor_name = $this->DEFAULT_VENDOR_NAME;
            $user->uptodate_token = time();//rand(1,1000000);
            $user->save();

            $playing_data_uptodate_token = -1;
            $playingData = PlayingData::select('uptodate_token')->where('user_id',$user->id)->first();
            if($playingData!=null)
                $playing_data_uptodate_token = $playingData->uptodate_token;

            $vendor = new AuthVendor();
            $vendor->user_name = $userName;
            $vendor->vendor_name = $this->DEFAULT_VENDOR_NAME;
            $vendor->user_id = $user->id;
            $vendor->profile_picture= $request->has('profile_picture')?$request->get('profile_picture') :$this->DEFAULT_PROFILE_PICTURE;
            $vendor->save();

            $response['data']=[
                'user_data'=>[
                    'id'=>$user->id,
                    'name'=>$vendor->user_name,
                    'email'=>$user->email,
                    'active_vendor'=>$user->last_active_vendor_name,
                    'profile_picture'=>$vendor->profile_picture,
                    'uptodate_token'=>$user->uptodate_token],
                'playing_data_uptodate_token'=> $playing_data_uptodate_token 
            ];
            if($user->verification_code>0)
                $response['status']="email_not_verified";

            $user->name=$vendor->user_name;
            $this->sendVerificationEmail($user);
        }

        return json_encode($response);
    }
    public function LogIn(Request $request){

        $response = array();
        $response['status']="OK";

        if(!$request->has("email")){
            $response['status']="Missing Email Parameter";
            return json_encode($response);
        }
        if(!$request->has("password")){
            $response['status']="Missing Password Parameter";
            return json_encode($response);
        }

        $email = $request->get("email");
        $password = $request->get("password");
        $user = User::where("email",$email)->first();
        if($user!=null){
            $user->last_active_vendor_name = $this->DEFAULT_VENDOR_NAME;
            $vendor = AuthVendor::where([['user_id','=',$user->id],['vendor_name','=',$user->last_active_vendor_name]])->first();
            if($user->password == $password){
                //Your life is safe
                if($vendor!=null){
                    $userData=[
                        'id'=>$user->id,
                        'uptodate_token'=> rand(1,10000000),
                        'name'=>$vendor->user_name,
                        'email'=>$user->email,
                        'active_vendor'=>$user->last_active_vendor_name,
                        'profile_picture'=>$vendor->profile_picture,
                    ];

                    if($user->verification_code > 0){
                        $response['status']="email_not_verified";
                        //pls jump to the verification section
                    }
                    $user->save();
                    
                    $playing_data_uptodate_token = -1;
                    $playingData = PlayingData::select('uptodate_token')->where('user_id',$user->id)->first();
                    if($playingData!=null)
                        $playing_data_uptodate_token = $playingData->uptodate_token;
                    $response['data']=['user_data'=>$userData,'playing_data_uptodate_token'=>$playing_data_uptodate_token];
                }else{
                    $response['status']="You has not signed up with this email yet";
                }
            }else{
                if($vendor == null){
                    $vendor = AuthVendor::where('user_id',$user->id)->first();
                    $response['status']="not_signed_up_but_already_signed_in";//"No password! This email has linked with facebook but not signed up. Please signup to provide new password.";
                    $response['data'] = [
                        'name'=>$vendor->user_name,
                        'profile_picture'=>$vendor->profile_picture,
                    ];
                }else{
                    $response['status']="Password is incorrect";
                }
            }
        }else{
            $response['status']="You have not signed up with this email yet";
        }
        return json_encode($response);
    }
    public function ResendVerificationEmail(Request $request){
        $response = array();
        $response['status'] = "OK";

        if($request->has("email")){
            $email = $request->get('email');
            $user = User::where('email',$email)->first();
            if($user!=null){
                $vendor = AuthVendor::where([['user_id','=',$user->id],['vendor_name','=',$user->last_active_vendor_name]])->first();
                $user->name = $vendor==null?"":$vendor->user_name;
                $this->sendVerificationEmail($user);
            }else{
                $response["status"]="User with email ".$email." not found";
            }
        }else{
            $response["status"]="Missing email parameter";
        }
        return json_encode($response);
    }

    private function sendVerificationEmail($user){
        error_log("sendVerificationEmail: Assume that an email was sent successfully");
        //Mail::to($user->email)->send(new VerificationMail($user));
    }

    //A Post Request
    public function VerifyEmail(Request $request){
        $response = array();
        $response['status'] = "OK";

        if($request->has("email")){
            $email = $request->get('email');
            if($request->has("verification_code")){
                $verificationCode = $request->get('verification_code');

                $user = User::where('email',$email)->first();
                if($user!=null){
                    if($user->verification_code>0){
                        if($verificationCode == $user->verification_code){
                            $user->verification_code = 0;
                            $user->save();
                        }else{
                            $response["status"]="Incorrect verification code";
                        }
                    }else{
                        $response["status"]="Your email has been verified already";
                    }
                }else{
                    $response["status"]="User with email ".$email." not found";
                }
            }else{
                $response["status"]="Missing verification_code parameter";
            }
        }else{
            $response["status"]="Missing email parameter";
        }
        return json_encode($response);
    }
    
    //getuser data with correct questions count
    public function GetUsers(){
        $response = array();
        $response['status']="OK";
        
        $users = User::all();

        foreach($users as $user){
            $vendor = AuthVendor::where([['user_id','=',$user->id],['vendor_name','=',$user->last_active_vendor_name]])->first();
            $playingData = PlayingData::select('total_score','id')->where('user_id',$user->id)->first();
            $user['profile_picture'] = $vendor->profile_picture;
            $user['name'] = $vendor->user_name;
            $user['pw'] = $user->password;
            $user->score = $playingData->total_score;

            $season_id = $this->getMetadata()->active_season;
            $packs = Pack::select('id')->where('season_id',$season_id)->get();
            $user->current_question_indices = CurrentQuestionIndex::where('playing_data_id',$playingData->id)->whereIn('pack_id',$packs)->pluck('index');
            $user->correct_answer_count = count(QuestionPlayingData::select('status')
                    ->where([['playing_data_id','=',$playingData->id],['season_id','=',$season_id],['status','=',99]])->get());
            unset($user->updated_at);        
        }

        $response["data"] = $users;
        return json_encode($response);
    }
    public function DeleteUser(Request $request){
        $response = array();
        $response['status']="OK";
        if($request->has('id')){
            $userId = $request->query('id');
            $user = User::find($userId);
            if($user!=null){
                $authVendors = AuthVendor::where('user_id',$userId);
                foreach($authVendors as $authVendor){
                    try{
                        unlink(public_path($authVendor->profile_picture));
                    }catch(\Exception $e){}
                    $authVendor->delete();
                }
                $playingData = PlayingData::where('user_id',$userId)->first();
                CurrentQuestionIndex::where('playing_data_id',$playingData->id)->delete();
                QuestionPlayingData::where('playing_data_id',$playingData->id)->delete();
                $playingData->delete();
                $user->delete();
            }else{
                $response['status']="User Id Not found";
            }
        }else{
            $response['status']="Missing id parameter";
        }
        return json_encode($response);
    }
    public function UploadPhoto(Request $request){
        $response = array();
        $response["status"] = "OK";
        if($request->has("id")){
            $id = $request->get("id");
            $user = User::find($id);
            if($user!=null){
                $imagePath = $this->saveImage($request,$user->email);
                if($imagePath!=null){
                    $vendor = AuthVendor::where([['user_id','=',$user->id],['vendor_name','=',$user->last_active_vendor_name]])->first();
                    if($vendor!=null){
                        $vendor->profile_picture = $imagePath;
                        $vendor->save();
                        if(!$request->has("dont_generate_access_token"))
                            $user->uptodate_token = time();//rand(1,1000000);
                        $user->save();
                        $response['data']=['profile_picture'=>$imagePath,'uptodate_token'=>$user->uptodate_token];
                    }else{
                        $response["status"]="User vendor not found";
                    }
                }
                else{
                    $response["status"]="Image not saved";
                }
            }else{
                $response["status"]="User not found";
            }
        }else{
            $response["status"]="Missing id parameter";
        }
        return $response;
    }
    public function Rename(Request $request){
        $response = array();
        $response["status"] = "OK";
        if($request->has("id")){
            $id = $request->get("id");
            $user = User::find($id);
            if($user!=null){
                $vendor = AuthVendor::where([['user_id','=',$user->id],['vendor_name','=',$user->last_active_vendor_name]])->first();
                if($vendor!=null){
                    if($request->has("new_name")){
                        $vendor->user_name = $request->get("new_name");
                        $vendor->save();

                        $user->uptodate_token = time();//rand(1,1000000);
                        $user->save();
                        $response['data']=['uptodate_token'=>$user->uptodate_token];
                    }else{
                        $response["status"]="Missing name parameter";
                    }
                }else{
                    $response["status"]="Vendor not found";
                }
            }else{
                $response["status"]="User not found";
            }
        }else{
            $response["status"]="Missing id parameter";
        }
        return $response;
    }
    private function saveImage($request,$email){
        if($request->hasFile("profile_picture")){
            return $this->saveImageToStorage($request->file('profile_picture'),"profile_picture_".$email);
            // $image = $request->file('profile_picture');
            // $name = "profile_picture_".$email.'.'.$image->getClientOriginalExtension();
            // $destinationPath = public_path('/assets/images');
            // $image->move($destinationPath, $name);
            // error_log("Saved image $name to $destinationPath");
            // return "/assets/images/".$name;
        }
        return null;
    }
    public function GetScoreboard(){

        $response = array();
        $response['status']="OK";
        
        $users = User::all()->sortByDesc("id");//->values();

        foreach($users as $key=>$user){
            if($user->verification_code>0){
                unset($users[$key]);
                continue;
            }
            $vendor = AuthVendor::where([['user_id','=',$user->id],['vendor_name','=',$user->last_active_vendor_name]])->first();
            $user['profile_picture'] = $vendor->profile_picture;
            $user['name'] = $vendor->user_name;
            unset($user['password']);
            unset($user['email']);
            unset($user['verification_code']);
            unset($user['last_active_vendor_name']);
            unset($user['created_at']);
            unset($user['updated_at']);
        }

        $response["data"] = $users->values();
        return json_encode($response);
    }
    public function GetUser(Request $request){
        $response = array();
        $response['status']="OK";
        if($request->has('id')){
            $user = User::find($request->get('id'));
            $vendor = AuthVendor::where([['user_id','=',$user->id],['vendor_name','=',$this->DEFAULT_VENDOR_NAME]])->first();
            if($user!=null&&$vendor!=null){
                $response["data"] = [
                    'id'=>$user->id,
                    'uptodate_token'=>$user->uptodate_token,
                    'name'=>$vendor->user_name,
                    'email'=>$user->email,
                    'active_vendor'=>$this->DEFAULT_VENDOR_NAME,
                    'profile_picture'=>$vendor->profile_picture,
                ];
            }else{
                $response['status']="User with id $user->id not found";
            }
        }else{
            $response['status']='Parameter Id not found';
        }
        return json_encode($response);
    }


    public function GetSeasons(){
        $response = array();
        $response['status']="OK";
        try{
            $seasons = Season::all();
            if($seasons!=null)
                $response["data"] = $seasons->values();
        }catch(\Exception $e){
            $response['status']=$e->getMessage();
        }
        // Storage::disk('local')->put('onluck.json','{}');
        return json_encode($response);
    }

    private function getMetadata(){
        return json_decode(Storage::disk('local')->get('onluck.json'));
    }
    private function generateNewSeasonUptodateToken($id){
        $metadata = $this->getMetadata();
        if($metadata->active_season==$id){
            $metadata->season_uptodate_token = time();
            Storage::disk('local')->put('onluck.json',json_encode($metadata));
            return $metadata;
        }
        return null;
    }

    public function ChangeSeason(Request $request){
        $response = array();
        $response['status']="OK";
        if($request->has("id")){
            $metadata = json_decode(Storage::disk('local')->get('onluck.json'));
            if($metadata->pointed_season!=$request->id){
                $metadata->pointed_season = $request->id;
                $metadata->season_uptodate_token = time();
                Storage::disk('local')->put('onluck.json',json_encode($metadata));
                $response['data'] = $metadata;
            }else{
                $response['status'] = 'Same season id';
            }
        }else{
            $response['status'] = 'Missing parameter id';
        }
        return json_encode($response);
    }
    public function ActivateSeason(Request $request){
        $response = array();
        $response['status']="OK";
        $metadata = json_decode(Storage::disk('local')->get('onluck.json'));
        if($request->has("id")){
            if($metadata->pointed_season==$request->id){
                if($metadata->activation_code!=$metadata->season_uptodate_token){
                    $metadata->activation_code = $metadata->season_uptodate_token;
                    $metadata->active_season = $metadata->pointed_season;
                    Storage::disk('local')->put('onluck.json',json_encode($metadata));
                    $response['data']= $metadata;

                    //update uptodate_token in PlayingData table.
                    $newUptodateToken = time();
                    foreach(PlayingData::all() as $playingData){
                        $playingData->uptodate_token = $newUptodateToken;
                        $playingData->save();
                    }

                    $season = Season::find($metadata->active_season);
                    unset($season->created_at);
                    unset($season->updated_at);
                    $packs = Pack::select(['id','season_id','title','sub_text','icon','question_type'])
                        ->where('season_id',$season->id)->get();
                    foreach ($packs as $pack){
                        if($pack->question_type=="0"){
                            $pack->typed_questions = TypedQuestion::where('pack_id',$pack->id)->get();
                        }else if($pack->question_type=="1"){
                            $pack->mcq_questions = McqQuestion::where('pack_id',$pack->id)->get();
                        }
                    }
                    $season->packs = $packs;
                    Storage::disk('public')->put('assets/images/game_data/game_data.json',json_encode($season));

                    //zip the images under assets/images/game_data folder
                    $this->ZipFolder(public_path('assets/images/game_data'),storage_path('app/game_data'));
                    Storage::disk('public')->delete('assets/images/game_data/game_data.json');
                }else{
                    $response['status'] = 'Season already activated';
                }
            }else{
                $response['status'] = 'Season Id not valid';
            }
        }else{
            $response['status'] = 'Missing parameter id';
        }
        return json_encode($response);
    }
    private function ZipFolder($folderPath,$zipFileName){
        // Get real path for our folder
        $rootPath = realpath($folderPath);

        // Initialize archive object
        try{
            $zip = new \ZipArchive();
        }catch(\Exception $e){
            error_log("ZipFolder".$e->getMessage());
        }
        $zip->open($zipFileName.'.zip', \ZipArchive::CREATE | \ZipArchive::OVERWRITE);

        // Create recursive directory iterator
        /** @var SplFileInfo[] $files */
        $files = new \RecursiveIteratorIterator(
            new \RecursiveDirectoryIterator($rootPath),
            \RecursiveIteratorIterator::LEAVES_ONLY
        );
        foreach ($files as $name => $file)
        {
            // Skip directories (they would be added automatically)
            if (!$file->isDir())
            {
                // Get real and relative path for current file
                $filePath = $file->getRealPath();
                $relativePath = substr($filePath, strlen($rootPath) + 1);

                // Add current file to archive
                $zip->addFile($filePath, $relativePath);
            }
        }
        // Zip archive will be created only after closing object
        $zip->close();
    }
    public function CreateOnluckMetadata(){
        $response = array();
        $response['status']="OK";
        $metadata = new \stdClass();
        $metadata->active_season = 0;
        $metadata->pointed_season = 0;
        $metadata->season_uptodate_token = 0;
        $metadata->activation_code = 0;
        $metadata->uptodate_token = 0;
        $metadata->quote = "";
        $metadata->intro_content = "";
        $metadata->guideline_content = "";
        Storage::disk('local')->put('onluck.json',json_encode($metadata));
        try{
            $response['data'] = json_decode(Storage::disk('local')->get('onluck.json'));
        }catch(\Exception $e){
            $response['status']=$e->getMessage();
        }
        return json_encode($response);
    }
    public function GetOnluckMetadata(){
        $response = array();
        $response['status']="OK";
        try{
            $response['data'] = json_decode(Storage::disk('local')->get('onluck.json'));
        }catch(\Exception $e){
            $response['status']=$e->getMessage();
        }
        return json_encode($response);
    }

    public function NotifyServerOnGameStart(Request $request){
        $response = array();
        $response['status']="OK";
        //1. Get metadata
        $metadata = null;
        $userData = null;
        $playing_data_uptodate_token = -1;

        try{
            $metadata = json_decode(Storage::disk('local')->get('onluck.json'));
            if($request->has("metadata_uptodate_token")){
                if($request->metadata_uptodate_token==$metadata->uptodate_token){
                    unset($request->quote);
                    unset($request->intro_content);
                    unset($request->guideline_content);
                }
            }
        }catch(\Exception $e){
            $response['status']=$e->getMessage();
        }

        if($request->has('user_id')){
            $user = User::find($request->get('user_id'));
            if($user!=null){
                $vendor_name = $request->has('vendor_name')?$request->vendor_name:$user->active_vendor;
                $vendor = AuthVendor::where([['user_id','=',$user->id],['vendor_name','=',$vendor_name]])->first();
                if($vendor!=null){
                    $userData = [
                        'id'=>$user->id,
                        'uptodate_token'=>$user->uptodate_token,
                        'name'=>$vendor->user_name,
                        'email'=>$user->email,
                        'active_vendor'=>$vendor_name,
                        'profile_picture'=>$vendor->profile_picture,
                    ];
                }

                //3.get playingdata uptodate_token
                try{
                    $playingData = PlayingData::select('uptodate_token')->where('user_id',$request->user_id)->first();
                    error_log('playingData->uptodate_token'.$playingData->uptodate_token);
                    if($playingData!=null){
                        $playing_data_uptodate_token = $playingData->uptodate_token;
                    }
                }catch(\Exception $e){
                    // $response['status']=$e->getMessage();
                }
            }else{
                $response['status']="User with id $request->user_id not found";
                $userData = new \stdClass();
                $userData->uptodate_token = -1;
            }
        }else{
            $response['status']='Missing parameter user_id';
        }
        $response['data']=[
            'user_data'=>$userData,
            'metadata'=>$metadata,
            'playing_data_uptodate_token'=>$playing_data_uptodate_token
        ];
        return json_encode($response);
    }
    public function CheckUptodateUserData(Request $request){
        $response = array();
        $response['status']="OK";
        
        if($request->has('id')){
            if($request->has('uptodate_token')){
                $user = User::find($request->get('id'));
                if($user!=null){
                    if($user->uptodate_token != $request->uptodate_token){
                        // $response['status']="not_uptodate";

                        unset($user->created_at);
                        unset($user->updated_at);
                        $userData = $user;
                    }
                }else{
                    $response['status']="User with id $user->id not found";
                }
            }else{
                $response['status']='Missing parameter uptodate_token';
            }
        }else{
            $response['status']='Missing parameter id';
        }
        return json_encode($response);
    }

    public function StoreMetadata(Request $request){
        $response = array();
        $response['status']="OK";
        if(
            $request->has('new_quote')||
            $request->has('intro_content')||
            $request->has('guideline_content'))
        {
            try{
                $metadata = json_decode(Storage::disk('local')->get('onluck.json'));
                if($request->has('new_quote'))
                    $metadata->quote = $request->new_quote;
                if($request->has('intro_content'))
                    $metadata->intro_content = $request->intro_content;
                if($request->has('guideline_content'))
                    $metadata->guideline_content = $request->guideline_content;
                $metadata->uptodate_token = time();
                Storage::disk('local')->put('onluck.json',json_encode($metadata));
            }catch(\Exception $e){
                $response['status']=$e->getMessage();
            }
        }else{
            $response['status']="Missing parameter new_quote|intro_content|guideline_content";
        }
        return json_encode($response);
    }
    public function CreateSeason(Request $request){
        $response = array();
        $response['status']="OK";

        if($request->has("name")){
            if($request->has("from")){
                if($request->has("to")){

                    $season = new Season();
                    $season->name = $request->name;
                    try{
                        $season->from = new \DateTime();//Carbon::createFromFormat('d-m-Y H:i:s', '20-11-2020 10:0:0')->format('Y-m-d H:i:s');
                        $season->to = new \DateTime();//Carbon::createFromFormat('d-m-Y H:i:s', '20-11-2020 10:0:0')->format('Y-m-d H:i:s');
                    }catch(\Exception $e){
                        error_log("Error: ".$e->getMessage());
                    }
                    $season->save();
                    $response['data'] = Season::all();
                }else{
                    $response['status']='Missing parameter to';
                }
            }else{
                $response['status']='Missing parameter from';
            }
        }else{
            $response['status']='Missing parameter name';
        }

        return json_encode($response);
    }
    public function UpdateSeason(Request $request){
        $response = array();
        $response['status']="OK";

        if($request->has("id")){
            if($request->has("name")){
                if($request->has("from")){
                    if($request->has("to")){
                        $season = Season::find($request->id);
                        $season->name = $request->name;
                        try{
                            $season->from = $request->from;
                            $season->to = $request->to;
                        }catch(\Exception $e){
                            error_log("Error: ".$e->getMessage());
                        }
                        $season->save();
                        $this->generateNewSeasonUptodateToken($season->id);
                        $response['data'] = Season::all();
                    }else{
                        $response['status']='Missing parameter to';
                    }
                }else{
                    $response['status']='Missing parameter from';
                }
            }else{
                    $response['status']='Missing parameter name';
            }
        }else{
            $response['status']='Missing parameter id';
        }

        return json_encode($response);
    }
    private function saveImageToStorage($image,$name,$relative_path=""){
        $name = $name.'.'.$image->getClientOriginalExtension();
        $destinationPath = public_path('/assets/images'.$relative_path);
        $image->move($destinationPath, $name);
        error_log("Saved image ".$destinationPath."/".$name);
        return "/assets/images".$relative_path."/".$name;
    }
    public function CreatePack(Request $request){
        $response = array();
        $response['status']="OK";

        if($request->has("season_id")){
            if($request->has("title")){
                if($request->has("question_type")){
                    $pack = new Pack();
                    $pack->season_id = $request->season_id;
                    $pack->title = $request->title;
                    $pack->sub_text = $request->sub_text;
                    $pack->icon = $request->has('icon')
                        ?$this->saveImageToStorage($request->file('icon'),"icon_".time(),"/game_data")
                        :$this->DEFAULT_PROFILE_PICTURE;
                    $pack->question_type = $request->question_type;
                    $pack->save();

                    $packs = Pack::where('season_id',$request->season_id)->get();
                    if($packs!=null)
                        $response["data"] = $packs->values();

                    $this->generateNewSeasonUptodateToken($request->season_id);
                }else{
                    $response['status']='Missing parameter question_type';
                }
            }else{
                $response['status']='Missing parameter title';
            }
        }else{
            $response['status']='Missing parameter season_id';
        }

        return json_encode($response);
    }
    public function UpdatePack(Request $request){
        $response = array();
        $response['status']="OK";

        if($request->has("id")){
            if($request->has("title")){
                $pack = Pack::find($request->id);
                $pack->title = $request->title;
                $pack->sub_text = $request->sub_text;
                if($request->has('icon')){
                    try{
                        unlink(public_path($pack->icon));
                    }catch(\Exception $e){error_log($e->getMessage());}
                    $pack->icon = $this->saveImageToStorage($request->file('icon'),"icon_".time(),"/game_data");
                }
                $pack->save();

                $packs = Pack::where('season_id',$pack->season_id)->get();
                if($packs!=null)
                    $response["data"] = $packs->values();

                $this->generateNewSeasonUptodateToken($pack->season_id);
            }else{
                $response['status']='Missing parameter title';
            }
        }else{
            $response['status']='Missing parameter id';
        }

        return json_encode($response);
    }
    public function GetPacks(Request $request){
        $response = array();
        $response['status']="OK";
        try{
            $packs = Pack::where('season_id',$request->season_id)->get();
            if($packs!=null)
                $response["data"] = $packs->values();
        }catch(\Exception $e){
            $response['status']=$e->getMessage();
        }
        return json_encode($response);
    }

    public function CreateQuestion(Request $request){
        $response = array();
        $response['status']="OK";
        if($request->has("pack_id")){
            if($request->has("question_type")){
                if($request->has("question")){
                    if($request->has("answer")){

                        $imagesPath = "";
                        if($request->has("images")){
                            $images = array();
                            foreach($request->images as $key=>$image){
                                array_push($images,$this->saveImageToStorage($image,"question_image_".time().$key,"/game_data"));
                            }
                            $imagesPath = json_encode($images);
                        }    

                        $status = false;
                        if($request->question_type==0){
                            $question = new TypedQuestion();
                            $question->pack_id = $request->pack_id;
                            $question->question = $request->question;
                            $question->answer = $request->answer;
                            $question->score = $request->score;
                            $question->images = $imagesPath;//$request->images;
                            $question->hints = $request->hints;
                            $question->save();
                            $status = true;

                            $response['data'] = TypedQuestion::where('pack_id',$request->pack_id)->get();
                        }else if($request->question_type==1){
                            $question = new McqQuestion();
                            $question->pack_id = $request->pack_id;
                            $question->question = $request->question;
                            $question->choices = $request->choices;
                            $question->answer = $request->answer;
                            $question->time =$request->time;
                            $question->score = $request->score;
                            $question->images = $imagesPath;
                            $question->hints = $request->hints;
                            $question->save();
                            $status = true;

                            $response['data'] = McqQuestion::where('pack_id',$request->pack_id)->get();
                        }else{
                            $response['status']="Wrong question_type";
                        }
                        if($status){
                            $this->generateNewSeasonUptodateToken(Pack::find($request->pack_id)->season_id);
                        }
                    }else{
                        $response['status']="Missing parameter answer";
                    }
                }else{
                    $response['status']="Missing parameter question";
                }
            }else{
                $response['status']="Missing parameter question_type";
            }
        }else{
            $response['status']="Missing parameter pack_id";
        }
        error_log($request->question_type);
        error_log($request->question);
        error_log($request->hints);
        return json_encode($response);
    }

    
    public function UpdateQuestion(Request $request){
        $response = array();
        $response['status']="OK";
        if($request->has("id")){
            if($request->has("question_type")){
                if($request->has("question")){
                    if($request->has("answer")){


                        $images = array();
                        if($request->has("images")){
                            foreach($request->images as $key=>$image){
                                error_log("save here");
                                array_push($images,$this->saveImageToStorage($image,"question_image_".time().$key,"/game_data"));
                            }
                        }    
                        if($request->question_type==0){
                            $question = TypedQuestion::find($request->id);
                            // $question->pack_id = $request->pack_id;
                            $question->question = $request->question;
                            $question->answer = $request->answer;
                            $question->score = $request->score;

                            $existingImages = json_decode($question->images);
                            if($request->has('tobedeleted_images')){
                                $tobeDeletedImageIndices = json_decode($request->tobedeleted_images);
                                foreach($tobeDeletedImageIndices as $index){
                                    if($existingImages[$index]!=null){
                                        try{
                                            unlink(public_path($existingImages[$index]));
                                        }catch(\Exception $e){
                                            error_log($e->getMessage());
                                        }
                                        unset($existingImages[$index]);
                                    }
                                }
                            }
                            $existingImages = array_merge($existingImages,$images);
                            $question->images = json_encode($existingImages);//$imagesPath;//$request->images;
                            $question->hints = $request->hints;
                            $question->save();
                            $this->generateNewSeasonUptodateToken(Pack::find($question->pack_id)->season_id);

                            $response['data'] = TypedQuestion::where('pack_id',$question->pack_id)->get();
                        }else if($request->question_type==1){
                            $question = McqQuestion::find($request->id);
                            $question->question = $request->question;
                            $question->choices = $request->choices;
                            $question->answer = $request->answer;
                            $question->time =$request->time;
                            $question->score = $request->score;

                            $existingImages = json_decode($question->images);
                            if($request->has('tobedeleted_images')){
                                $tobeDeletedImageIndices = json_decode($request->tobedeleted_images);
                                foreach($tobeDeletedImageIndices as $index){
                                    if($existingImages[$index]!=null){
                                        try{
                                            unlink(public_path($existingImages[$index]));
                                        }catch(\Exception $e){
                                            error_log($e->getMessage());
                                        }
                                        unset($existingImages[$index]);
                                    }
                                }
                            }
                            $existingImages = array_merge($existingImages,$images);
                            $question->images = json_encode($existingImages);//$imagesPath;//$request->images;

                            $question->hints = $request->hints;
                            $question->save();
                            $this->generateNewSeasonUptodateToken(Pack::find($question->pack_id)->season_id);
                            $response['data'] = McqQuestion::where('pack_id',$question->pack_id)->get();
                        }else{
                            $response['status']="Wrong question_type";
                        }

                    }else{
                        $response['status']="Missing parameter answer";
                    }
                }else{
                    $response['status']="Missing parameter question";
                }
            }else{
                $response['status']="Missing parameter question_type";
            }
        }else{
            $response['status']="Missing parameter id";
        }
        error_log($request->question_type);
        error_log($request->question);
        error_log($request->hints);
        return json_encode($response);
    }
    public function GetQuestions(Request $request){
        $response = array();
        $response['status']="OK";
        try{
            error_log($request->pack_id);
            if($request->question_type==0){
                $questions = TypedQuestion::where('pack_id',$request->pack_id)->get();
                if($questions!=null)
                    $response["data"] = $questions->values();
            }else if($request->question_type==1){
                $questions = McqQuestion::where('pack_id',$request->pack_id)->get();
                if($questions!=null)
                    $response["data"] = $questions->values();
            }
        }catch(\Exception $e){
            $response['status']=$e->getMessage();
        }
        return json_encode($response);
    }
    public function DeleteSeason(Request $request){
        $response = array();
        $response['status']="OK";
        if($request->has("id")){
            try{
                $season = Season::find($request->id);
                $packs = Pack::where('season_id',$season->id)->get();
                foreach($packs as $pack){
                    if($pack->question_type=="0"){
                        foreach(TypedQuestion::where('pack_id',$pack->id)->get() as $question){
                            $images = json_decode($question->images);
                            foreach($images as $image){
                                try{
                                    unlink(public_path($image));
                                }catch(\Exception $e){}
                            }                        
                            $question->delete();
                        }
                    }else if($pack->question_type=="1"){
                        foreach(McqQuestion::where('pack_id',$pack->id)->get() as $question){
                            $images = json_decode($question->images);
                            foreach($images as $image){
                                try{
                                    unlink(public_path($image));
                                }catch(\Exception $e){}
                            }                      
                            $question->delete();
                        }
                    }
                    try{
                        unlink(public_path($pack->icon));
                    }catch(\Exception $e){}
                    $pack->delete();
                }
                $metadata = json_decode(Storage::disk('local')->get('onluck.json'));
                if($metadata->active_season==$season->id){
                    $metadata->activation_code = 0;
                    $metadata->active_season = -1;
                    Storage::disk('local')->put('onluck.json',json_encode($metadata));
                }

                $season->delete();
                $response['data'] = Season::all();
            }catch(\Exception $e){
                $response['status']=$e->getMessage();
            }
        }
        return json_encode($response);
    }
    public function DeletePack(Request $request){
        $response = array();
        $response['status']="OK";
        if($request->has("id")){
            try{
                $pack = Pack::find($request->id);
                if($pack->question_type=="0"){
                    foreach(TypedQuestion::where('pack_id',$pack->id)->get() as $question){
                        $images = json_decode($question->images);
                        foreach($images as $image){
                            try{
                                unlink(public_path($image));
                            }catch(\Exception $e){}
                        }
                        $question->delete();
                    }
                }else if($pack->question_type=="1"){
                    foreach(McqQuestion::where('pack_id',$pack->id)->get() as $question){
                        $images = json_decode($question->images);
                        foreach($images as $image){
                            try{
                                unlink(public_path($image));
                            }catch(\Exception $e){}
                        }                      
                        $question->delete();
                    }
                }
                try{
                    unlink(public_path($pack->icon));
                }catch(\Exception $e){}
                $season_id = $pack->season_id;
                $pack->delete();
                $response['data'] = Pack::where('season_id',$season_id)->get()->values();
            }catch(\Exception $e){
                $response['status']=$e->getMessage();
            }
        }
        return json_encode($response);
    }
    public function DeleteQuestion(Request $request){
        $response = array();
        $response['status']="OK";
        if($request->has("id")){
            if($request->has("pack_id")){
                $pack = Pack::find($request->pack_id);
                try{
                    if($pack->question_type=="0"){
                        $question = TypedQuestion::find($request->id);
                        $images = json_decode($question->images);
                        foreach($images as $image){
                            try{
                                unlink(public_path($image));
                            }catch(\Exception $e){}
                        }                        
                        $question->delete();
                        $response['data'] = TypedQuestion::where('pack_id',$request->pack_id)->get();
                    }else if($pack->question_type=="1"){
                        $question = McqQuestion::find($request->id);
                        $images = json_decode($question->images);
                        foreach($images as $image){
                            try{
                                unlink(public_path($image));
                            }catch(\Exception $e){}
                        }                      
                        $question->delete();
                        $response['data'] = McqQuestion::where('pack_id',$request->pack_id)->get();
                    }else{
                        $response['status']="Invalid question_type";
                    }
                }catch(\Exception $e){
                    $response['status']=$e->getMessage();
                }
            }
        }
        return json_encode($response);
    }
    public function DownloadGameData(){
        return response()->download(storage_path('app/game_data.zip'));
        // $response = array();
        // $response['status']="OK";

        // try{
        //     $response['data']= json_decode(Storage::disk('local')->get('game_data.json'));
        // }catch(\Exception $e){
        //     $response['status'] = $e->getMessage();
        // }

        // $metadata = $this->getMetadata();
        // if($metadata->active_season != 0){
            // $season = Season::find($metadata->active_season);
            // unset($season->created_at);
            // unset($season->updated_at);
            // $packs = Pack::select(['id','season_id','title','sub_text','icon','question_type'])
            //     ->where('season_id',$season->id)->get();
            // foreach ($packs as $pack){
            //     if($pack->question_type=="0"){
            //         $pack->typed_questions = TypedQuestion::where('pack_id',$pack->id)->get();
            //     }else if($pack->question_type=="1"){
            //         $pack->mcq_questions = McqQuestion::where('pack_id',$pack->id)->get();
            //     }
            // }
            // $season->packs = $packs;
        //      = $season;
        // }else{
        //     $response['status']="Season not found";
        // }
    }

    public function GetPlayingData(Request $request){
        $response = array();
        $response['status']="OK";
        if($request->has("user_id")){
            try{
                $season_id = $this->getMetadata()->active_season;
                $packs = Pack::where('season_id',$season_id)->pluck('id');
                $playingData = PlayingData::select(['id','user_id','total_score','uptodate_token'])->where('user_id',$request->user_id)->first();
                if($playingData!=null){
                    $playingData->current_question_indices = CurrentQuestionIndex::where('playing_data_id',$playingData->id)->whereIn('pack_id',$packs)->pluck('index');
                    $playingData->playing_questions = 
                        QuestionPlayingData::select(['id','playing_data_id','season_id','pack_id','question_id','status','started','ended','used_hint_count','score'])
                            ->where([['playing_data_id','=',$playingData->id],['season_id','=',$season_id]])->get();
                }else{
                    $playingData = new PlayingData();
                    $playingData->user_id = $request->user_id;
                    $playingData->total_score = 0;
                    $playingData->uptodate_token = time();
                    $playingData->save();
                    unset($playingData->created_at);
                    unset($playingData->updated_at);
                    $playingData->playing_questions = [];
                    $playingData->current_question_indices = [];
                }
                $response['data'] = $playingData;
            }catch(\Exception $e){
                $response['status']=$e->getMessage();
            }
        }else{
            $response['status']="Missing parameter user_id";
        }
        return json_encode($response);
    }

    public function StorePlayingData(Request $request){
        $startTime = round(microtime(true) * 1000);
        $response = array();
        $response['status']="OK";
        if($request->has("playing_data_id")){
            if($request->has("total_score")){
                if($request->has("modified_questions")){
                    $playingData = PlayingData::find($request->playing_data_id);
                    if($playingData){
                        $playingData->total_score = $request->total_score;
                        $playingData->uptodate_token = time();
                        $playingData->save();
                        $response['data']=$playingData->uptodate_token;
                    }
                    $index = CurrentQuestionIndex::where(
                        [['pack_id','=',$request->pack_id],['playing_data_id','=',$request->playing_data_id]])->first();
                    if($index==null){
                        //new index
                        $index = new CurrentQuestionIndex();
                        $index->pack_id = $request->pack_id;
                        $index->playing_data_id = $request->playing_data_id;
                    }
                    $index->index = $request->index;
                    $index->save();
                    foreach($request->modified_questions as $modified_question){
                        if($modified_question['id']==0){
                        //new playing question
                            $playingQuestion = new QuestionPlayingData();
                        }else{
                            $playingQuestion = QuestionPlayingData::find($modified_question['id']);
                        }
                        $playingQuestion->playing_data_id = $request->playing_data_id;
                        $playingQuestion->season_id = $this->getMetadata()->active_season;
                        $playingQuestion->pack_id = $request->pack_id;
                        $playingQuestion->question_id = $modified_question['question_id'];
                        $playingQuestion->status = $modified_question['status'];
                        $playingQuestion->started = $modified_question['started'];
                        $playingQuestion->ended = $modified_question['ended'];
                        $playingQuestion->used_hint_count = $modified_question['used_hint_count'];
                        $playingQuestion->score = $modified_question['score'];
                        $playingQuestion->save();
                    }
                }else{
                    $response['status']='Missing parameter modified_questions';
                }
            }else{
                $response['status']='Missing parameter total_score';
            }
        }else{
            $response['status']='Missing parameter playing_data_id';
        }
        error_log("StorePlayingData:Execution Time: ".(round(microtime(true) * 1000)-$startTime));
        return json_encode($response);
    }
}
