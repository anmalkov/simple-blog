function addImageReferenceToTextArea(textareaId, folderName, fileName, fileNameWithExt) {
    var element = document.getElementById(textareaId);
    element.value = "[" + fileName + "]: /blogimages/" + folderName + "/" + fileNameWithExt + " \"" + fileName + "\"\r\n" + element.value;
}
