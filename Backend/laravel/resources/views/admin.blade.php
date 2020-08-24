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
    <script src="https://kit.fontawesome.com/a076d05399.js"></script>
    <style>
        body{
            padding-left:20px;
            padding-right:20px;
            background-color:#eeeeee;
        }
        .item-selected{
            background-color:#dddddd;
        }
        .active-season{
            border-left: 8px solid green;
        }
        .pointed-season{
            border-left: 8px solid yellow;
        }
        .animate-rotate{
            transition: all 0.75s 0.25s;
            transform: rotate(360deg);
        }
        .creator-reposition{
            position: absolute;
            top:10px;
        }
        .panel-color{
            background-color:#fdfdfd;
        }
    </style>
</head>
<body>


    <div class="container-fluid mt-2">
        <div class="row justify-content-md-center">
            <div class="col-md-6 col-sm-10 py-2 mb-3 border-left panel-color">
                <div class="row mb-5 mt-2">
                    <div class="col-2"></div>
                    <div class="col-8 h2 text-center my-auto">ONLUCK ADMIN</div>
                    <div class="col-2 my-auto"><a href="/login" class="text-decoration-none" style="color:orangered;font-weight:bold">Logout</a></div>
                </div>
                <div class="h4">Quote of admin</div>
                <div id="quote"></div>
                <div class="row">
                    <div class="m-2" style="cursor:pointer;color:orangered;font-weight:bold" id="intro_button">Introduction</div>
                    <div class="m-2" style="cursor:pointer;color:orangered;font-weight:bold" id="guideline_button">Guideline</div>
                </div>
                <div id="central_panel"></div>
            </div>
        </div>
    </div>
    <div class="container-fluid">
        <div class="row px-4 justify-content-center mb-0 ">
            <span class="col-md-10 col-sm-12 py-2 panel-color">
                <span class="h4">
                All users (<span id="user_count"></span>) </span>
                <i class="fas fa-redo" id="refresh_user_list_button" style="cursor:pointer"></i>
            </span>
        </div>
        <div class="row px-4 mt-0 justify-content-center">
            <div class="col-md-5 col-sm-12 mb-3 overflow-auto panel-color" id="user_list_panel" style="max-height:400px;"></div>
            <div class="col-md-5 col-sm-12 mb-3 overflow-auto panel-color" id="user_info_panel" style="max-height:400px;"></div>
        </div>
    </div>
    <div class="container-fluid panel-color">
        <div class="row px-1">
            <div class="col-md-4 col-sm-12 px-1 panel-color" >
                <div class="row justify-content-center my-2">
                    <span class="h4 mb-0">Seasons (<span id="season_count" class="d-inline"></span>)</span>
                    <i class="fas fa-redo  my-auto" id="refresh_season_list_button" style="cursor:pointer"></i>
                </div>
    
                <div class="row m-0 p-0"><div class="container-fluid pl-0 overflow-auto" id="season_panel" style="max-height:600px;"></div></div>
                <div class="row m-0 py-1 px-md-2 px-sm-5" id="season_creator_panel"></div>
            </div>
            <div class="col-md-4 col-sm-12 px-1 panel-color" >
                <div class="row justify-content-center my-2 h4">Packs (<div id="pack_count" class="d-inline"></div>)</div>
                <div class="row m-0 p-0"><div class="container pl-0 overflow-auto" id="pack_panel" style="max-height:600px;"></div></div>
                <div class="row m-0 py-1 px-md-2 px-sm-5" id="pack_creator_panel"></div>
            </div>
            <div class="col-md-4 col-sm-12 px-1 panel-color">
                <div class="row justify-content-center my-2 h4">Questions (<div id="question_count" class="d-inline"></div>)</div>
                <div class="row m-0 p-0"><div class="container pl-0 overflow-auto" id="question_panel" style="max-height:600px;"></div></div>
                <div class="row m-0 py-1 px-md-2 px-sm-5" id="question_creator_panel"></div>
            </div>
        </div>
    </div>

    <div id="popup" style="display:none">
        <div class="btn btn-danger" id="btn_popup_delete">Delete</div>
    </div>
    <div class="row m-0 pt-4" style="height:200px">
    </div>

<script>

