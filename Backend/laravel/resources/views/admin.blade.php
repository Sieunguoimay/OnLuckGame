<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">

    <title>Onluck Admin</title>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css" integrity="sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk" crossorigin="anonymous">
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js" integrity="sha384-OgVRvuATP1z7JjHLkuOU7Xw704+h835Lr+6QL9UvYjZE3Ipu6Tp75j7Bh/kR0JKI" crossorigin="anonymous"></script>

</head>
<body>


    <div class="container">
        <div class="h2 text-center m-3">ONLUCK ADMIN</div>
        <div class="h4 row">All users (<div id="user_count" class="d-inline"></div>)</div>
        <div class="row">
            <div class="col-md-6 overflow-auto bg-light" id="user_panel" style="max-height:400px;">
            </div>
            <div class="col-md-6">
            </div>
        </div>
    </div>

    <div id="popup" style="display:none">
        <div class="btn btn-danger" id="btn_popup_delete">Delete</div>
    </div>


<script>

var Client = {
    baseUrl:'/api/onluck',
    userList:null,
    $userListElement:$('#user_panel')
};
Client.DisplayUserList = function(){
    // console.log(response);
    var html = '';
    // for(const user of Client.userList){
    for(var index = 0; index<Client.userList.length; index++){
        var user = Client.userList[index];
        html+=
        '<div class="row my-1 user-item" index="'+index+'">'+
        '<div class="col-1 p-0 my-auto"><img src="'+user.profile_picture+'" style="width:100%"/></div>'+
        '<div class="col-4 pl-1 pr-0 my-auto h6">'+user.name+'</div>'+
        '<div class="col-5 p-0 my-auto">'+user.email+'</div>'+
        '<div class="col-1 p-0 my-auto">'+user.score+'</div>'+
        '<div class="col-1 p-0 my-auto">'+user.verification_code+'</div>'+
        '</div>';
    }

    Client.$userListElement.html(html);
}
Client.Init = function(){
    // This is where the admin begins
    $.get(Client.baseUrl+'/getusers',function(response){
        var json = JSON.parse(response);
        if(json.status == "OK"){
            $('#user_count').text(json.data.length);

            Client.userList = json.data;

            Client.DisplayUserList();

            $('.user-item').on('contextmenu',function(event){
                console.log("OK");
                
                Popup.Show(event.pageX,event.pageY,$(this));

                return false;
            });
        }
    });

}
Client.Init();
Client.DeleteUserByIndex = function(index){
    Client.DeleteUser(Client.userList[index].id);
}
Client.DeleteUser = function(id){
    $.get(Client.baseUrl+'/deleteuser?id='+id,function(response){
        console.log(response);
        var json = JSON.parse(response);
        if(json.status=="OK"){
            Client.userList.splice(Client.userList.findIndex(user => user.id === id),1);
            Client.DisplayUserList();
        }
    });
}





var Popup = {
    $element:$('#popup'),
    $baseElement:null
};
Popup.Init = function(){
    $('#btn_popup_delete').on('click',function(){
        console.log("Hello");
        Client.DeleteUserByIndex(Popup.$baseElement.attr('index'));
    });
}
Popup.Init();

Popup.Show = function(posX, posY, $element){
    if(Popup.$baseElement!=null){
        Popup.Hide();
    }
    Popup.$element.css('left',posX);      // <<< use pageX and pageY

    if(Popup.$baseElement!=$element){
        Popup.$element.css('top',$element.offset().top+$element.height()/2);
        Popup.$element.css('display','inline');     
        Popup.$element.css("position", "absolute");  // <<< also make it absolute!
        $element.css('background-color','#f0f0f0');
        Popup.$baseElement = $element;
    }
}
Popup.Hide = function(){
    Popup.$element.css('display','none');
    if(Popup.$baseElement!=null)        
        Popup.$baseElement.css('background-color','#ffffff00');
}



$(document).on('click',function(){
    console.log('global click');
    Popup.Hide();
    return true;
});



</script>
</body>
</html>