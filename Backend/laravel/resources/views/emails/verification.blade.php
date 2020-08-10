@component('mail::message')
# Verification

Hi {{$user->name}},

Thanks for signing up on Onluck!<br>

To complete your registration, please verify your email address by clicking on the button below:<br>

@component('mail::button', 
    ['url' => 
        env('APP_URL', 'onluckthegame.com')
        .'/api/onluck/verifyemail?verification_code='.$user->verification_code
        .'&email='.$user->email
    ])
Verify
@endcomponent

If the above link doesn't work, please copy and paste the following link into your browser address bar:<br>
<a href="{{env('APP_URL', 'onluckthegame.com').'/api/verifyemail?verification_code='.$user->verification_code
    .'&email='.$user->email}}">{{env('APP_URL', 'onluckthegame.com').'/api/verifyemail?verification_code='.$user->verification_code
    .'&email='.$user->email}}</a>

<br>
Thanks,<br>
{{ config('app.name') }}
@endcomponent
