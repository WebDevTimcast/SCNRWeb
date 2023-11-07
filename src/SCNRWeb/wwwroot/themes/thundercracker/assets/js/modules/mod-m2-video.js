/* ===================================================================



                    Initialize Slide Videos



===================================================================*/
  var player = [];
  if($('.bg-video.vimeo').length){
      
         var sldsw=0;
         $('.slideshow').each(function(){//
          console.log('add videos for slideshow '+sldsw);
            
            player[sldsw]=[];
            var i=1;

            $.each($(this).find('.slide'),function(){
                  // console.log('set video i=  '+i);

                    
                  if($(this).find('.bg-video.vimeo').length){
                 //   console.log('has a vimeo video');


                  //console.log('this is vimeo');
                     var options = {
                              loop: true
                          };
                    videoIdvar= $(this).find('.bg-video.vimeo').find('iframe').attr('id').split("vimeo_");
                    videoIdvar= videoIdvar[1];
                      
                     
                    var thisslideshow = sldsw;
                    var thisplayer = String(i);
               
                    player[thisslideshow][thisplayer] =  new Vimeo.Player($("#vimeo_"+videoIdvar), options);
                    


                      var plrobject = player[thisslideshow][thisplayer];
                     // console.log('plrobject parent class').$(plrobject).parent().attr('class');
                
                      plrobject.ready().then(function() {
                       plrobject.playbar=0;
                      plrobject.setVolume(0);
                        
                        plrobject.interval = setInterval(function(){
                          plrobject.getPaused().then(function(paused) {
                            if(paused && $("#vimeo_"+videoIdvar).parent().parent().hasClass('on')){
                                 plrobject.play();
                                 console.log('play this vid');
                            }
                            if(!paused && !$("#vimeo_"+videoIdvar).parent().parent().hasClass('on')){
                                 plrobject.pause();
                                 console.log('pause this vid');
                            }
                          })
                        }, 1000
                        ); 
                       
                      });


                      plrobject.on('timeupdate', function (getAll){

                        //console.log(' timeupdate');
                         plrobject.getPaused().then(function(paused) {
                          if(paused && $("#vimeo_"+videoIdvar).parent().parent().hasClass('on')){
                               plrobject.play();
                               console.log('play this vid');
                          }
                          if(!paused && !$("#vimeo_"+videoIdvar).parent().parent().hasClass('on')){
                               plrobject.pause();
                               console.log('pause this vid');
                          }
                            

                        });
                      });
              

                      //  $("#vimeo_"+videoIdvar).css("opacity","0");

                          plrobject.on('bufferend', function(e) {
                          $(plrobject).css("opacity","1");
                          // console.log('buffer end');
                        });
                           plrobject.on('bufferstart', function(e) {
                           $(plrobject).css("opacity","0");
                           //console.log('buffer start');
                        });


                          plrobject.on('loaded', function(e) {
                         //  $("#vimeo_"+videoIdvar).css("visibility","visible");
                        });
                    
                }
            i++;
          });
          // console.log('the player');
           //console.log(player);

           sldsw++;
      });
    

    buildslideshows();
    console.log('build slideshow with video');
  }
 
  else{
    buildslideshows();
  //  console.log('build slideshow without video');
  }


/* ===================================================================



                      Size Slideshow Videos



===================================================================*/
if($('.slideshow .bg-video').length){

  //$('.mantle .mantle-vid').each(function(){

    $(window).resize(function(){

        $('.slideshow .bg-video').each(function(){
          
           var thisv = $(this);

            var vw = $(thisv).attr('data-img-width');
            var vh = $(thisv).attr('data-img-height');

            var vid_ratio = ((vh*100)/vw)/100;

            var bw = $(this).parent().width();
            var bh = $(this).parent().height();       
            var view_ratio = ((bw*100)/bh)/100;
            console.log('ratio '+vid_ratio);
            console.log('viewratio '+view_ratio);

            var difference = Math.abs(vw-vh);

            var npw = (bh/vid_ratio) + (difference/vid_ratio);

            if(view_ratio >= vid_ratio ){
               if(npw<bw){
                  npw =bw;
               }
                var nph = (bw/vid_ratio);
  
                $(thisv).css('height',nph);
                $(thisv).css('width',npw);
                //console.log('width'+npw);

                $(thisv).css('top',(bh/2)-(nph/2));
                $(thisv).css('left',(bw/2)-(npw/2));
                //console.log('video is wide. H= '+vh+' w='+vw+' ratio='+ratio+' difference='+difference)

            }  
            else{
               // console.log('video is tall');
                $(thisv).height(bh);

                if(nph<bh){
                  nph =bh;
                }
                var npw = (bh*vid_ratio)*100;

                $(thisv).css('width',npw);
                $(thisv).css('top',0);
                $(thisv).css('left',(bw/2)-(npw/2));
              }  
            // console.log('set slideshow h '+$('.slideshow .bg-video').height());
            // console.log('set slideshow w '+$('.slideshow .bg-video').width());
          });
      
    });
            $(window).resize();
   //    });

  }