var Client = {
    baseUrl:'/api/onluck',
    userList:null,
    seasonList:null,
    packList:null,
    questionList:null,
    onluckMetadata:null,
    $userListElement:$('#user_list_panel'),
    $seasonListElement:$('#season_panel'),
    $packListElement:$('#pack_panel'),
    $questionListElement:$('#question_panel'),
    $seasonCreatorElement:$('#season_creator_panel'),
    $packCreatorElement:$('#pack_creator_panel'),
    $questionCreatorElement:$('#question_creator_panel')
};
Client.DisplayUserList = function(){
    // //console.log(response);
    // for(const user of Client.userList){
    $('#user_count').text(Client.userList.length);
    var html = '';
    for(var index = 0; index<Client.userList.length; index++){
        var user = Client.userList[index];
        html+=
        '<div class="row border-bottom user-item" index="'+index+'">'+
            '<div class="container">'+
                '<div class="row">'+
                    '<div class="col-1 p-1 my-auto"><img src="'+user.profile_picture+'" style="width:100%"/></div>'+
                    '<div class="col-5 pl-1 pr-0 my-auto h6">'+user.name+'</div>'+
                    '<div class="col-6 p-0 my-auto">'+user.email+'</div>'+
                '</div>'+
                '<div class="row">'+
                    '<div class="col-6 p-0 my-auto">'+(user.verification_code>0?'<small>Not verified</small>':'<small>Verified</small>')+'</div>'+
                    '<div class="col-6 p-0 my-auto">'+user.pw+'</div>'+
                '</div>'+
            '</div>'+
        '</div>';
    }
    Client.$userListElement.html(html);
    $('.user-item').on('contextmenu',function(event){
        // //console.log("OK");
        Popup.Show(event.pageX,event.pageY,$(this));
        return false;
    });
    $('.user-item').on('click',function(){
        UserItemSelector.Select($(this));
        Client.DisplayUserInfo(Client.userList[UserItemSelector.$element.attr('index')]);
    });
}
function formatDate(str){
    var date = new Date(str);
    return date.getDay()+"/"+date.getMonth()+"/"+date.getFullYear();
}
Client.DisplayUserInfo = function(user){
    console.log("user.created_at",user.created_at);
    $('#user_info_panel').html(
        '<div class="container">'+
            '<div class="row">'+
                '<div class="col-3">'+
                    '<img src="'+user.profile_picture+'" style="width:100%;">'+
                '</div>'+
                '<div class="col-9 px-1">'+
                    '<div class="h3">'+user.name+'</div>'+
                    '<div class="p"><strong>Started From: </strong>'+formatDate(user.created_at)+'</div>'+
                    '<div class="p"><strong>Score: </strong>'+user.score+'</div>'+
                '</div>'+
                '<div style="position:absolute;right:0px;top:0px"><i style="cursor:pointer" class="fas fa-window-close" id="close_user_info_panel_button"></i>'+
                '</div>'+
            '</div>'+
            '<div class="row">'+
                '<div class="col-12">'+
                    '<div><small><strong class="mb-0">Current Question: </strong>'+JSON.stringify(user.current_question_indices)+'</small></div>'+
                    '<div><small><strong class="mb-0">Correct Answers: </strong>'+user.correct_answer_count+'</small></div>'+
                '</div>'+
            '</div>'+
        '</div>'
        );
}
$('#user_info_panel').delegate('#close_user_info_panel_button','click',function(){
    $('#user_info_panel').html('');
});
Client.Init = function(){
    $.get(Client.baseUrl+'/getonluckmetadata',function(response){
        // //console.log(response);
        var json = JSON.parse(response);
        if(json.status == "OK"){
            Client.onluckMetadata = json.data;
            DisplayMetadata();
        }
    }).done(function(){

        LoadSeasonList();

    });
    // This is where the admin begins
    LoadUserList();
    Client.ToggleSeasonCreator(false);

    $('#quote').delegate('#quote_edit_button','click',function(){
        $(this).parent().html('<div><textarea id="quote_edit" style="width:100%">'+Client.onluckMetadata.quote+'</textarea>'+
            '<i class="fas fa-save" id="quote_save_button" style="cursor:pointer"></i> '+
            '<i style="cursor:pointer" class="fas fa-window-close" id="quote_close_button"></i></div>');
    });
    $('#quote').delegate('#quote_save_button','click',function(){
        var newQuote = $('#quote_edit').val();
        if(newQuote==Client.onluckMetadata.quote){
            DisplayMetadata();
            return;
        }
        // //console.log( $('#quote_edit').val());
        $.post(Client.baseUrl+'/storemetadata',{new_quote:newQuote},function(response){
            var json = JSON.parse(response);
            if(json.status == "OK"){
                Client.onluckMetadata.quote = newQuote;
                DisplayMetadata();
            }
        });
    });
    $('#quote').delegate('#quote_close_button','click',function(){
        $(this).parent().html('');
        DisplayMetadata();
    });
    $('#refresh_user_list_button').on('click',function(){
        Client.$userListElement.html('');
        LoadUserList();
        $(this).addClass('animate-rotate');
        $('#user_info_panel').html('');
    });
    $('#refresh_season_list_button').on('click',function(){
        Client.ClearSeasonPanel();
        Client.ClearPackPanel();
        Client.ClearQuestionPanel();
        SeasonItemSelector.Select(null);
        PackItemSelector.Select(null);
        QuestionItemSelector.Select(null);
        LoadSeasonList();
        $(this).addClass('animate-rotate');
    });
    $('#intro_button').on('click',function(){
        ShowTextToEdit(0,Client.onluckMetadata.intro_content);
    });
    $('#guideline_button').on('click',function(){
        ShowTextToEdit(1,Client.onluckMetadata.guideline_content);
    });
    $('#central_panel').delegate('#central_panel_save_button','click',function(){
        var text = $(this).parent().find('textarea').val();
        var index = $(this).parent().find('textarea').attr('index');
        var data = {};
        if(index==0){
            Client.onluckMetadata.intro_content = text;
            data.intro_content =text;
        }else if(index==1){
            data.guideline_content =text;
        }
        console.log(text,index);
        $.post(Client.baseUrl+'/storemetadata',data,function(response){
            var json = JSON.parse(response);
            if(json.status == "OK"){
                $('#central_panel').html('');
            }
        });
    });
    $('#central_panel').delegate('.close_parent_button','click',function(){
        $(this).parent().html('');
    });
}
function LoadUserList(){
    $.get(Client.baseUrl+'/getusers',function(response){
        console.log(response);
        var json = JSON.parse(response);
        if(json.status == "OK"){
            Client.userList = json.data;
            Client.DisplayUserList();
        }
        $('#refresh_user_list_button').removeClass('animate-rotate');
    });
}
function LoadSeasonList(){
    $.get(Client.baseUrl+'/getseasons',function(response){
        // //console.log(response);
        var json = JSON.parse(response);
        if(json.status == "OK"){
            Client.seasonList = json.data;
            Client.DisplaySeasonList();
        }
        $('#refresh_season_list_button').removeClass('animate-rotate');
    });
}
function DisplayMetadata(){
    $('#quote').html('<span>'+Client.onluckMetadata.quote+'</span><i class="fas fa-pen" id="quote_edit_button" style="cursor:pointer"></i>');
}
function ShowTextToEdit(index,content){
    $('#central_panel').html(
        '<div><textarea style="width:100%" index='+index+'>'+content+'</textarea>'+
        '<i id="central_panel_save_button" style="cursor:pointer" class="fas fa-save"></i> '+
        '<i style="cursor:pointer" class="fas fa-window-close close_parent_button"></i></div>');
}

