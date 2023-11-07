
  if ($('.mod-m5').length) {
      $('.m5-section-hit').click(function(){
          var isopen;
          if($(this).parent().hasClass('active')){
              isopen =1;
          }

          $('.m5-section').removeClass('active');

          if(!isopen){
            $(this).parent().addClass('active');
          }

          console.log('hit')
      })
  }

  $(window).resize(function() {
   
    if ($('.mod-m5').length) {
        $('.m5-expander').each(function() {
            $(this).css('height', $(this).find('.m5-sizer').innerHeight());
        });

    }
})

$(window).resize();
      