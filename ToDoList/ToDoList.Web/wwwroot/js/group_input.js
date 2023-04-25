'use strict';

$(document).ready(function () {
	function set_correct_group_input(selection) {
		const ids = ['#Group1Input', '#Group2Input', '#Group3Input', '#NoneGroupInput']
		switch (selection) {
			case 'Język angielski':
				ids.forEach(id => id != '#Group2Input' ? $(id).addClass("d-none") : $(id).removeClass("d-none"));
				break;
			case 'Inne':
				ids.forEach(id => id != '#Group3Input' ? $(id).addClass("d-none") : $(id).removeClass("d-none"));
				break;
			case '--Select subject--':
				ids.forEach(id => id != '#NoneGroupInput' ? $(id).addClass("d-none") : $(id).removeClass("d-none"));
				break;
			default:
				ids.forEach(id => id != '#Group1Input' ? $(id).addClass("d-none") : $(id).removeClass("d-none"));
				break;
		}
	}

	$('#Time').prop('disabled', true);
	set_correct_group_input($('#ListItem_SubjectId Option:Selected').text());

	$('#ListItem_SubjectId').change(function () {
		set_correct_group_input($('#ListItem_SubjectId Option:Selected').text());
	});
});
