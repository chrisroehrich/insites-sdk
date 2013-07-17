/*
 * Copyright (c) Intelligent InSites. All rights reserved.
 * http://www.intelligentinsites.com
 */

/**
 * --------------------------------------------------------------------
 * jQuery tree plugin
 * Author: Scott Jehl, scott@filamentgroup.com
 * Copyright (c) 2009 Filament Group 
 * licensed under MIT (filamentgroup.com/examples/mit-license.txt)
 * --------------------------------------------------------------------
 */







$.fn.tree = function(settings){

    var o = $.extend({
        expanded: ''
    },settings);

    return $(this).each(function(){
        if( !$(this).parents('.tree').length ){
        //save reference to tree UL
        var tree = $(this);

        //add the role and default state attributes
       // if( !$('body').is('[role]') ){ $('body').attr('role','application'); }


        //add role and class of tree
        tree.attr({'role': 'tree'}).addClass('tree');
        //set first node's tabindex to 0
        tree.find('a:eq(0)').attr('tabindex','0');
        //set all others to -1
        tree.find('a:gt(0)').attr('tabindex','-1');
        //add group role and tree-group-collapsed class to all ul children
        tree.find('ul').attr('role','group').addClass('tree-group-collapsed');
        //add treeitem role to all li children


        tree.find('li a').attr('role','treeitem');
        //find tree group parents
        tree.find('li:has(ul)')
                .attr('aria-expanded', 'false')
				.attr('role', 'presentation')
                .find('>a')
				  .attr('aria-expanded', 'false')
                .addClass('tree-parent tree-parent-collapsed');
				//tree.find('ins').attr( 'role','presentation').attr( 'aria-hidden','true');
	

        //expanded at load        
        tree
           .find(o.expanded)
            .attr('aria-expanded', 'true')
			.attr('role', 'presentation')
                .find('>a')
				 .attr('aria-expanded', 'true')
                .removeClass('tree-parent-collapsed')
                .next()
                .removeClass('tree-group-collapsed');



        //bind the custom events
        tree
            //expand a tree node
            .bind('expand',function(event){
                var target = $(event.target) || tree.find('a[tabindex=0]');
                
				//sample code 
          tree.active_ele=target;
          if(!target.attr('tarvered') && o.url){
                        target.attr('tarvered','true');        
            var lid = target.parent().attr('id');
            
            ajax_url = o.url;
            query = {}
            query[o.query] = lid;
            
           if(lid){
             //url= url+"?location_id="+lid;
            }
      
           //$("div#loading").remove();
           //$('body').append('<div id="loading"><a href="#">loading please wait</a></div>');
           //$('div#message > a').focus(); 
          $.ajax({
             url:ajax_url,  
             beforeSend:function(xht){  target.removeClass('tree-parent'); target.addClass('ajax-loading'); },
             complete:function(xhr,status){target.removeClass('ajax-loading'); target.addClass('tree-parent');},
            data:query,
            error:function(xhr,status,error){
              if(status="parsererror"){
                 target.trigger('collapse');
                  return false;
              }
               

            },
            success:function(data){
           var ul = target.next();
           for(var i =0 ; i<data.length; i++){
               if(!data){
                  target.removeAttr('class');
                 return false;

               }

              if (data[i].hasChildren) {
                        

           var li = "<li role='presentation' id='" + data[i].id + "' aria-expanded='false' >";
               li += "<a aria-expanded='false' role='treeitem' class='tree-parent tree-parent-collapsed' tabindex='-1'";
               li += " href='#'>" + data[i].title + "</a>" + '<ul class="tree-group-collapsed" role="group"></ul>' + "</li>";
  
             
             
                  }else{
             
                 var li = "<li role='presentation' id='" + data[i].id + "'><a href='javascript:void(0)'  tabindex='-1' role='treeitem'>" + data[i].title + "</a></li>";  
                
             }

       $(ul).append(li);
      try{
	   	top.AutoResizeIframe();
	   }
       catch(err){
		;
	   }
           
           }
        //$(ul).find('a:eq(0)').attr('tabindex','0');
        // Set all others to -1
        //$(ul).find('a').attr('tabindex','-1');


          //target.next().find('a:eq(0)').focus();
          //restyle('#files');
               }
          });
                  //update the tree 
                  



           }


          //sample code 
				
				
				
				target.removeClass('tree-parent-collapsed');
                target.next().hide().removeClass('tree-group-collapsed').slideDown(150, function(){
                    $(this).removeAttr('style');
                    target.parent().attr('aria-expanded', 'true');
					 target.attr('aria-expanded', 'true');
                });
            })
            //collapse a tree node
            .bind('collapse',function(event){
                var target = $(event.target) || tree.find('a[tabindex=0]');
                target.addClass('tree-parent-collapsed');
                target.next().slideUp(150, function(){
                    target.parent().attr('aria-expanded', 'false');
					 target.attr('aria-expanded', 'false');
                    $(this).addClass('tree-group-collapsed').removeAttr('style');
                });
            })
            .bind('toggle',function(event){
                var target = $(event.target) || tree.find('a[tabindex=0]');
                //check if target parent LI is collapsed
                if( target.parent().is('[aria-expanded=false]') ){ 
                    //call expand function on the target
                    target.trigger('expand');
                }
                //otherwise, parent must be expanded
                else{ 
                    //collapse the target
                    target.trigger('collapse');
                }
            })
            //shift focus down one item        
            .bind('traverseDown',function(event){
                var target = $(event.target) || tree.find('a[tabindex=0]');
                var targetLi = target.parent();
                if(targetLi.is('[aria-expanded=true]')){
                    target.next().find('a').eq(0).focus();
                }
                else if(targetLi.next().length) {
                    targetLi.next().find('a').eq(0).focus();
                }    
                else {                
                    targetLi.parents('li').next().find('a').eq(0).focus();
                }
            })
            //shift focus up one item
            .bind('traverseUp',function(event){
                var target = $(event.target) || tree.find('a[tabindex=0]');
                var targetLi = target.parent();
                if(targetLi.prev().length){ 
                    if( targetLi.prev().is('[aria-expanded=true]') ){
                        targetLi.prev().find('li:visible:last a').eq(0).focus();
                    }
                    else{
                        targetLi.prev().find('a').eq(0).focus();
                    }
                }
                else {                 
                    targetLi.parents('li:eq(0)').find('a').eq(0).focus();
                }
            });


        //and now for the native events
        tree    
            [$.fn.on ? 'on' : 'bind']('focus', $.fn.on ? '*' : {}, function(event){
                //deactivate previously active tree node, if one exists
                tree.find('[tabindex=0]').attr('tabindex','-1').removeClass('tree-item-active');
                //assign 0 tabindex to focused item
                $(event.target).attr('tabindex','0').addClass('tree-item-active');
            })
            .click(function(event){
                //save reference to event target
                var target = $(event.target);
                //check if target is a tree node
                if( target.is('a.tree-parent') ){
                    target.trigger('toggle');
                    target.eq(0).focus();
                    //return click event false because it's a tree node (folder)
                    return false;
                }
            })
            .keydown(function(event){    
                    var target = tree.find('a[tabindex=0]');
                    //check for arrow keys
                    if(event.keyCode == 37 || event.keyCode == 38 || event.keyCode == 39 || event.keyCode == 40){
                        //if key is left arrow 
                        if(event.keyCode == 37){ 
                            //if list is expanded
                            if(target.parent().is('[aria-expanded=true]')){
                                target.trigger('collapse');
                            }
                            //try traversing to parent
                            else {
                                target.parents('li:eq(1)').find('a').eq(0).focus();
                            }    
                        }                        
                        //if key is right arrow
                        if(event.keyCode == 39){ 
                            //if list is collapsed
                            if(target.parent().is('[aria-expanded=false]')){
                                target.trigger('expand');
                            }
                            //try traversing to child
                            else {
                                target.parents('li:eq(0)').find('li a').eq(0).focus();
                            }
                        }
                        //if key is up arrow
                        if(event.keyCode == 38){ 
                            target.trigger('traverseUp');
                        }
                        //if key is down arrow
                        if(event.keyCode == 40){ 
                            target.trigger('traverseDown');
                        }
                        //return any of these keycodes false
                        return false;
                    }    
                    //check if enter or space was pressed on a tree node
                    else if((event.keyCode == 13 || event.keyCode == 32) && target.is('a.tree-parent')){
                            target.trigger('toggle');
                            //return click event false because it's a tree node (folder)
                            return false;
                    }
            });
        }
    });
};
//$('#treeExample').tree({
  //  expanded: 'li:first'
//});

function get_path(dom) {
 
    var path = $(dom).text();
        $parent = $(dom).parents("li").eq(1).find("a:first");

    if ($parent.length == 1) {
        path = get_path($parent) + "," + path;
    }
    return path;
 }


//params dom is the ul location id 
function get_selected_id(dom){  
    var cur = $(dom).find('a.tree-item-active').parent().attr('id');
    return cur;
}
function get_selected(dom){  
    var cur = $(dom).find('a.tree-item-active');
    return cur;
}


function get_path_id(dom){
    var path=[]
     path.push($(dom).parent().attr('id'));
        $parent = $(dom).parents("li").eq(1).find("a:first");

    if ($parent.length == 1) {
        path = get_path_id($parent).concat(path);
    }
    return path;
}


function store_state(dom){

path = get_path_id(dom);
if(path.length){

$.jStorage.set("location-id-tree",path);

} 

}

// params is ul id 
function get_state(dom){

var path = $.jStorage.get("location-id-tree");

if(path && path.length){

for(var i =0; i< path.length; i++){

id = path[i];
$("li#"+id+">a").trigger('expand');

}
}
}

