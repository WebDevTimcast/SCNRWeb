$('[data-fancybox-group="gallery"]').fancybox({
	openEffect	: 'none',
	closeEffect	: 'none',
	padding : 8,
	helpers : {
		title : {
			type : 'inside'
		},
		buttons	: {}
	},

	afterLoad : function() {
		this.title = 'Image ' + (this.index + 1) + ' of ' + this.group.length + (this.title ? ' - ' + this.title : '');
	}

});


$("a.fancy-box").fancybox({
	maxWidth    : 665,
	maxheight    : 600,
	fitToView	: false,
	width		: 'autoSize',
	height		: 'autoSize',
	autoSize	: false,
	closeClick	: false,
	openEffect	: 'none',
	closeEffect	: 'none',

});


$("a.fancy-window").fancybox({

	fitToView	: false,
	autoSize	: true,
	closeClick	: false,
	openEffect	: 'none',
	closeEffect	: 'none',
	type: 'iframe'

});