Client.DeleteUserByIndex = function(index){
    Client.DeleteUser(Client.userList[index].id);
}
Client.DeleteUser = function(id){
    $.get(Client.baseUrl+'/deleteuser?id='+id,function(response){
        // //console.log(response);
        var json = JSON.parse(response);
        if(json.status=="OK"){
            Client.userList.splice(Client.userList.findIndex(user => user.id === id),1);
            Client.DisplayUserList();
        }
    });
}
Client.DisplaySeasonList = function(){
    if(Client.seasonList == null) return;
    $('#season_count').text(Client.seasonList.length);
    var html = '';
    for(var index = 0; index<Client.seasonList.length; index++){
        var season = Client.seasonList[index];
        html+=
        '<div class="container px-0 mb-2 pt-2 border-top season-item '+
            (season.id == Client.onluckMetadata.pointed_season?
                ((Client.onluckMetadata.season_uptodate_token==Client.onluckMetadata.activation_code)?'active-season':'pointed-season')
                :(season.id == Client.onluckMetadata.active_season?'active-season':''))+
            '" index="'+index+'">'+
            '<div class="row">'+
                '<div class="col-1 text-center h6">'+(index+1)+'</div>'+
                '<div class="col-11 h6">'+season.name+'</div>'+
            '</div>'+
            '<div class="row">'+
                '<div class="col-12 text-right"><small style="font-style:italic">'+formatDate(season.to)+' - '+formatDate(season.from)+'</small></div>'+
            '</div>'+
        '</div>';
    }
    Client.$seasonListElement.html(html);

}
Client.SeasonDataChanged = function(){
    if(SeasonItemSelector.$element!=null){
        if(Client.seasonList[SeasonItemSelector.$element.attr('index')].id == Client.onluckMetadata.pointed_season){
            Client.onluckMetadata.activation_code = -1;
        }
    }
    Client.DisplaySeasonList();
}
Client.$seasonListElement.delegate('.season-item','click',function(){
    var seasonIndex = $(this).attr('index');
    // //console.log(Client.seasonList[seasonIndex].id);
    var seasonItem = $(this);

    if(SeasonItemSelector.$element!=null){
        if(seasonIndex == SeasonItemSelector.$element.attr('index')){
            if(SeasonItemSelector.$element!=seasonItem){
                SeasonItemSelector.Select(seasonItem);
            }
            return;
        }
    }
    Client.LoadPacksOfSeason(Client.seasonList[seasonIndex],function(){
        SeasonItemSelector.Select(seasonItem);
        Client.TogglePackCreator(false);
    });
});
Client.ToggleSeasonCreator = function(status,index=-1){
    if(status){
        var season = {name:"",from:"",to:""};
        if(index!=-1)
            season = Client.seasonList[index];
        Client.$seasonCreatorElement.html(
            '<form id="season_form">'+
                (index!=-1?('<input type="hidden" name="id" value="'+season.id+'">'):'')+
                '<div class="input-group">'+
                '<label for="name" class="input-group-text">Name </label>'+
                '<input type="text" id="name" name="name" class="form-control" value="'+season.name+'">'+
                '</div>'+

                '<div class="input-group">'+
                '<label for="from" class="input-group-text">From </label>'+
                '<input type="date" id="from" name="from" class="form-control" value="'+season.from.split(" ")[0]+'">'+
                '</div>'+

                '<div class="input-group">'+
                '<label for="to" class="input-group-text">To </label>'+
                '<input type="date" id="to" name="to" class="form-control" value="'+season.to.split(" ")[0]+'">'+
                '</div>'+

                '<button type="submit" class="btn btn-primary">Submit</button>'+
                '<button type="button" class="btn btn-danger" id="season_creator_cancel_button">Cancel</button>'+
            '</form>'
            );
        $('#season_form').submit(function(event){
            event.preventDefault();
            // Client.ToggleSeasonCreator(false);
            var name = $(this).find('input[name="name"]').val();
            var from = $(this).find('input[name="from"]').val()+" 00:00:00";
            var to = $(this).find('input[name="to"]').val()+" 00:00:00";
            var url = Client.baseUrl+'/createseason';
            var data = {name:name,from:from,to:to};

            if(index!=-1){
                var id = $(this).find('input[name="id"]').val();
                data.id = id;
                url = Client.baseUrl+'/updateseason';
            }

            $.post(url,data,function(response){
                var json = JSON.parse(response);
                if(json.status == "OK"){
                    Client.seasonList = json.data;
                    Client.SeasonDataChanged();
                    Client.ToggleSeasonCreator(false);
                    Client.ClearPackPanel();
                    Client.ClearQuestionPanel();
                    SeasonItemSelector.Select(null);
                }
            });
        });
        $('#season_creator_cancel_button').on('click',function(){
            Client.ToggleSeasonCreator(false);
        });
    }else{
        Client.$seasonCreatorElement.html('<button class="btn btn-primary" id="season_create_button">Create</button>');
        $('#season_create_button').on('click',function(){
            Client.ToggleSeasonCreator(true);
        });
    }
}
Client.ChangeSeason=function(id){
    $.get(Client.baseUrl+"/changeseason?id="+id,function(response){
        // //console.log(response);
        var json = JSON.parse(response);
        if(json.status == "OK"){
            Client.onluckMetadata = json.data;
            Client.DisplaySeasonList();
        }
    });
}
Client.ActivateSeason=function(id){
    $.get(Client.baseUrl+"/activateseason?id="+id,function(response){
        // //console.log(response);
        var json = JSON.parse(response);
        if(json.status == "OK"){
            Client.onluckMetadata = json.data;
            Client.DisplaySeasonList();
        }
    });
}
Client.LoadPacksOfSeason =function(season,callback=null){
    if(season.packList==null){
        $.get(Client.baseUrl+"/getpacks?season_id="+season.id,function(res){
            //console.log(res);
            var response = JSON.parse(res);
            if(response.status == "OK"){
                Client.packList = response.data;
                season.packList = Client.packList;
                Client.DisplayPackList(season.packList);
                Client.ClearQuestionPanel();
                if(callback!=null)
                    callback();
            }
        });
    }else{
        Client.packList = season.packList;
        Client.DisplayPackList(season.packList);
        Client.ClearQuestionPanel();
        if(callback!=null)
            callback();
    }
}
Client.DisplayPackList = function(){
    if(Client.packList == null) return;
    $('#pack_count').text(Client.packList.length);
    Client.$packListElement.empty();
    for(var index = 0; index<Client.packList.length; index++){
        var pack = Client.packList[index];
        var $elementItem = $(
        '<div class="container px-0 pack-item mb-2 pt-2 border-top" index="'+index+'" season_id="'+pack.season_id+'" question_type="'+pack.question_type+'">'+
            '<div class="row">'+
                '<div class="col-1 pl-1 pr-0 my-auto h6 text-center">'+(index+1)+'</div>'+
                '<div class="col-1 p-0 my-auto"><img src="'+pack.icon+'" style="width:100%"/></div>'+
                '<div class="col-5 h5">'+pack.title+'</div>'+
                '<div class="col-5">'+pack.sub_text+'</div>'+
            '</div>'+
            '<div class="row">'+
                '<div class="col-12 text-right"><small style="font-style:italic">'+(pack.question_type==0?'Typing':'MCQ')+'</small> | <small>'+pack.season_id+'<small></div>'+
            '</div>'+
        '</div>');
        Client.$packListElement.append($elementItem);
    }
    $('.pack-item').on('click',function(){

        var packIndex = $(this).attr('index');
        var seasonId = $(this).attr('season_id');
        var questionType = $(this).attr('question_type');
        // //console.log(packId, seasonId,questionType);
        var packItem = $(this);
        Client.LoadQuestionsOfPack(seasonId,Client.packList[packIndex],questionType,()=>{
            PackItemSelector.Select(packItem);
            Client.ToggleQuestionCreator(false,questionType);
        });
    });
}
Client.TogglePackCreator = function(status,index=-1){
    if(status){

        var season = Client.seasonList[SeasonItemSelector.$element.attr('index')];
        var pack = {id:0,icon:"",title:"",sub_text:"",question_type:0};
        if(index!=-1)
            pack = Client.packList[index];
        Client.$packCreatorElement.html(
            '<form id="pack_form">'+
                '<input type="hidden" name="id" value='+pack.id+'>'+
                '<input type="hidden" name="season_id" value='+season.id+'>'+


                '<div class="input-group">'+
                '<label for="icon" class="input-group-text">Pack Icon <img id="icon_preview" src="'+pack.icon+'" style="width:100px"/></label>'+
                '<input type="file" id="icon" name="icon" accept="image/*" required>'+
                '</div>'+

                '<div class="input-group">'+
                '<label for="title" class="input-group-text">Title </label>'+
                '<input type="text" id="title" name="title" class="form-control" value="'+pack.title+'" required>'+
                '</div>'+

                '<div class="input-group">'+
                '<label for="sub_text" class="input-group-text">Subtext </label>'+
                '<input type="text" id="sub_text" name="sub_text" class="form-control" value="'+pack.sub_text+'" required>'+
                '</div>'+

                    ((index!=-1)?'':(
                '<div class="input-group">'+
                    '<label for="question_type" class="input-group-text">Question Type </label>'+
                    '<select id="question_type" name="question_type">'+
                        '<option value="0" >Typing</option>'+
                        '<option value="1" >MCQ</option>'+
                    '</select>'+
                '</div>'))+

                '<button type="submit" class="btn btn-primary">Submit</button>'+
                '<button type="button" class="btn btn-danger" id="pack_creator_cancel_button">Cancel</button>'+
            '</form>'
            );

        $('#pack_form').submit(function(event){
            event.preventDefault();

            var formData = new FormData();
            formData.append('season_id',$(this).find('input[name="season_id"]').val());
            formData.append('title',$(this).find('input[name="title"]').val());
            formData.append('sub_text',$(this).find('input[name="sub_text"]').val());
            formData.append('icon',$(this).find('input[name="icon"]')[0].files[0]);
            formData.append('question_type',$(this).find('select[name="question_type"]').val());

            var url = Client.baseUrl+'/createpack';
            if(index!=-1){
                url = Client.baseUrl+'/updatepack';
                formData.append('id',$(this).find('input[name="id"]').val());
            }

            $.ajax({
                url: url,
                type: 'post',
                data: formData,
                contentType: false,
                processData: false,
                success: function(response){
                    //console.log(response);
                    var json = JSON.parse(response);
                    if(json.status == "OK"){
                        Client.packList = json.data;
                        Client.seasonList[SeasonItemSelector.$element.attr('index')].packList = Client.packList;
                        Client.DisplayPackList(Client.packList);
                        Client.TogglePackCreator(false);
                        Client.SeasonDataChanged();
                        Client.ClearQuestionPanel();
                    }
                },
            });

        });
        $('#icon').on('change',function(){readURL($(this)[0],$('#icon_preview'));});
        $('#pack_creator_cancel_button').on('click',function(){
            Client.TogglePackCreator(false);
        });
    }else{
        Client.$packCreatorElement.html('<button class="btn btn-primary" id="pack_create_button">Create</button>');
        $('#pack_create_button').on('click',function(){
            Client.TogglePackCreator(true);
        });
    }
}
Client.LoadQuestionsOfPack = function(seasonId, pack,question_type,callback = null){
    if(pack.questionList == null){
        $.get(Client.baseUrl+"/getquestions?pack_id="+pack.id+"&question_type="+question_type,function(response){
            //console.log(response);
            var json = JSON.parse(response);
            if(json.status == "OK"){
                Client.questionList = json.data;
                pack.questionList = Client.questionList;
                Client.DisplayQuestionList();
                // Client.ToggleQuestionCreator(false,null);
                if(callback!=null)
                    callback();
            }
        });
    }else{
        Client.questionList = pack.questionList;
        Client.DisplayQuestionList();
        if(callback!=null)
            callback();
    }
}

