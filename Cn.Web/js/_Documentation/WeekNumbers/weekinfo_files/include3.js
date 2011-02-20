
function Caption3() {
  //     JAVASCRIPT INCLUDE FILE - (c) J R Stockton  >= 2006-02-08
  //             http://www.merlyn.demon.co.uk/include3.js
  //       Routines may be copied, but URL must not be linked to.
  //               Needs, for LZ at least, include1.js
  }

var Inc3T=0

// Gregorian Date/Time Utilities :


// Date.prototype.getTimezoneOffset = new Function("with (this) return 0 ")


if (String.prototype && !String.prototype.substr) {
  String.prototype.substr =
    new Function("J", "K", "return this.substring(J, J+K)") }


function DoWstr(DoWk) { // Javascript & ISO indexing
  return "SunMonTueWedThuFriSatSun".substr(3*DoWk, 3) }

var Mon3 =
  ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec']


Date.prototype.ISOlocaltimeStr =
  new Function("  /* Date.ISOlocaltimeStr hh:mm:ss */\n" +
    "  with (this)\n    return" +
    " LZ(getHours())+':'+LZ(getMinutes())+':'+LZ(getSeconds())")


Date.prototype.USlocaltimeStr =
  new Function("  /* Date.USlocaltimeStr hh:mm:ss xM */\n" +
    "  var H\n" +
    "  with (this) return" +
    " LZ(  1+((H=getHours())+11)%12  )+':'+\n      LZ(getMinutes())+':'+" +
    " LZ(getSeconds())+[' AM',' PM'][+(H>11)]")


Date.prototype.ISOlocaldateStr =
  new Function("  /* Date.ISOlocaldateStr YYYY-MM-DD */\n" +
    "  with (this)\n    return " +
    "getFullYear()+'-'+LZ(getMonth()+1)+'-'+LZ(getDate())")


Date.prototype.ISOlocalDTstr =
  new Function("  /* Date.ISOlocalDTstr YYYY-MM-DD hh:mm:ss */\n" +
    "   with (this)\n    return " +
    "ISOlocaldateStr()+' '+ISOlocaltimeStr()")


Date.prototype.UTCstr =
  new Function("  /* Date.UTCstr YYYY-MM-DD hh:mm:ss */\n" +
    "  with (this)\n    return " +
    "getUTCFullYear() + '-' + LZ(getUTCMonth()+1) + '-' +\n" +
    "      LZ(getUTCDate()) + ' ' + LZ(getUTCHours()) + ':' +\n" +
    "      LZ(getUTCMinutes()) + ':' + LZ(getUTCSeconds()) ")


Date.prototype.UTCDstr =
  new Function("  /* Date.UTCDstr Xxx YYYY-MM-DD hh:mm:ss */\n" +
    "  with (this)\n    return " +
    "DoWstr(getUTCDay())+', '+UTCstr()")


Date.prototype.YMDDstr =
  new Function("  /* Date.YMDDstr YYYY-MM-DD Xxx */\n" +
    "  with (this)\n    return " +
    "ISOlocaldateStr() + ' ' + DoWstr(getDay())")


Date.prototype.TZstr = // SIGN SHOULD BE RIGHT
  new Function("  /* Date.TZstr +-hh:mm */\n" +
    "  var X, Y, Z ;\n" +
    "  with (this) {\n" +
    "    X = getTimezoneOffset() ; Y = Math.abs(X) ;\n" +
    "    Z = Y % 60 ; Y = (Y-Z)/60 ;\n" +
    "    return (X>0?'-':'+') + LZ(Y) + ':' + LZ(Z) }")



function ValidDate(y, m, d) { // m = 0..11 ; y m d integers, y!=0
  with (new Date(y, m, d))
    return (getMonth()==m && getDate()==d) /* was y, m */ }


function DateGood(y, m, d) { var D // m = 1..12 ; y m d ints, y!=0
  with (D=new Date(y, --m, d))
    return (getMonth()==m && getDate()==d) ? D : NaN }


