
function Caption1() {
  //     JAVASCRIPT INCLUDE FILE - (c) J R Stockton  >= 2006-04-11
  //             http://www.merlyn.demon.co.uk/include1.js
  //       Routines may be copied, but URL must not be linked to.
  }

var Inc1T=0

var Site = "Merlyn", Owner = "J R Stockton"

function VSF() { document.writeln( // After Michael Donn
  '<a href="view-source:'+this.location+'">View Source File<\/a>') }


// DynWrite(target, text) (Jim Ley) works on controls, after page load :
// it is a computed function :

// Classify browser :

function GetDocVars() { // set 4 Globals; called in include1.js
  nCheck = 0
  if (DocDom = (document.getElementById?true:false))
                                           nCheck++ // NS6 also IE5
  if (DocLay = (document.layers?true:false))
                                           nCheck++ // NS4
  if (DocAll = (document.all?true:false))
                                           nCheck++ // IE4
  }  GetDocVars() // call *here*

// if (nCheck!=1) alert('Browser classification problem!  nCheck = ' +
//  String(nCheck) + '\nPlease let me know how the page works and what' +
//  ' the\nbrowser is; and, if possible, what needs to be done about it.')

function ReportDocVars() {
  if (nCheck==0)
    document.write(' None of DocDom, DocLay, DocAll is set;',
      ' Dynamic Write will fail.'.italics(), '<p>')
  if (DocDom) document.write(' DocDom is set. ')
  if (DocLay) document.write(' DocLay is set. ')
  if (DocAll) document.write(' DocAll is set. ') }

function TableDocVars() {
  document.writeln('<br><br><table summary="Browser class info"',
    ' bgcolor=blue align=center cellpadding=10 border=4>',
    '<tr><th bgcolor=wheat>For the displaying computer :</th>',
    '</tr><tr><td bgcolor=gainsboro align=center>',
    'Your browser gives<br>',
    '<b>DocAll = ', DocAll, ' ; DocDom = ', DocDom,
    ' ; DocLay = ', DocLay, '.</b><br>',
    'Check : ', nCheck, ' browser classification(s) set.',
    '</td></tr></table>') }

function ShowDocVars() { ReportDocVars() ; TableDocVars() }


// Define Function DynWrite(Where, What) to suit browser :
DocStr=''
if (DocLay) DocStr="return document.layers['NS_'+id]"
if (DocAll) DocStr="return document.all[id]"
if (DocDom) DocStr="return document.getElementById(id)"
GetRef=new Function("id", DocStr)

// DocLay = true ; DocAll = DocDom = false // Simulate NS4.7
DynWarn = 0

if (DocStr=='') { DynWrite=new Function("return false") } else {
  if (DocAll || DocDom) {
    DynWrite=new Function("id", "S", "GetRef(id).innerHTML=S; return true")
    }
// if (DocLay) DynWrite=new Function("id", "S", "var x=GetRef(id).document;"+
//  "x.open('text/html'); x.write(S); x.close(); return true")
if (DocLay) DynWrite=new Function(
    "if (0==DynWarn++)"+
    " alert('DynWrite not supported in \".layers\" browsers');"+
    "return false")
 }


function NoDynLay() {
  if (DocLay) document.writeln(
    '<p><i>Dynamic Writing in a ".layers" browser such as yours ',
    'is difficult and I cannot now test it.  Therefore, ',
    'I don\'t now attempt it. An alert will be given in the ',
    'first use in each load/reload of a page.<\/i>') }

// DynWrite() end.

// Alternative :

function SetgEBI() {
  if (document.all && !document.getElementById) { // e.g. IE4
    document.getElementById = function(id) {
      return document.all[id] } } }

function Wryt(ID, S) { document.getElementById(ID).innerHTML = S }

// General Utilities :

function LS(x) { return String(x).replace(/\b(\d)\b/g, '0$1') }

function LZ(x) { return (x>=10||x<0?"":"0") + x }
function Lz(x) { return (x<10&&x>=0?"0":"") + x } // better ?

function LZZ(x) { return x<0||x>=100?""+x:"0"+LZ(x) }

function lz(x) { var t = String(x)
  return t.length==1 ? "0"+t : t } // slower?


if (String.prototype && !String.prototype.substr) {
  String.prototype.substr =
    new Function("J", "K", "return this.substring(J, J+K)") }

function TrimS() { // used for String.prototype.trim  \u00A0?
  return (this.toString() ?
    this.toString().replace(/\s+$|^\s+/g, "") : "") }
