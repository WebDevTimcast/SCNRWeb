//console.log(navigator.userAgent);

var $ = jQuery;
//CSS Class uses to help target styles to avoid performance issues on Safari
if (navigator.userAgent.indexOf('Safari') != -1 && navigator.userAgent.indexOf('Chrome') == -1) {
	$('html').addClass('safari');
}


//Remove the mobile class if not on mobile device (removing instead of adding made for a less jarring experience)
function detectmob() { 
	 if( navigator.userAgent.match(/Android/i)
	 || navigator.userAgent.match(/webOS/i)
	 || navigator.userAgent.match(/iPhone/i)
	 || navigator.userAgent.match(/iPad/i)
	 || navigator.userAgent.match(/iPod/i)
	 || navigator.userAgent.match(/BlackBerry/i)
	 || navigator.userAgent.match(/Windows Phone/i)
	 ){
	  }
	else{
		$('html').removeClass('mobile');
	}
 
}
detectmob();


//listen for device rotation{
function readDeviceOrientation() {
                 		
	  $(window).resize();
	
}

window.onorientationchange = readDeviceOrientation;