function ReadISO8601date(Q) { var T // adaptable for other layouts
  if ((T = /^(\d+)([-\/])(\d\d)(\2)(\d\d)$/.exec(Q)) == null)
    { return -2 } // bad format
  for (var j=1; j<=5; j+=2) T[j] = +T[j] // some use needs numbers
  if (!ValidDate(T[1], T[3]-1, T[5])) { return -1 } // bad value
  return [ T[1], T[3], T[5] ] }



// MJD 40587 = 1970-01-01 GMT = Javascript day 0.
// CMJD 40587 = 1970-01-01 local.


function CMJDtoDob(CMJD) { // local time; CMJD 0 = 1858-11-17 local
  return new Date(1858, 10, 17 + CMJD) }  // ???


function YMDto8601(A) { var S = "-"
  return A[0] + S + LZ(A[1]) + S + LZ(A[2]) }


function YMDtoCMJD(y, m, d) { // m = 1..12
  return 40587 + (Date.UTC(y, m-1, d)/864e5) }


function YMDzuCMJD(y, m, d) { // Fast.
  if (m<3) { m += 12 ; y-- } // Initial m *must* be in 1..12
  return -678973 + d + (((153*m-2)/5)|0) + // 153 = 13 + 5*28
    (1461*y>>2) - ((y/100)|0) + ((y/400)|0) }


function CMJDtoYMD(CMJD) {
  with (new Date((CMJD-40587)*864e5)) {
    return [getUTCFullYear(), getUTCMonth()+1, getUTCDate()] } }


function CMJDzuYMD(CMJD) {
  var Y=0, M=0, t, d = CMJD + 678881 // 0000-03-01 : Julian, add 2
  t = (( (4*(d+36525))/146097 )|0) - 1 // Gregorian Added Rules
  Y += 100*t ; d -= 36524*t + (t>>2)
  t = (( (4*(d+366))/1461 )|0) - 1 // Julian Rules
  Y += t ; d -= 365*t + (t>>2)
  M = ( (5*d+2)/153 )|0 // 153=5*28+13 months from March, M = 0...
  d -= ((2+M*153)/5)|0 // remove full months, d = 0...
  if (M > 9) { M -= 12 ; Y++ }
  return [Y, M+3, ++d] }



// function CMJDtoISOdow(CMJD) { return ((CMJD-40587+77777773)%7)+1 } // ISO

// function WkNoDtoCMJD(Y, W, D) { // ISO 8601
//   var Jan4 = YMDtoCMJD(Y, 1, 4)
//   var DoWk = (Jan4+777772)%7
//   return Jan4 - DoWk + 7*(W-1) + (D-1) }

// function JDNtoCMJD(YYYY, DDD) {
//   return YMDtoCMJD(YYYY, 1, 1) + (DDD-1) }

// function LastSun(y, m, ult) {
//   return ult - (YMDtoCMJD(y, m, ult)+77777773)%7 }

// end of CMJD use


function ZelDoW(y, m, d) { // Sun = 0
  if (m<3) { m += 12 ; y-- }
  return (2 + d + (((13*m-2)/5)|0) +
//    y + ((y/4)|0) - ((y/100)|0) + ((y/400)|0) ) % 7 } // <20060127
    ((5*y)>>2) - ((y/100)|0) + ((y/400)|0) ) % 7 }



function YMD2YWD(y, m, d) { // ISO 8601 WkNo. m = 1..12  (in i3.js)
  var ms1d = 864e5, ms7d = 7*ms1d
  var DC3 = Date.UTC(y, m-1, d+3)/ms1d, DoW = 1 + (DC3+7777777)%7
  var AWN = Math.floor(DC3/7) // an Absolute Week Number
  var Wyr = new Date(AWN*ms7d).getUTCFullYear()
  return [Wyr, AWN - Math.floor(Date.UTC(Wyr, 0, 0)/ms7d), DoW ] }


