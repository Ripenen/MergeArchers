mergeInto(LibraryManager.library, {
  InitPurchases: function() {
    initPayments();
  },

  Purchase: function(id) {
    buy(UTF8ToString(id));
  },

  AuthenticateUser: function() {
    auth();
  },

  GetUserData: function() {
    getUserData();
  },

  ShowFullscreenAd: function () {
    showFullscrenAd();
  },

  ShowRewardedAd: function(placement) {
    showRewardedAd(placement);
    return placement;
  },

  OpenWindow: function(link){
    var url = UTF8ToString(link);
      document.onmouseup = function()
      {
        window.open(url);
        document.onmouseup = null;
      }
  },

  GetLang: function() {
    var returnStr = window.ysdk.environment.i18n.lang;
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
},
  Review: function () {
    ShowReview();
  },
   InitPlayerData: function () {
      initPlayerData();
  },
   SetScore: function (key,value) {
       setScore(UTF8ToString(key),value);
  },
   SetData: function (key,value) {
       setData(UTF8ToString(key),UTF8ToString(value));
  },
  GetPurchases: function()
{
getPurchases();
},
  SetLeaderBoard: function(value)
  {
    SetLeader(value);
  }
});