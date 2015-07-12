var newsMods = [];
function updateNewsMods() {
 for (var i=0; i<newsMods.length; i++) {
  if (newsMods[i]!='') {
   var btn = document.getElementById(newsMods[i]);
   if (btn != null) {btn.click();}
   newsMods[i]='';
   break;
  }
 }
}
function updateNewsModsLoad() {
 Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
 setTimeout('updateNewsMods()',300);
}
function EndRequestHandler() {
 updateNewsMods();
}