function CMJDtoYWD(CMJD) { // cf. Calendar FAQ 2.7, Stefan Potthast
  var d4, L, d1, WeekNumber, Bodge, DoW // Bodge, DoW added by JRS
  CJD = CMJD + 2400001 ; DoW = CJD % 7
  d4 = (CJD+31741 - DoW) % 146097 % 36524 % 1461 // 0..1460
  L = +(d4==1460) // was L = (d4/1460)|0
  d1 = ((d4-L) % 365) + L
  WeekNumber = ((d1/7)|0) + 1 // ISO 8601
    Bodge = CJD - 7*(WeekNumber-26) // mid-year
    Bodge = ((Bodge/365.2425)|0) - 4712
  return [Bodge, WeekNumber, 1+DoW] }



function YWDtoCMJD(YWD) { var CJD, y // By JRS 2005-02-23+
  y = YWD[0]-1 // for CJD of Jan 4, modified from YMDzuCMJD
  CJD = 1721429 + (1461*y>>2) - ((y/100)|0) + ((y/400)|0)
  CJD -= CJD%7 // Round down to Monday; CJD 0 is Monday.
  CJD += YWD[1]*7 + +YWD[2] - 8 // Contributions of W and D
  return CJD - 2400001  /* ISO 8601 */ }



function UKtaxWNstr(y, m, d) { var dd, fy = y // m = 1..12
  if (m*32+d<134) fy--
  dd = (Date.UTC(y, m-1, d) - Date.UTC(fy, 3, 6))/864e5
  return 'UK Revenue & Customs:' + ' Year ' + fy + ' week ' +
    LZ(Math.floor(dd/7)+1) + ' day ' + (dd%7+1) + ';' }

function UKtaxMNstr(y, m, d) { var yy = y, mm = m // m = 1..12
  if (d<6) mm--
  if (mm<4) { yy-- ; mm += 12 }
  return ' Year ' + yy + ' month ' + LZ(mm-3) + '.' }



function DaysInMonth(Y, M) { // M=1..12
  with (new Date(Y, M, 1, 12)) {
    setDate(0) ; return getDate() /* OK in NS4? */ } }


function LeapYr(y) { if (y%4) return false
  if (y%100) return true ; return !(y%400) }


function Suffix(j) { return "thstndrd".
  substr("01230000000000000000012300000001".
    charAt(j)*2,2) }


function TimeChangeDates(YYYY) { // Detect OS settings
  var Ton, Tof, JanOff, JulOff, TonMin, TonMax, TofMin, TofMax, K
  with(new Date(YYYY, 00, 01)) {
    TonMin = TofMin = getTime() ; JanOff = getTimezoneOffset()
    setMonth(06) ; JulOff = getTimezoneOffset()
    if (JanOff == JulOff) { return [0,0] }
    setMonth(12) ; TonMax = TofMax = getTime()
    var Min = Math.min(JanOff, JulOff),
        Max = Math.max(JanOff, JulOff)
    Ton = Tof = (TonMin + TofMax)/2
    for (K=0; K<22; K++) {
      setTime(Ton) ; if (getTimezoneOffset() != Max)
        { TonMax = Ton ; Ton = (TonMin+Ton)/2 }
        else
        { TonMin = Ton ; Ton = (Ton+TonMax)/2 }
      setTime(Tof) ; if (getTimezoneOffset() != Min)
        { TofMax = Tof ; Tof = (TofMin+Tof)/2 }
        else
        { TofMin = Tof ; Tof = (Tof+TofMax)/2 }
      }
    }
  Ton = Math.round(Ton/6e4)*6e4 ; Tof = Math.round(Tof/6e4)*6e4
  return {Spring:Ton, Autumn:Tof} }


function TimeFunc(Fns, N) { var Fn, J, K, D0, D1, St, T = new Date()
  St = "TimeFunc: " + N + " Loops\n\nms\tTimes"
  for (J in Fns) { St += "\n" ; Fn = Fns[J] ; if (!Fn) continue
    K = N ; D0 = new Date() ; while (K--) Fn(K) ; D1 = new Date()
    St += (D1-D0) + "\t" + FuncName(Fn) }
  alert(St + "\n\nOverall: " + (new Date() - T) + " ms.") }


var Inc3B=0 // end.

