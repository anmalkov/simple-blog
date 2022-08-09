function addImageReferenceToTextArea(textareaId, folderName, fileName, fileNameWithExt) {
    var element = document.getElementById(textareaId);
    element.value = "[" + fileName + "]: /blogimages/" + folderName + "/" + fileNameWithExt + " \"" + fileName + "\"\r\n" + element.value;
}
function titleToUrl(titleInputId, urlInputId) {
    var title = document.getElementById(titleInputId).value;
    var url = title.toLowerCase().replace(/ /g, "-");
    document.getElementById(urlInputId).value = url;
}