String.prototype.trim = TrimS


function Sign(X) { return X>0 ? "+" : X<0 ? "-" : " " }

function Sgnd(X) { return Sign(X) + Math.abs(X) }

function PrfxTo(S, L, C) { var R = S + ""
  if (C.length>0) while (R.length<L) R = C + R ; return R }

function SpcsTo(S, L) { var R = S + "" // case of PrfxTo
  while (R.length<L) R = " " + R ; return R }

function StrU(X, M, N) { // X >= 0.0 ; gives M digits point N digits
  var S = String(Math.round(X*Math.pow(10, N)))
  if (/\D/.test(S)) return SpcsTo(X, M+N+1) // cannot cope
  S = PrfxTo(S, M+N, '0') ; var T = S.length - N
  return S.substring(0, T) + '.' + S.substring(T) }

function StrS(X, M, N) { return Sign(X) + StrU(Math.abs(X), M, N) }

function StrT(X, M, N) { return SpcsTo(StrU(X, 1, N), M+N+2) }

function StrW(X, M, N) { return SpcsTo(StrS(X, 1, N), M+N+2) }

if (!Number.toFixed) { // 20030313
  Number.prototype.toFixed = // JL
    new Function("X",
      "  /* toFixed */ if (!X) X=0\n  return StrS(this, 1, X)") }


function OldSigFigNum(X, N) { // returns a Number
  var p = Math.pow(10, N-Math.ceil(Math.log(Math.abs(X))/Math.LN10))
  return isFinite(p) ? Math.round(X*p)/p : X }


function OldSigFigExp(X, N) { // N<22 , returns a String
  if (X==0) return OldSigFigExp(1, N).replace(/\+1/, ' 0')
  var p = Math.floor(Math.log(Math.abs(X))/Math.LN10)
  if (!isFinite(p)) return X
  return (X>0?'+':"") + String(
    Math.round(X*Math.pow(10, N-p-1))).replace(/(\d)/, "$1.") +
    (p>=0?"e+":"e-") + LZ(Math.abs(p)) } // All OK?



function Expo(E) { return "e" + (E<0?'-':'+') + LZ(Math.abs(E)) }

function GetSEM(X) { // returns Sign Mantissa Exponent (base 10)
  var U, Obj = { S : Sign(X), E : 0, M : X==U?U:Math.abs(X) }
  with (Obj) { if (M==0 || !isFinite(M)) return Obj
    while (M >= 10) { E++ ; M /= 10 }
    while (M < 1.0) { E-- ; M *= 10 } }
  return Obj }

function NumDecSigFig(X, N) { var U // returns a Number
  if (X==0||X==U) return X
  with (GetSEM(X)) var P = Math.pow(10, N-E-1)
  return Math.round(X*P)/P }

function StrSigFigFxd(X, N) { var U // ?? for 2 <= N <= 16 ??
  if (X==U) return " " + U
  with (GetSEM(X)) { var P, Q
    P = Math.pow(10, N-E-1) ; Q = Math.round(Math.abs(X)*P)/P
    return S + StrU(Q, 1, Math.max(1, N-E-1)) } }

function StrSigFigExp(X, N) { // ?? for 2 <= N <= 16 ??
  with (GetSEM(X))
    return S + StrU(M, 1, N-1) + (isFinite(M) ? Expo(E) : " ") }


SigFigNum = NumDecSigFig // OldSigFigNum
SigFigFxd = StrSigFigFxd 
SigFigExp = StrSigFigExp // OldSigFigExp


function SigFigAre() { var IzN = " is now"
  document.write("Across this site,",
    "\nSigFigNum", IzN, FuncName(SigFigNum),
    "\nSigFigFxd", IzN, FuncName(SigFigFxd),
    "\nSigFigExp", IzN, FuncName(SigFigExp)) }



function Div(X, Y) { return Math.floor(X/Y) /* full range */ }

function Mod(X, Y) { return X - Math.floor(X/Y)*Y }


function FuncName(Fn) { // Fn is a function; return space name
  return Fn.toString().match(/( \w+)/)[0] }


function Uinp(Ctrl) { return +eval(Ctrl.value) /* for exprns */ }

function GetNum(X) { return +eval(X.value.replace(/[ ]/g, '')) }

function RadBtns(Rbtn, Arr) { var Q, J=0
  while (Q=Rbtn[J]) { if (Q.checked) return Arr[J] ; J++ } }