Client.ClearSeasonPanel = function(){
    Client.$seasonListElement.html('');
    Client.$seasonCreatorElement.html('');
    $('#season_count').text(0);
}
Client.ClearPackPanel = function(){
    Client.$packListElement.html('');
    Client.$packCreatorElement.html('');
    $('#pack_count').text(0);
}
Client.ClearQuestionPanel = function(){
    Client.$questionListElement.html('');
    Client.$questionCreatorElement.html('');
    $('#question_count').text(0);
}
Client.ToggleQuestionCreator = function(status, mode, index=-1){
    //console.log(status,mode);
    if(status){
        var question = {id:0,question:"",answer:"",choicesArray:["","","",""],time:30,score:10};
        if(index!=-1){
            question = Client.questionList[index];
            question.hintsArray = JSON.parse(question.hints);
            question.imagesArray = JSON.parse(question.images);
            if(mode==1)
                question.choicesArray = JSON.parse(question.choices);
        }

        var html=
            '<form id="question_form">'+
                ((index!=-1)?('<input type="hidden" name="id" value='+question.id+'>'):'')+
                '<input type="hidden" name="pack_id" value='+Client.packList[PackItemSelector.$element.attr('index')].id+' question_type="'+mode+'" >'+


                '<div class="input-group">'+
                '<label for="question" class="input-group-text">Question </label>'+
                '<input type="text" id="question" name="question" class="form-control" value="'+question.question+'" required>'+
                '</div>'+


                (mode==0?(

                    '<div class="input-group">'+
                    '<label for="answer" class="input-group-text">Answer </label>'+
                    '<input type="text" id="answer" name="answer" class="form-control" value="'+question.answer+'" required>'+
                    '</div>' 
                ):(

                    '<div class="input-group">'+
                        '<label for="choice" class="input-group-text">Choices </label>'+
                        '<div id="choice_container">'+
                        '<input type="text" id="choice" name="choices" class="form-control" placeholder="A" value="'+question.choicesArray[0]+'" required>'+
                        '<input type="text" name="choices" class="form-control" placeholder="B" value="'+question.choicesArray[1]+'" required>'+
                        '<input type="text" name="choices" class="form-control" placeholder="C" value="'+question.choicesArray[2]+'" required>'+
                        '<input type="text" name="choices" class="form-control" placeholder="D" value="'+question.choicesArray[3]+'" required>'+
                        '</div>'+
                    '</div>'+
                    '<div class="input-group">'+
                        '<label for="answer" class="input-group-text">Answer </label>'+
                        '<select id="answer" name="answer">'+
                            '<option value="0" '+((question.answer==0)?'selected':'')+'>A</option>'+
                            '<option value="1" '+((question.answer==1)?'selected':'')+'>B</option>'+
                            '<option value="2" '+((question.answer==2)?'selected':'')+'>C</option>'+
                            '<option value="3" '+((question.answer==3)?'selected':'')+'>D</option>'+
                        '</select>'+
                    '</div>'+

                    '<div class="input-group">'+
                    '<label for="time" class="input-group-text">Time </label>'+
                    '<input type="number" id="time" name="time" class="form-control" value='+question.time+' required>'+
                    '</div>'
                ))+
                




                '<div class="input-group">'+
                '<label for="score" class="input-group-text">Score </label>'+
                '<input type="number" id="score" name="score" class="form-control" value='+question.score+' required>'+
                '</div>'+

                '<div class="input-group">'+
                    '<label class="input-group-text">Hints </label>'+
                    '<div id="hint_container">';

                if(index!=-1){
                    for(var i = 0; i<question.hintsArray.length; i++){
                        html+='<input type="text" name="hints" class="form-control" value="'+question.hintsArray[i]+'">';
                    }
                }else{
                    html+='<input type="text" name="hints" class="form-control">';
                }                
                html+=
                    '</div>'+
                    '<div style="display:inline-block;align-self:flex-end">'+
                        '<button type="button" id="remove_hint_button">-</button>'+
                        '<button type="button" id="new_hint_button">+</button>'+
                    '</div>'+
                '</div>';

                if(index!=-1){
                    html+=
                    '<div class="card">'+
                    '<label for="images" class="input-group-text">Existing Images </label>'+
                    '<div class="card" id="existing_images_preview">';
                    for(var i = 0; i<question.imagesArray.length;i++){
                        html+=
                        '<div><img src="'+question.imagesArray[i]+'" class="my-1" style="width:80px;"/>'+
                        '<button class="existing_image_preview_button" index="'+i+'" type="button" style="width:25px;">x</button>'+
                        '</div>';
                    }
                    html+='</div>'+
                    '</div>';
                }           
                html+=


                '<div class="input-group">'+
                '<label for="images" class="input-group-text">New Images </label>'+
                '<div class="card" id="images_preview"></div>'+
                '<input type="file" id="images" name="images" accept="image/*" multiple="multiple" '+((index!=-1)?'':'required')+'>'+
                '</div>'+

                '<button type="submit" class="btn btn-primary">Submit</button>'+
                '<button type="button" class="btn btn-danger" id="question_creator_cancel_button">Cancel</button>'+
            '</form>';
        Client.$questionCreatorElement.html(html);

        $('#question_creator_cancel_button').on('click',function(){
            Client.ToggleQuestionCreator(false);
        });
        $('#new_hint_button').on('click',function(){
            $('<input type="text" name="hints" class="form-control" required>').appendTo('#hint_container');
        });
        $('#remove_hint_button').on('click',function(){
            $('#hint_container input').last().remove();
        });
        $('.existing_image_preview_button').on('click',function(){
            tobeDeletedImages.push($(this).attr('index'));
            $(this).parent().remove();
        });
        var $imagesPreview = $('#images_preview');
        $imagesPreview.delegate('.image_preview_button','click',function(){
            //console.log("Clicked "+imagesArray.length+" "+$(this).attr('index'));
            
            var tobeDeleteIndex = $(this).attr('index');
            imagesArray.splice(tobeDeleteIndex,1);
            $(this).parent().remove();
            for(var i = tobeDeleteIndex; i<imagesArray.length;i++){
                imagesArray[i].$element.find('.image_preview_button').attr('index',i);
            }
        });
        var tobeDeletedImages = [];
        var imagesArray = [];
        $('#images').on('change',function(){
            readSelectedImage($(this)[0].files,function(fileReader){
                var index = imagesArray.length;
                var $element = $('<div><img src="'+fileReader.result+'" class="my-1" style="width:80px;"/><button class="image_preview_button" index="'+index+'" type="button" style="width:25px;">x</button></div>');
                $imagesPreview.append($element);
                fileReader.$element = $element;
                imagesArray.push(fileReader);
            });
        });

        $('#question_form').submit(function(event){
            event.preventDefault();
            
            var hints = [];
            $('input[name="hints"]').each(function(){
                hints.push($(this).val());
            });


            var formData = new FormData();
            formData.append('question_type',$(this).find('input[name="pack_id"]').attr('question_type'));
            formData.append('question',$(this).find('input[name="question"]').val());

            if(mode==0){
                formData.append('answer',$(this).find('input[name="answer"]').val());
            }else if(mode==1){
                var choices = [];
                $('input[name="choices"]').each(function(){
                    choices.push($(this).val());
                });
                formData.append('choices',JSON.stringify(choices));
                formData.append('answer',$(this).find('select[name="answer"]').val());
                formData.append('time',$(this).find('input[name="time"]').val());
            }

            formData.append('score',$(this).find('input[name="score"]').val());
            formData.append('hints',JSON.stringify(hints));
            
            var images = imagesArray;// ('input[name="images"]')[0].files;
            for(var i = 0; i<images.length; i++){
                formData.append('images[]',images[i].file);
            }

            var url = Client.baseUrl+'/createquestion';
            if(index!=-1){
                formData.append('id',$(this).find('input[name="id"]').val());
                formData.append('tobedeleted_images',JSON.stringify(tobeDeletedImages));
                //console.log("Tobe delete images "+JSON.stringify(tobeDeletedImages));
                url = Client.baseUrl+'/updatequestion';;
            }else{
                formData.append('pack_id',$(this).find('input[name="pack_id"]').val());
            }
            $.ajax({
                url: url,
                type: 'post',
                data: formData,
                contentType: false,
                processData: false,
                success: function(response){
                    //console.log(response);
                    var json = JSON.parse(response);
                    if(json.status == "OK"){
                        Client.questionList = json.data;
                        Client.packList[PackItemSelector.$element.attr('index')].questionList = Client.questionList;
                        Client.DisplayQuestionList();
                        Client.ToggleQuestionCreator(false,null);
                        Client.SeasonDataChanged();
                    }
                },
            });
        });

    }else{
        Client.$questionCreatorElement.html('<button class="btn btn-primary" id="question_create_button">Create</button>');
        $('#question_create_button').on('click',function(){
            Client.ToggleQuestionCreator(true,Client.packList[PackItemSelector.$element.attr('index')].question_type);
        });
    }
}
Client.DisplayQuestionList = function(){
    if(Client.questionList == null) return;
    $('#question_count').text(Client.questionList.length);
    var html = '';
    for(var index = 0; index<Client.questionList.length; index++){
        var question = Client.questionList[index];
        var images = JSON.parse(question.images);
        //console.log(images);
        html+=
        '<div class="container px-0 mb-1 question-item border-top" index='+index+'>'+
            '<div class="row">'+
                '<div class="col-1 h6 my-auto text-center">'+(index+1)+'</div>'+
                '<div class="col-1 p-0 my-auto"><img src="'+images[0]+'" style="width:100%"/></div>'+
                '<div class="col-5 pl-1 pr-0 my-auto">'+question.question+'</div>'+
                '<div class="col-5 pl-1 pr-0 my-auto text-center">'+question.answer+'</div>'+
            '</div>'+
            '<div class="row">'+
                '<div class="col-12 pl-1 pr-0 my-auto text-right"><small style="font-style:italic">'+question.score+' | '+question.hints+'</small></div>'+
            '</div>'+
        '</div>';
    }
    Client.$questionListElement.html(html);
    $('.question-item').on('click',function(){
        QuestionItemSelector.Select($(this));
    });
}
Client.Init();


