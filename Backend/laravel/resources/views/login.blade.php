<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <title>Login to Onluck Admin</title>
    <style>
        .error-message{
            color:orangered;
            font-style:italic; 
        }
        html,body{
            height:100%;
            margin:0px;
        }
    </style>
    
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css" integrity="sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk" crossorigin="anonymous">
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js" integrity="sha384-OgVRvuATP1z7JjHLkuOU7Xw704+h835Lr+6QL9UvYjZE3Ipu6Tp75j7Bh/kR0JKI" crossorigin="anonymous"></script>

</head>
<body>
    <div class="container h-100">
        <div class="row justify-content-center align-items-center h-100">
            <div class="col-lg-4 col-md-6 col-sm-8 col-xs-11 ">
                <div class="row justify-content-center py-5 text-center"><h2>LOGIN TO ONLUCK ADMIN</h2></div>
                @if(session('status'))<div class="error-message">{{session('status')??''}}</div>@endif
                <form action="/admin" class="form-group">
                    <label for="email"><small>Email</small></label>
                    <input type="email" name="email" id="email" class="form-control mb-2">
                    <label for="password"><small>Password</small></label>
                    <input type="password" name="password" id="password" class="form-control">
                    <input type="submit" name="Login" class="form-control my-4 btn btn-primary">
                </form>
            </div>
        </div>
    </div>
</body>
</html>