// After LRN, altered ( old ones moved to js-boxes 20050527) : 

var BoxW=69

function xSafeHTML(S) { return S.replace(/&/g, "&amp;"). 
    replace(/</g, "&lt;").replace(/>/g, "&gt;") }

function SafeHTML(S) {
  return S.split("&").join("&amp;").split("<").join("&lt;").
    split(">").join("&gt;") } // IAS

function Depikt(X, Y, S, Hue) { // String S is shown in a box
  document.writeln(
    "<p align=center>\n<textarea readonly wrap=virtual",
    " style=\"border: double thick ", Hue || "black", " ;\"",
    " cols=", X, " rows=", Y, ">\n", SafeHTML(S),
    "<\/textarea><\/p>") }

function ShoTrim(St) {
  var match = /\S([\s\S]*\S)*/.exec(St)
  return match ? match[0] : " eh? " }

function ShoLinCnt(St, Mx) { // counts lines when wrapped at     lineLength Mx
  var Ch, j = xj = CR = NL = 0, Len = St.length, X = 1
  while (j<Len) { Ch = St.charCodeAt(j++)
    if (Ch!=10 && Ch!=13) continue
    if (Ch==10) NL++
    if (Ch==13) CR++
    X += Math.max(Math.floor((j-xj-2)/Mx), 0) ; xj = j }
  // In principle, not quite right, as MSIE wraps at whitespace
  return X + Math.max(NL, CR) }

function ShoGen(A) { // see ShoFFF
  var St = "", Len = A.length, j = 0, Vis = 1, Arg, LC = 0
  while (1) {
    Arg = A[j++]
    if (!Arg) { Vis = 0 ; continue }
    Arg = ShoTrim(Arg.toString())
    St += Arg ; if (Vis) LC += ShoLinCnt(Arg, BoxW)
    if (j==Len) break
    St += "\n\n" ; LC += Vis } 
  return {Str:St, Cnt:LC} }

function ShoFFF() { // Args may be functions, but 0 ends visibles
  with (ShoGen(arguments)) Depikt(BoxW, Cnt, Str, "lightgreen") }

function ShoCod(Fn) { // N.B. Fn() should be called externally
  var St = ShoTrim(Fn.toString())
  Depikt(BoxW, ShoLinCnt(St, BoxW), St, "red") }

function ShoOut(Fn) { // N.B. this calls  Fn()
  document.writeln("<pre class=OUT>")
  Fn()
  document.writeln("<\/pre>") }

function ShoDoo(Fn) { // N.B. this calls  Fn() - ADD show others ?
  ShoCod(Fn) ; Fn() }

function ShoDuu(Fn) { // N.B. this calls  Fn() - ADD show others ?
  ShoCod(Fn) ; ShoOut(Fn) }

// .


function eIVSF() {
  Depikt(47, 2, "  For the code, view the source of this page,\n" +
                "     and any include files which it calls.") }


var BID = 0, BoxX = 70


function PopThis(btn) { var Obj = ShoGen(btn.btnargs)
  // Height and Width values need improving
  var Wndw = window.open("", "X"+ +new Date(),
    "height=" + (16*Obj.Cnt+30) + ",width=" + (8*BoxX+20) +
    ",resizable,scrollbars")
  Wndw.document.write("<pre>\n", SafeHTML(Obj.Str), "\n<\/pre>")
  Wndw.document.close() /* DU */ }

function PopBtn() { var I = 'JJ' + BID++ // var BID = 0 precedes
  document.write("<form name=", I, ">",
    "<input type=button name=N value='Pop Code Up'",
    " onClick='PopThis(this)'><\/form>")
  document.close() // DU // ??
  document.forms[I].elements["N"].btnargs = arguments }




function NewPage(Title, Body) { // Same window. Passes TIDY check.
  document.writeln('<!DOCTYPE HTML PUBLIC' +
    ' "-//W3C//DTD HTML 4.01 Transitional//EN"\n' +
    ' "http://www.w3.org/TR/html4/strict.dtd">\n' +
    '<HTML lang="en">\n<HEAD>\n' +
    '<META HTTP-EQUIV="Content-Type"' +
    ' CONTENT="text/html; charset=ISO-8859-1">\n' +
    '<TITLE>' + Site + ' -\n ' +
    Title + '\n  - ' + Owner + '<\/TITLE>\n<\/HEAD>\n<BODY>\n\n' +
    Body + "\n\n<hr>\n<\/BODY>\n</\HTML>\n") }




var Inc1B=0 // end.

