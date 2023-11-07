
function buildslideshows(){

    if($('.mod .slideshow').length){
        var sldsw=0;
            //console.log('build slideshow '+sldsw);
        $('.mod .slideshow').each(function(){
           // console.log("add slideshow ");
            var thissldsw=sldsw;

            /* -------------------------------------------------
                         Define slider variables
            ------------------------------------------------ */
            var thisslideshow = this;
            var sidesCnt=$(thisslideshow).find('.slides .slide').length;
          //  console.log("add slideshow cnt="+sidesCnt);
            
            $(thisslideshow).attr('data-shownum',sldsw);
            var actualSidesCnt;
            var slideSizeW;
            var slideSizeH;
            var firstslide = $(thisslideshow).find('.slides li').eq(0);
            var lastslide = $(thisslideshow).find('.slides li').eq($(thisslideshow).find('.slides li').length-1);

           
                $(firstslide).clone().addClass('first-slide').appendTo($(thisslideshow).find('.slides'));
                 $(lastslide).clone().addClass('last-slide').prependTo($(thisslideshow).find('.slides'));

            
            /* -----------------------------------------------------------------------------------------------------------
                Duplicate the firest slide to the end of the slide show and last, used for the illusion in infite slider
            -------------------------------------------------------------------------------------------------------------- */
             if($(thisslideshow).find('.slides li .bg-video')){
                var nfirstslide = $(thisslideshow).find('.slides li').eq($(thisslideshow).find('.slides li').length-1);
                //var newid = $(nfirstslide).find('.mantle-vid').attr('id')+'_dupe';
                //$(nfirstslide).find('.mantle-vid').attr('id',newid);
                //$(nfirstslide).css('background-image',"url('"+$(nfirstslide).attr('data-vidthumb')+"')");
                $(nfirstslide).find('iframe').remove();
                 $(nfirstslide).find('.bg-video').remove();
            
            }
             if($(thisslideshow).find('.slides li .bg-video')){
                var nlastslide = $(thisslideshow).find('.slides li').eq(0);
                //var newid = $(nlastslide).find('.mantle-vid').attr('id')+'_dupe';
                //$(nlastslide).find('.mantle-vid').attr('id',newid);
             
                //   $(nlastslide).css('background-image', "url('"+$(nlastslide).attr('data-vidthumb')+"')");
                
                $(nlastslide).find('iframe').remove();
                $(nlastslide).find('.bg-video').remove();

            }
            

            /* -------------------------------------------------
                            Determine slide length
            ------------------------------------------------ */
            actualSidesCnt = $(thisslideshow).find('.slides li').length;
           // console.log(' actualSidesCnt= '+ actualSidesCnt);

            /* -------------------------------------------------
                 Stop the set up if no slides.
            ------------------------------------------------ */
            if( actualSidesCnt == 0){
                    return;
            }

            /* -------------------------------------------------
                         Build Slide indicator
            ------------------------------------------------ */
            $(thisslideshow).next().find('.indicator').html('');

            for(i=0;i<sidesCnt;i++){
                $(thisslideshow).next().find('.indicator').append('<li></li>');
               // console.log(' add slide indicator dot = '+i);

            }
            
            $(thisslideshow).next().find('indicator > li:nth-child(1)').addClass('on');

            // console.log("hide slides "+sidesCnt);
            if(sidesCnt<=1){
                $(thisslideshow).next().find('.indicator').addClass('t-hide');
                $(thisslideshow).find('slide-nav').addClass('t-hide');
               
            }

                

            /* -------------------------------------------------
                            Set up each slide
            ------------------------------------------------ */
            $.each($(thisslideshow).find('.slides > li'),function(){
                //sidesCnt++;
                var slide = $(this);
                var srcw= slide.attr('data-w');
                var srch= slide.attr('data-h');

                $(window).resize(function(){
                    //Image is wide
                    offset = srch*100/srcw;
                    showW = $(thisslideshow).width();
                    slideSizeW = showW;
                    showH = $(thisslideshow).height();
                    slideSizeH = showH;
                    //console.log('height '+showH);
                    showOffet = showH*100/showW;

                    if(offset>showOffet){
                        slide.attr('data-ratio','wide');
                    }
                    else{
                        slide.attr('data-ratio','tall');
                    }
                    
                });

            });  

           
            /* -------------------------------------------------
                Size .slides based on Sliding direction
            ------------------------------------------------ */
            
            $(window).resize(function(){


                if($('.mod.m2-fillh').length){
                    $('.mod.m2-fillh').each(function(){
                        var indbox;
                        var scrollh;
                        if($('.scrollto').length){
                            scrollh = $('.scrollto').closest('.t-contain').height();
                        }
                        else{
                            scrollh =0;
                        }

                        if($('.indicator-box').length){
                            indbox= 0;//$(this).find('.indicator-box').height();
                        }
                        else{
                            indbox=0;
                        }
                        if($(window).height()>300){
                            $(this).find('.slideshow').css('height', $(window).height() - ($('#t-scrollnav-holder').height() + scrollh + indbox));
                            
                        }
                        else{
                              $(this).find('.slideshow').css('height','auto');
                        }
                    
                    })
                }


                if($(thisslideshow).attr('data-dir') && $(thisslideshow).attr('data-dir')=='vert' ){
                    var slideh= parseInt($(thisslideshow).css('height'));
                    $(thisslideshow).find('.slides').css('height',slideh*actualSidesCnt);
                    if(!$(thisslideshow).find('.slides > li').css('position')!='static'){
                        $(thisslideshow).find('.slides').css('width',parseInt($(thisslideshow).width()));
                       // console.log('slides are floating');
                    }
                    else{
                        $(thisslideshow).find('.slides').css('width','100%');
                       // console.log('slides have no style');
                    }
                    $(thisslideshow).find('.slides > li').css('width',parseInt($(thisslideshow).width()));
                    $(thisslideshow).find('.slides > li').css('height',slideh);

                }
                else{
                   $(thisslideshow).find('.slides').css('height',parseInt($(thisslideshow).height()));
                    
                    var thisslideshow_w=actualSidesCnt;
                    if($(thisslideshow).attr('data-infiniteslide')=="1"){
                        var thisslideshow_w = (actualSidesCnt+1);
                    }

                    if($(thisslideshow).find('.slides > li').css('position')!='static'){
                         $(thisslideshow).find('.slides').css('width',parseInt($(thisslideshow).width())*thisslideshow_w);
                    }
                    else{
                        $(thisslideshow).find('.slides').css('width',$(window).width());
                       // console.log('slides have no style');
                    }


                    $(thisslideshow).find('.slides > li').css('width',parseInt($(thisslideshow).width()));
                }


                 
            });

            $(window).resize();


            //Sliding functionality

            var scnt = sidesCnt;
            var slideNum = 1;
            function slide(thisslideshow,s,ease,thissldsw){
               


                /* -------------------------------------------------
                         Pause Video if sliding away
                ------------------------------------------------ */
            
                if(player && player[thissldsw] && player[thissldsw].length){
                    for (n=1;  n<=player[thissldsw].length; n++){
                        
                        var sn = String(n);
                        if(player[thissldsw] && player[thissldsw][(n+1)] && (player[thissldsw][(n+1)].pauseVideo || player[thissldsw][(n+1)].pause)){

                            if(player[thissldsw][(n+1)].pause){
                               player[thissldsw][(n+1)].pause();
                               console.log('pausing'+thissldsw+',sn'+(n+1));
                            }
                            else{
                                player[thissldsw][n+1].pauseVideo();
                                 console.log('pausing'+thissldsw+',sn'+(n+1));
                                

                            }
                            
                     
                        }
                    }
                }

                slideNum = s;

                /* -------------------------------------------------
                           Check if at end of infinte loop
                ------------------------------------------------ */
                if(ease && (slideNum >-1) && (slideNum <actualSidesCnt)){
                    $(thisslideshow).addClass('ease');
                    setTimeout(function(){ $(thisslideshow).removeClass('ease'); }, 3000);
                }
                else{
                    $(thisslideshow).removeClass('ease');
                }
                //slide slides 


                /* -------------------------------------------------
                                Slider Direction
                ------------------------------------------------ */

                if($(thisslideshow).attr('data-dir') && $(thisslideshow).attr('data-dir')=='vert' ){
                    $(thisslideshow).find('.slides').css('top',(0-(slideSizeH*slideNum)));
                    $(thisslideshow).find('.slides').css('left',0);
                }
                else{
                     $(thisslideshow).find('.slides').css('left',(0-(slideSizeW*slideNum)));
                     $(thisslideshow).find('.slides').css('top',0);
                }


                //show proper image
                $(thisslideshow).find('.slides .slide').removeClass('on');
                $(thisslideshow).next().find('.indicator li').removeClass('on');
                $(thisslideshow).find('.slides li.slide:eq('+(slideNum)+')').addClass('on');
                $(thisslideshow).next().find('.indicator > li:nth-child('+(slideNum)+')').addClass('on');
                var playcheck;


                /* --------------------------------------------------------------
                        Play video if on slide and Autoplay is set to True
                 ---------------------------------------------------------------- */

                /* If scripts loaded before Video API set up interval to keep checking */
                function checkPlay(){
                    if($(thisslideshow).attr('data-autoplay')==1 && $(thisslideshow).find('.slides s-'+(n)+' .bg-video').length && player[thissldsw][String(slideNum+1)].getPlayState() !== 1){
                        player[thissldsw][String(slideNum+1)].playVideo();
                      //  console.log('check play interval');
                    }
                    else{
                        clearInterval(playcheck);
                    }

                }
               
                if(player && player.length && player[thissldsw] && player[thissldsw][String(slideNum)] && $(thisslideshow).attr('data-autoplay')==1 && (player[thissldsw][String(slideNum)].play || player[thissldsw][String(slideNum)].playVideo)){
                          //console.log('player found on  slide '+slideNum);

                            /* Check if Youtube */
                            if(player[thissldsw][String(slideNum)].playVideo){
                                player[thissldsw][String(slideNum)].playVideo();
                                player[thissldsw][String(slideNum)].mute();
                               playcheck = setInterval(checkPlay(), 1500);
                            }
                            else{

                               // console.log('play vimeo player['+thissldsw+']["'+String(slideNum)+'"]');
                                var plrobject = player[thissldsw][String(slideNum)]
                                
                                plrobject.ready().then(function() {
                                    
                                    plrobject.play();
                                });

                            }

                }

                else{
                      // console.log('no video found on slide on slideshow '+thissldsw+', slide '+slideNum);
                }




                /* ---------------------------------------------------------------------------
                        Display Nav arrows depengin on slide and if InfineSliding is set
                 --------------------------------------------------------------------------- */

                $(thisslideshow).find('.slide-nav li').removeClass('mute');

                if(slideNum >= (scnt)){

                    if($(thisslideshow).attr('data-infiniteslide')!="1"){
                        $(thisslideshow).find('.slide-nav .btn-next').addClass('mute');

                    }
                }
                if(slideNum <= 1){
                     if($(thisslideshow).attr('data-infiniteslide')!="1"){
                       $(thisslideshow).find('.slide-nav .btn-prev').addClass('mute');
                    }
                }


                /* -------------------------------------------------------------------------------------
                        If on first or last slide jump to coorisponding for InfinteSliding illusion
                 ------------------------------------------------------------------------------------ */

                if(slideNum <=0){
                    setTimeout(function(){ slide(thisslideshow,actualSidesCnt-2,'',thissldsw); }, 500);
                }

                if(slideNum>=actualSidesCnt-1){
                    setTimeout(function(){ slide(thisslideshow,1,'',thissldsw); }, 500);
                }

                $(thisslideshow).attr('data-slide',s+1);
            }



            /* -------------------------------------------------------------------------------------
                                              Slider nav functions
             ------------------------------------------------------------------------------------ */
           $(thisslideshow).find('.slide-nav .btn-prev').click(function(e){
           // console.log('next');
                e.preventDefault();
                if(slideNum>=0){
                        
                    slide(thisslideshow,slideNum-1,1,thissldsw);
                    clearInterval(autoslideshow);
                }
               
            });

           $(thisslideshow).find('.slide-nav .btn-next').click(function(e){
           // console.log('next');
                e.preventDefault();
                if(slideNum<=(scnt+1)){
                    
                    slide(thisslideshow,slideNum+1,1,thissldsw);
                     clearInterval(autoslideshow);
                }
               
            });


            /* -------------------------------------------------------------------------------------
                                       Indicator functions
             ------------------------------------------------------------------------------------ */
             $(thisslideshow).next().find('.indicator li').click(function() {
                   
                    slide(thisslideshow, $(this).index() + 1, 1,thissldsw);

                   clearInterval(autoslideshow)

                });
                if (sidesCnt < 2) {
                    $(thisslideshow).find('.slide-nav').remove();
                    $(thisslideshow).next('.indicator-box').remove();
                }


            /* -------------------------------------------------------------------------------------
                                         Swipe functionality
             ------------------------------------------------------------------------------------ */
            $(thisslideshow).swipe({

                swipeLeft:function(event, direction, distance, duration, fingerCount) {
                  // console.log('swipe');
                   //  if(($(thisslideshow).attr('data-infiniteslide')!="1" ) && slideNum >= (scnt)){
                       $(thisslideshow).find('.slide-nav .btn-next').click();
                         clearInterval(autoslideshow);
                    // }
                },
                swipeRight:function(event, direction, distance, duration, fingerCount) {
                  // console.log('swipe');
                  // if(($(thisslideshow).attr('data-infiniteslide')!="1") && slideNum>=actualSidesCnt-1){
                        $(thisslideshow).find('.slide-nav .btn-prev').click();
                        clearInterval(autoslideshow);
                   // }
                }
            });

            $(window).resize(function(){
            
                slide(thisslideshow,slideNum,thissldsw);

            });





            /* -------------------------------------------------------------------------------------
                                         Check for Autosliding
             ------------------------------------------------------------------------------------ */
                 //   console.log('check auto slide');
                   // console.log('thisslideshow = '+$(thisslideshow).attr('data-autoslide'));

                    var autoslideshow;
                    /* ========= Auto Transition ============ */
                      $(thisslideshow).on( 'mouseenter', function() {
                              
                               $(thisslideshow).addClass('hover');
                           });
                       $(thisslideshow).on( 'mouseleave', function() {
                              
                               $(thisslideshow).removeClass('hover');
                           });


                    if($(thisslideshow).attr('data-autoslide')==1){
                          // console.log('auto slide');
                             autoslideshow =  setInterval(function(){
                        
                             if(!$(thisslideshow).hasClass('hover')){
                               
                                slide(thisslideshow,slideNum+1,1,thissldsw);

                                //checkelementtones();
                            }
                         }, 10000);

                    }


                    slide(thisslideshow,1,'',1);
            

            sldsw++;
        })

    }

}
