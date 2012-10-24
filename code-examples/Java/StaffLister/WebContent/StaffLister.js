$(document).ready(function() {
	$.post('StaffLister', function(responseData) {
			$('#locationlist').html(responseData);
			$('#locationselect').change(function() {
				updateTable(1, $(this).val());
			});
		},
		"html");
});

function updateTable(pageNumber, locationId) {
	$.get('StaffLister',
		{page: pageNumber, location: locationId},
		function(responseData) {
			$('#staffview').html(responseData);
		},
		"html");
}