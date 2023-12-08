mergeInto(LibraryManager.library, {
	/**
	* Note: Only call this after a mouse down event in Unity so that it is called BEFORE the next mouse up
	*/
	OpenURL: function(input) {
		const url = UTF8ToString(input);
		const trigger = async function() {
			document.removeEventListener('mouseup', trigger);
			window.open(url, "_blank");
		}

		document.addEventListener('mouseup', trigger);
	},
});