class ItemSelector{
    constructor(styleClass,$popupElement){
        this.styleClass = styleClass;
        this.$element = null;
        this.$popupElement = $popupElement;
    }
    Select($element){
        this.Clear();
        this.$element = $element;
        if($element == null)return;
        this.$element.addClass(this.styleClass);
        if(this.$popupElement!=null)
            this.$popupElement.appendTo(this.$element);
    }
    Clear(){
        if(this.$element!=null){
            this.$element.removeClass(this.styleClass);
        }
    }
}
$(document).delegate(".edit_item_button",'click',function(){
    //console.log("Edit it babe" );
    if($(this).attr("who") == "season"){
        OnEditSeasonButtonClick();
    }else if($(this).attr("who") == "pack"){
        OnEditPackButtonClick();
    }else if($(this).attr("who") == "question"){
        OnEditQuestionButtonClick();
    }
});
$(document).delegate(".delete_item_button",'click',function(){
    //console.log("Delete it babe");
    if($(this).attr("who") == "season"){
        OnDeleteSeasonButtonClick();
    }else if($(this).attr("who") == "pack"){
        OnDeletePackButtonClick();
    }else if($(this).attr("who") == "question"){
        OnDeleteQuestionButtonClick();
    }
});
$(document).delegate('.active-button','click',function(){
    //console.log("Comeone");
    var func = $(this).attr("function");
    if(func == "change_season"){
        //console.log("Change it babe");
        Client.ChangeSeason(Client.seasonList[SeasonItemSelector.$element.attr('index')].id);
    }else if(func == "activate_season"){
        //console.log("Activate it babe");
        Client.ActivateSeason(Client.seasonList[SeasonItemSelector.$element.attr('index')].id);
    }
});
var popupElement = 
    '<div class="my-1">'+
    '<button class="btn btn-secondary btn-sm edit_item_button">Edit</button>'+
    '<button class="btn btn-danger btn-sm delete_item_button">Delete</button>'+
    '</div>';


