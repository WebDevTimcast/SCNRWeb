if($('.slide .bg-video').length){
    //console.log($('.videos > div > .vid-pos').length);
    $.each($('.slide .bg-videos'),function(){
        var thisv = $(this);

        var zoom =1;

        $(window).resize(function(){
            $(thisv).css('height',$(thisv).closest('.mod').height());
            
            $(thisv).find('video').css('height','100%').css('width','100%');

            var origH=$(thisv).attr('data-img-height');
            var origW= $(thisv).attr('data-img-width');

            var ratio = (origH*100)/origW;


             if($(thisv).attr('data-zoom')){
                zoom = $(thisv).attr('data-zoom');
             }

            var mw = $(thisv).parent().width() * zoom;
            var mh = $(thisv).parent().height() * zoom;

      
            var newW = (mw*100)/origW;
            var newH = (mh*100)/origH;


            console.log('p Width'+$(thisv).parent().width());
            console.log('p Height'+$(thisv).parent().height());

            console.log('New Width '+newW);
            console.log('New Height'+newH);
            console.log('Orig Width'+origW);
            console.log('Orig Height'+origH);
            console.log('Slide Width'+mw);
            console.log('Slide Height'+mh);
            console.log('Ratio '+ratio);

            hoffset = 0;
            woffset = 0;

            if(origW>=origH){
                if($('.bg-video').parent().width()>$('.bg-video').parent().height()){
                    $(thisv).css('width',mw).css('height',((mw/ratio)*100));
                    
                    var woffset = ($(thisv).height() - mh)/2;
                }
                else{
                    $(thisv).css('height',mh).css('width',((mh/100)*ratio));
                    var hoffset = ($(thisv).width() - mw/2);
                }

            }

            $(thisv).css('top',0 - hoffset);
            $(thisv).css('left', 0 - woffset);


            $(window).scroll();
            
        });
    });

    $(window).resize();

}
