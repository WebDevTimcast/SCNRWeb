$.each($('.btn-viewmore'),function(){
		var obj = $(this);
		var state;
		var parentObj = obj.parent().parent();
		var expandlinesize = parentObj.find('.list-expander').attr('data-expandlinesize');
		var expandsize = parentObj.find('.list-expander').attr('data-expandsize');
	
		var colapsed;
		var expanded;

		var sizeItems=function(){
			colapsed=0;
			expanded =0;
	

		//console.log(expandsize);

		if(!expandlinesize){
		//$(window).resize(function(){
			for(i=0; i<(parseInt(expandsize)); i++){
				colapsed +=parentObj.find('.list-expander').children().eq(i).height();
				colapsed +=parseInt(parentObj.find('.list-expander').children().eq(i).css('padding-bot'));

				//console.log(parentObj.find('.list-expander').children().eq(i).height());
				//console.log(parseInt(parentObj.find('.list-expander').children().eq(i).css('padding-bot')));
				
			}

			for(i=0; i<(parentObj.find('.list-expander').children().length); i++){
				expanded +=parentObj.find('.list-expander').children().eq(i).height();
				expanded +=parseInt(parentObj.find('.list-expander').children().eq(i).css('padding-bot'));
				
			}
		}

		if(expandlinesize){
			 console.log(parentObj.find('.list-expander p').css('line-height'));
			
				colapsed = parseInt(parentObj.find('.list-expander p').css('line-height'))*expandlinesize;

				//console.log(parentObj.find('.list-expander').children().eq(i).height());
				//console.log(parseInt(parentObj.find('.list-expander').children().eq(i).css('padding-bot')));
				
	

			for(i=0; i<(parentObj.find('.list-expander').children().length); i++){
				expanded +=parentObj.find('.list-expander').children().eq(i).height();
				expanded +=parseInt(parentObj.find('.list-expander').children().eq(i).css('padding-bot'));
				
			}
		}
			
		}
		sizeItems();

		parentObj.find('.list-expander').css('height',colapsed);

		$(window).resize(
			function(){
				sizeItems();
					//console.log(obj.hasClass('expanded')+'  '+obj.attr('class'))
				if(obj.hasClass('expanded')){
					parentObj.find('.list-expander').css('height',expanded);
				}
				else{
					parentObj.find('.list-expander').css('height',colapsed);
				}
			});

			//console.log('expanded ='+parentObj.find('.list-expander').children().length);
			

			$(this).click(function(e){
				e.preventDefault();
				//console.log('clicked');
				if(state==1){
					state =0;
					parentObj.find('.list-expander').css('height',colapsed);
					$(this).html('View More').removeClass('expanded');


				}
				else{
					state =1;
					parentObj.find('.list-expander').css('height',expanded);
					$(this).html('View Less').addClass('expanded');
				}
			
			});
		
	});