class SeasonItemSelectorClass extends ItemSelector{
    constructor(){
        super('item-selected',$(
        '<div>'+
        '<button class="btn btn-secondary btn-sm edit_item_button">Edit</button>'+
        '<button class="btn btn-danger btn-sm delete_item_button">Delete</button>'+
        '<button class="btn btn-warning btn-sm active-button" function="no_function">No Function</button>'+
        '<small id="info_text" style="color:gray">This season is activated</button>'+
        '</div>'
        ));
        this.$popupElement.find(".edit_item_button").attr("who","season");
        this.$popupElement.find(".delete_item_button").attr("who","season");
    }
    Select($element){
        super.Select($element);
        if($element==null)return;
        //console.log("come on");
        var season = Client.seasonList[this.$element.attr('index')];
        var $button = this.$popupElement.find('.active-button');
        var $deleteButton = this.$popupElement.find('.delete_item_button');

        if(season.id == Client.onluckMetadata.pointed_season){
            $deleteButton.hide();
            //console.log(Client.onluckMetadata.season_uptodate_token,Client.onluckMetadata.activation_code);
            if(Client.onluckMetadata.season_uptodate_token == Client.onluckMetadata.activation_code){
                $button.hide();//addClass('hidden');
                this.$element.find('#info_text').show();
            }else{
                this.$element.find('#info_text').hide();
                $button.show();//removeClass('hidden');
                // $button.removeClass('hidden');
                $button.addClass('btn-success');
                $button.removeClass('btn-warning');
                $button.attr("function","activate_season");
                $button.text("Activate this season");
            }
        }else{
            this.$element.find('#info_text').hide();
            $deleteButton.show();
            var $button = this.$popupElement.find('.active-button');
            $button.removeClass('btn-success');
            $button.addClass('btn-warning');
            $button.show();//removeClass('hidden');
            $button.attr("function","change_season");
            $button.text("Change to this");
        }
    }
}
var SeasonItemSelector = new SeasonItemSelectorClass();//ItemSelector('item-selected',$popupElement);
function OnEditSeasonButtonClick(){
    var index = SeasonItemSelector.$element.attr('index');
    Client.ToggleSeasonCreator(true,index);
}
function OnDeleteSeasonButtonClick(){
    if(!confirm("You are sure to delete this?"))return;
    var id = Client.seasonList[SeasonItemSelector.$element.attr('index')].id;
    $.post(Client.baseUrl+"/deleteseason",{id:id},function(response){
        //console.log(response);
        var json = JSON.parse(response);
        if(json.status == "OK"){
            Client.seasonList = json.data;
            Client.DisplaySeasonList();
            Client.ClearPackPanel();
            Client.ClearQuestionPanel();
            Client.ToggleSeasonCreator(false);
        }
    });
}
var $popupElement2 = $(popupElement);
$popupElement2.find(".edit_item_button").attr("who","pack");
$popupElement2.find(".delete_item_button").attr("who","pack");
var PackItemSelector = new ItemSelector('item-selected',$popupElement2);
function OnEditPackButtonClick(){
    var index = PackItemSelector.$element.attr('index');
    Client.TogglePackCreator(true,index);
}
function OnDeletePackButtonClick(){
    if(!confirm("You are sure to delete this?"))return;
    var id = Client.packList[PackItemSelector.$element.attr('index')].id;
    $.post(Client.baseUrl+"/deletepack",{id:id},function(response){
        //console.log(response);
        var json = JSON.parse(response);
        if(json.status == "OK"){
            Client.packList = json.data;
            Client.seasonList[SeasonItemSelector.$element.attr('index')].packList = Client.packList;
            Client.DisplayPackList(Client.packList);
            Client.ClearQuestionPanel();
            Client.TogglePackCreator(false);
        }
    });
}
var $popupElement3 = $(popupElement);
$popupElement3.find(".edit_item_button").attr("who","question");
$popupElement3.find(".delete_item_button").attr("who","question");
var QuestionItemSelector = new ItemSelector('item-selected',$popupElement3);
function OnEditQuestionButtonClick(){
    var index = QuestionItemSelector.$element.attr('index');
    Client.ToggleQuestionCreator(true,Client.packList[PackItemSelector.$element.attr('index')].question_type,index);
}
function OnDeleteQuestionButtonClick(){
    if(!confirm("You are sure to delete this?"))return;
    var question = Client.questionList[QuestionItemSelector.$element.attr('index')];
    var id = question.id;
    $.post(Client.baseUrl+"/deletequestion",{id:id,pack_id:question.pack_id},function(response){
        //console.log(response);
        var json = JSON.parse(response);
        if(json.status == "OK"){
            Client.questionList = json.data;
            Client.packList[PackItemSelector.$element.attr('index')].questionList = Client.questionList;
            Client.DisplayQuestionList();
            Client.ToggleQuestionCreator(false);
        }
    });
}

// var $userSelectorElement = $('<div>Hello world</div>');
var UserItemSelector = new ItemSelector('item-selected');

var Popup = {
    $element:$('#popup'),
    $baseElement:null
};
Popup.Init = function(){
    $('#btn_popup_delete').on('click',function(){
        //console.log("Hello");
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

function readURL(input,$targetImage) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $targetImage
                .attr('src', e.target.result);
                // .width(150)
                // .height(200);
        };

        reader.readAsDataURL(input.files[0]);
    }
}
function readSelectedImage(files,callback) {
    if (files) {
        for(var i = 0; i<files.length;i++){
            var reader = new FileReader();
            reader.file = files[i];
            reader.onload = function (e) {
                callback(e.target);
                // //console.log(e.target);
            };
            reader.readAsDataURL(files[i]);
        }
    }
}

$(document).on('click',function(){
    //console.log('global click');
    Popup.Hide();
    return true;
});



</script>
</body>
</html>