<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;

class PageController extends Controller
{
    function Admin(Request $request){
        $response = array();
        $response['status'] = "OK";
        if($request->has('email')){
            if($request->has('password')){
                if($request->email=="admin@onluck.com"&&$request->password=="vuduydu"){
                    return view('admin');
                }else{
                    $response['status']="Incorrect email or password";
                }
            }else{
                $response['status']="Missing Parameter password";
            }
        }else{
            $response['status']="Missing Parameter email";
        }
        return redirect('login')->with('status',$response['status']);
    }

}
