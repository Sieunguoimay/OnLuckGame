<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\User;
use App\AuthVendor;
use App\Mail\VerificationMail;
use Illuminate\Support\Facades\Mail;
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

                $user->verification_code = 0;
                $user->last_active_vendor_name = $vendorName;
                $user->save();

                $response["data"]=[
                    'user_id'=>$user->id,
                    'score'=>$user->score,
                    'active_vendor'=>$user->last_active_vendor_name,
                    'has_profile_picture'=>!($vendor==null)
                ];

                if($vendor == null){
                    // $profilePicturePath = null;
                    // if($request->hasFile("profile_picture")){
                    //     $image = $request->file('profile_picture');
                    //     $name = "profile_picture_".$user->email.'.'.$image->getClientOriginalExtension();
                    //     $destinationPath = public_path('/assets/images');
                    //     $image->move($destinationPath, $name);
                    //     error_log("Saved image $name to $destinationPath");
                    //     $profilePicturePath = "/assets/images/".$name;
                    // }
                    $vendor = new AuthVendor();
                    $vendor->vendor_name = $vendorName;
                    $vendor->user_name = $request->has("name")?$request->get("name"):"No Name";
                    $vendor->profile_picture = $this->DEFAULT_PROFILE_PICTURE;//$request->has("profile_picture")?$profilePicturePath:
                    $vendor->user_id = $user->id;
                    $vendor->save();
                }else{
                    $response['data']['profile_picture']=$vendor->profile_picture;
                }
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
                $user->verification_code = rand(10000,99999);
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
            $user->save();

            $vendor = new AuthVendor();
            $vendor->user_name = $userName;
            $vendor->vendor_name = $this->DEFAULT_VENDOR_NAME;
            $vendor->user_id = $user->id;
            $vendor->profile_picture=$this->DEFAULT_PROFILE_PICTURE;
            $vendor->save();

            $response['data']=[
                'user_id'=>$user->id,
                'user_name'=>$vendor->user_name,
                'active_vendor'=>$user->last_active_vendor_name,
                'profile_picture'=>$vendor->profile_picture,
                'score'=>$user->score];
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
        $user = User::where([["email",'=',$email],["password",'=',$password]])->first();
        if($user!=null){
            //Your life is safe
            $user->last_active_vendor_name = $this->DEFAULT_VENDOR_NAME;
            $vendor = AuthVendor::where([['user_id','=',$user->id],['vendor_name','=',$user->last_active_vendor_name]])->first();
            if($vendor!=null){
                $response['data']=[
                    'user_id'=>$user->id,
                    'user_name'=>$vendor->user_name,
                    'active_vendor'=>$user->last_active_vendor_name,
                    'profile_picture'=>$vendor->profile_picture,
                    'score'=>$user->score];
                if($user->verification_code > 0){
                    $response['status']="email_not_verified";
                    //pls jump to the verification section
                }
                $user->save();
            }else{
                $response['status']="You has not signed up with this email yet";
            }
        }else{
            $response['status']="Email or password is wrong";
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

    public function GetUsers(){
        $response = array();
        $response['status']="OK";
        
        $users = User::all();
        $response["data"] = $users;

        foreach($users as $user){
            $vendor = AuthVendor::where([['user_id','=',$user->id],['vendor_name','=',$user->last_active_vendor_name]])->first();
            $user['profile_picture'] = $vendor->profile_picture;
            $user['name'] = $vendor->user_name;
            unset($user['password']);
        }

        return json_encode($response);
    }
    public function DeleteUser(Request $request){
        $response = array();
        $response['status']="OK";
        if($request->has('id')){
            $userId = $request->query('id');
            $user = User::find($userId);
            if($user!=null){
                AuthVendor::where('user_id','=',$userId)->delete();
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
                        $response['data']=['profile_picture'=>$imagePath];
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
            $image = $request->file('profile_picture');
            $name = "profile_picture_".$email.'.'.$image->getClientOriginalExtension();
            $destinationPath = public_path('/assets/images');
            $image->move($destinationPath, $name);
            error_log("Saved image $name to $destinationPath");
            return "/assets/images/".$name;
        }
        return null;
    }
    public function GetScoreboard(){

        $response = array();
        $response['status']="OK";
        
        $users = User::all()->sortByDesc("score")->values();
        $response["data"] = $users;

        foreach($users as $user){
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
        return json_encode($response);
    }
}
