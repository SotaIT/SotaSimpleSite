using System;

namespace Sota.Web.SimpleSite.Resources 
{
	/// <summary>
	/// Summary description for AdminCss.
	/// </summary>
	public class AdminScriptHandler: System.Web.IHttpHandler
	{
		public AdminScriptHandler()
		{
		}
		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(System.Web.HttpContext context)
		{
			context.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
			context.Response.ContentType = "application/x-javascript";
			switch(context.Request.QueryString["script"])
			{
				case "comboboxdatepicker.js":
					context.Response.Cache.SetLastModified(new DateTime(2006,8,27));
					context.Response.Write(
@"/*---------------------------------+
|State-Of-The-Art ASP.NET Controls |
|http://www.donhost.ru/            |
+----------------------------------*/
function CBDatePicker_getMonthLen(theYear, theMonth)
{
	var dPrevDate = new Date(theYear, theMonth+1, 0);
	return dPrevDate.getDate();
}
function CBDatePicker_Refresh(id)
{
	if(CBDatePicker_IsNull(id))
		return;
		
	var theMonth	= document.getElementById(id+'_Month').value-1;
	var theYear		= document.getElementById(id+'_Year').value;
	var howMany		= CBDatePicker_getMonthLen(theYear, theMonth);
	var off			= 0;
	
	var cmbDay = document.getElementById(id+'_Day');
	if(cmbDay.options[0].value=='-1')
	{
		off = 1;
		howMany++;
	}
	if(howMany != cmbDay.options.length)
	{
		var oldVal   = cmbDay.selectedIndex == -1 ? off : cmbDay.selectedIndex;
		if(oldVal > howMany-1)
		{
			oldVal = howMany-1;
		}
		if(howMany > cmbDay.options.length)
		{
			for(var i = cmbDay.options.length; i < howMany; i++)
			{
				var day			= i+1-off;
				var oOption		= document.createElement('OPTION');
				oOption.value	= day;
				oOption.text	= day<10 ? '0'+day.toString() : day;
				cmbDay.options[cmbDay.options.length] = oOption;
			}
		}
		else
		{
			cmbDay.options.length = howMany;
		}
		if(cmbDay.selectedIndex != oldVal)
		{
			cmbDay.selectedIndex = oldVal;
		}
	}
    CBDatePicker_CorrectDate(id);
}

function CBDatePicker_RePaintYear(id, year)
{
    var minY    = document.getElementById(id+'_MinHidden').value.split('.')[2];
    var maxY    = document.getElementById(id+'_MaxHidden').value.split('.')[2];

	var cmbYear = document.getElementById(id+'_Year');
	
	var y = parseInt(year);
	var ymax = Math.min(y+5, maxY)+1;
	var ymin = Math.max(y-5, minY);
	if(cmbYear.options[0].value=='-1')
		cmbYear.options.length = 1;
	else
		cmbYear.options.length = 0;
	var index = 0;
	for(i=ymin;i<ymax;i++)
	{
		var oOption = document.createElement('OPTION');
		oOption.value = i;
		oOption.text = i;
		if(i == y)
		{	
			index = cmbYear.options.length;
		}
		cmbYear.options[cmbYear.options.length] = oOption;
	}
	cmbYear.selectedIndex = index;
}
function CBDatePicker_CorrectDate(id)
{
	if(CBDatePicker_IsNull(id))
		return;

    var minH    = document.getElementById(id+'_MinHidden').value.split('.');
    var minDate = new Date(minH[2], minH[1]-1, minH[0]);
    var maxH    = document.getElementById(id+'_MaxHidden').value.split('.');
    var maxDate = new Date(maxH[2], maxH[1]-1, maxH[0]);
    var cmbDay  =  document.getElementById(id+'_Day');
    var cmbMonth  =  document.getElementById(id+'_Month');
    var cmbYear  =  document.getElementById(id+'_Year');
    var curDate = new Date(cmbYear.value, cmbMonth.value-1, cmbDay.value);
    if(curDate>maxDate)
    {
        CBDatePicker_SetSelectValue(cmbYear, maxDate.getFullYear());
        CBDatePicker_SetSelectValue(cmbMonth, maxDate.getMonth()+1);
        CBDatePicker_SetSelectValue(cmbDay, maxDate.getDate());
        CBDatePicker_Refresh(id);
    }
    else if(curDate<minDate)
    {
        CBDatePicker_SetSelectValue(cmbYear, minDate.getFullYear());
        CBDatePicker_SetSelectValue(cmbMonth, minDate.getMonth()+1);
        CBDatePicker_SetSelectValue(cmbDay, minDate.getDate());
        CBDatePicker_Refresh(id);
    }
}
function CBDatePicker_SetSelectValue(lst, val)
{
    var n = lst.options.length;
	for(var i=0;i<n;i++)
	{
		if(lst.options[i].value==val)
		{
			lst.selectedIndex = i;
		}
	}
}
function CBDatePicker_IsNull(id)
{
	var cmbMonth = document.getElementById(id+'_Month');
	var cmbYear  = document.getElementById(id+'_Year');
	var cmbDay = document.getElementById(id+'_Day');
	return (cmbDay.value=='-1') || (cmbMonth.value=='-1') || (cmbYear.value=='-1');
}
function CBDatePicker_SetNull(id)
{
	var cmbMonth	= document.getElementById(id+'_Month');
	var cmbYear		= document.getElementById(id+'_Year');
	var cmbDay		= document.getElementById(id+'_Day');
	cmbMonth.selectedIndex	= 0; 
	cmbYear.selectedIndex	= 0;
	cmbDay.selectedIndex	= 0;
}
function CBDatePicker_UnSetNull(id)
{
	var cmbMonth	= document.getElementById(id+'_Month');
	var cmbYear		= document.getElementById(id+'_Year');
	var cmbDay		= document.getElementById(id+'_Day');
	if(cmbDay.value=='-1')
	{
		cmbDay.selectedIndex	= 1;
	}
	if(cmbMonth.value=='-1')
	{
		cmbMonth.selectedIndex	= 1;
	}
	if(cmbYear.value=='-1')
	{
		cmbYear.selectedIndex	= 1;
	}
}
function CBDatePicker_DayChanged(id)
{
	var cmbDay = document.getElementById(id+'_Day');
	if(cmbDay.value=='-1')
	{
		CBDatePicker_SetNull(id);
		return false;
	}
	CBDatePicker_UnSetNull(id);
	CBDatePicker_CorrectDate(id);
}
function CBDatePicker_MonthChanged(id)
{
	var cmbMonth = document.getElementById(id+'_Month');
	if(cmbMonth.value=='-1')
	{
		CBDatePicker_SetNull(id);
		return false;
	}
	CBDatePicker_UnSetNull(id);
	CBDatePicker_Refresh(id);
}
function CBDatePicker_YearChanged(id)
{
	var cmbYear  = document.getElementById(id+'_Year');
	if(cmbYear.value=='-1')
	{
		CBDatePicker_SetNull(id);
		return false;
	}
	CBDatePicker_UnSetNull(id);
	CBDatePicker_RePaintYear(id, cmbYear.value);
	CBDatePicker_Refresh(id);
}
"
						);
					break;
					//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				case "common.js":
					context.Response.Cache.SetLastModified(new DateTime(2006,12,9));
					context.Response.Write(
@"function Nothing()
{
}
var dialogWindow = null;
function OpenDialog(url,name,w,h)
{	
	if(dialogWindow)
	{
		dialogWindow.close();
		dialogWindow = null;
	}
	var b = document.body;
	var l =	b.clientWidth/2+b.clientLeft-w/2;
	var t =	b.clientHeight/2+b.clientTop-h/2;
	dialogWindow = window.open(url,name,'width='+w+',height='+h+',left='+l+',top='+t+',resizable=yes,status=no,scrollbars=no');
	return dialogWindow;
}
function OpenModalDialog(url,args,w,h)
{
	return showModalDialog(url,args,'dialogWidth:'+w+'px;dialogHeight:'+h+'px;help:no;status:no;scroll:no;resizable:yes;');
}
function OpenModelessDialog(url,args,w,h)
{
	return showModelessDialog(url,args,'dialogWidth:'+w+'px;dialogHeight:'+h+'px;help:no;status:no;scroll:no;resizable:yes;');
}
function getE(id)
{
	return document.getElementById(id);
}
function paste(t1,t2, textBox)
{
	if (document.selection) 
	{
		textBox.focus();
		var txt = textBox.value;
		var str = document.selection.createRange();
		if (str.text == '') 
		{
			str.text = t1 + t2;
		} 
		else 
			if (txt.indexOf(str.text)>=0) 
			{
				str.text = t1 + str.text + t2;
			} 
			else 
			{
				textBox.value = txt + t1 + t2;
			}
	}
}

function SetCookie(cookieName, cookieValue, expires, path, domain, secure) {
	document.cookie =
		escape(cookieName) + '=' + escape(cookieValue)
		+ (expires ? '; expires=' + expires.toGMTString() : '')
		+ (path ? '; path=' + path : '')
		+ (domain ? '; domain=' + domain : '')
		+ (secure ? '; secure' : '');
}
function GetCookie(cookieName) {
	var cookieValue = '';
	var posName = document.cookie.indexOf(escape(cookieName) + '=');
	if (posName != -1) {
		var posValue = posName + (escape(cookieName) + '=').length;
		var endPos = document.cookie.indexOf(';', posValue);
		if (endPos != -1) cookieValue = unescape(document.cookie.substring(posValue, endPos));
		else cookieValue = unescape(document.cookie.substring(posValue));
	}
	return (cookieValue);
}
function RemoveCookie(cookieName){
	var now = new Date();
	var yesterday = new Date(now.getTime() - 1000 * 60 * 60 * 24);
	SetCookie(cookieName, 'deleted', yesterday);
}
function cmnInformation()
{
	this.sUser_agent = navigator.userAgent.toLowerCase();
	this.bIE       = ( ( this.sUser_agent.indexOf('msie') != -1 ) && ( this.sUser_agent.indexOf('opera') == -1 ) );
	this.bOpera    = ( this.sUser_agent.indexOf('opera') != -1 );
	this.bMAC      = ( this.sUser_agent.indexOf('mac') != -1 );
	this.bGecko    = ( navigator.product == 'Gecko' );
	this.bMozilla  = this.bGecko;
	this.sLanguage = null;
	this.bHTTP = null;
	return this;
}
var cmn_oInformation = new cmnInformation();

function cmnInit_Information(){
	cmn_oInformation.sLanguage = ( document.body && document.body.getAttribute( 'lang' ) != '' ) ? document.body.getAttribute( 'lang' ) : 'ru';
	cmn_oInformation.bHTTP = ( document.location.href.indexOf('http://') == 0 ) ? true : false;
}

cmnAdd_event( window, 'load', cmnInit_Information );


function cmnMatch_class( eOn, sClass_name ){
	return ( sClass_name && eOn.className && eOn.className.length && eOn.className.match( new RegExp('(^|\\s+)(' + sClass_name +')($|\\s+)') ) );
}

function cmnAdd_event( eOn, sEvent_type, ptrFunction ){
	if( eOn.addEventListener ){  
		eOn.addEventListener( sEvent_type, ptrFunction, false );
	}else if(eOn.attachEvent){
		eOn.attachEvent( 'on' + sEvent_type, ptrFunction );
	}
}

cmnAdd_event( window, 'load', atkAllow_tab_key_in_text_inputs );

function atkAllow_tab_key_in_text_inputs()
{
	var aeText_input = document.getElementsByTagName('TEXTAREA');
	for( var i = 0 ; i < aeText_input.length ; i++ )
	{
		if(cmnMatch_class(aeText_input[i],'allow_tab_key') && aeText_input[i].bTab_pressed != false)
		{
			atkAllow_tab_key_for( aeText_input[i] );
		}
	}
}

atk_aeText_input = new Array();

function atkAllow_tab_key_for( eInput ){
	if( cmn_oInformation.bIE ){
		cmnAdd_event( eInput, 'keydown',
			function(e){
				if( window.event.keyCode == 9 ){
					var etcRange = document.selection.createRange();
					if( etcRange.text.length ){
						if( window.event.shiftKey ){
							etcRange.text = atkRemove_tabs( etcRange.text );
						}else{
							etcRange.text = atkInsert_tabs( etcRange.text );
						}
					}else{
						etcRange.text = '\t';
					}
					return false;
				}
			}
		);
	}else if( eInput && eInput.selectionStart ){
		var i = atk_aeText_input.length;
		atk_aeText_input[i] = eInput;
		cmnAdd_event( eInput, 'keydown',
			function(e){
				if( e.keyCode == 9 ){
					this.bTab_pressed = true;
					var iScroll_top = this.scrollTop;
					var iStart = this.selectionStart;
					var sA = this.value.substring( 0, iStart );
					var sB = this.value.substring( iStart, this.selectionEnd );
					var bSelection = false;
					var sC = this.value.substring( this.selectionEnd, this.value.length );
					if( sB.length ){
						bSelection = true;
						if( e.shiftKey ){
							sB = atkRemove_tabs( sB );
						}else{
							sB = atkInsert_tabs( sB );
						}
					}else{
						sB = '\t';
					}
					this.value = sA + sB + sC;
					this.focus();
					if( bSelection ){
						this.selectionStart = iStart;
						this.selectionEnd = iStart + sB.length;
					}else{
						this.selectionStart = ++iStart;
						this.selectionEnd = iStart;
					}
					this.scrollTop = iScroll_top;
				}
			}
		);
		cmnAdd_event( eInput, 'blur',
			function(e){
				if( this.bTab_pressed ){
					this.bTab_pressed = false;
					setTimeout( 'atk_aeText_input[' + i + '].focus()', 1 );
				}
			}
		);
	}
	eInput.bTab_pressed = false;
}

function atkRemove_tabs( sText ){
	return sText.replace( /(^|\n)\t/g, '$1' );
}

function atkInsert_tabs( sText ){
	return sText.replace( /(^|\n)([\t\S])/g, '$1\t$2' );
}

var bAllow_tab_key_script_loaded = true;

function ctrlS_Save(e)
{
    if(ctrlSEvent)
    {
		var k = null;
		var ctrl = null;
		if (e)
		{
			ctrl = e.ctrlKey;
			k = e.which;
		}
		else
		{
			k = event.keyCode;
			ctrl = event.ctrlKey;
		}
		if (k==83 && ctrl) 
		{
			eval(ctrlSEvent);
		}
    }
}
var ctrlSEvent = null;
document.onkeydown = ctrlS_Save;
"
						);
					break;
					//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				case "treeview.js":
					context.Response.Cache.SetLastModified(new DateTime(2006,7,8));
					string img = Path.VRoot+"admin.ashx?img=";
					context.Response.Write(
						@"/*--------------------------------------------------|
| dTree 2.05 | www.destroydrop.com/javascript/tree/ |
|---------------------------------------------------|
| Copyright (c) 2002-2003 Geir Landrö               |
|                                                   |
| This script can be used freely as long as all     |
| copyright messages are intact.                    |
|                                                   |
| Updated: 17.04.2003                               |
|--------------------------------------------------*/

// Node object
function Node(id, pid, name, url, title, target, icon, iconOpen, open) {
	this.id = id;
	this.pid = pid;
	this.name = name;
	this.url = url;
	this.title = title;
	this.target = target;
	this.icon = icon;
	this.iconOpen = iconOpen;
	this._io = open || false;
	this._is = false;
	this._ls = false;
	this._hc = false;
	this._ai = 0;
	this._p;
};

// Tree object
function dTree(objName, rootIcon) {
if(rootIcon)
{}
else
	rootIcon = null;
	
	this.config = {
		target					: null,
		folderLinks			: true,
		useSelection		: true,
		useCookies			: true,
		useLines				: true,
		useIcons				: true,
		useStatusText		: false,
		closeSameLevel	: false,
		inOrder					: false
	}	this.icon = {
		root				: rootIcon==null ? '<%=Img%>base.gif' : rootIcon,
		folder			    : '<%=Img%>folder.gif',
		folderOpen	        : '<%=Img%>folderopen.gif',
		node				: '<%=Img%>page.gif',
		empty				: '<%=Img%>empty.gif',
		line				: '<%=Img%>line.gif',
		join				: '<%=Img%>join.gif',
		joinBottom	        : '<%=Img%>joinbottom.gif',
		plus				: '<%=Img%>plus.gif',
		plusBottom	        : '<%=Img%>plusbottom.gif',
		minus				: '<%=Img%>minus.gif',
		minusBottom	        : '<%=Img%>minusbottom.gif',
		nlPlus			    : '<%=Img%>nolines_plus.gif',
		nlMinus			    : '<%=Img%>nolines_minus.gif'
	};
	this.obj = objName;
	this.aNodes = [];
	this.aIndent = [];
	this.root = new Node(-1);
	this.selectedNode = null;
	this.selectedFound = false;
	this.completed = false;
};

// Adds a new node to the node array
dTree.prototype.add = function(id, pid, name, url, title, target, icon, iconOpen, open) {
	this.aNodes[this.aNodes.length] = new Node(id, pid, name, url, title, target, icon, iconOpen, open);
};

// Open/close all nodes
dTree.prototype.openAll = function() {
	this.oAll(true);
};
dTree.prototype.closeAll = function() {
	this.oAll(false);
};

// Outputs the tree to the page
dTree.prototype.toString = function() {
	var str = '<div class=dtree>\n';
	if (document.getElementById) {
		if (this.config.useCookies) this.selectedNode = this.getSelected();
		str += this.addNode(this.root);
	} else str += 'Browser not supported.';
	str += '</div>';
	if (!this.selectedFound) this.selectedNode = null;
	this.completed = true;
	return str;
};

// Creates the tree structure
dTree.prototype.addNode = function(pNode) {
	var str = '';
	var n=0;
	if (this.config.inOrder) n = pNode._ai;
	for (n; n<this.aNodes.length; n++) {
		if (this.aNodes[n].pid == pNode.id) {
			var cn = this.aNodes[n];
			cn._p = pNode;
			cn._ai = n;
			this.setCS(cn);
			if (!cn.target && this.config.target) cn.target = this.config.target;
			if (cn._hc && !cn._io && this.config.useCookies) cn._io = this.isOpen(cn.id);
			if (!this.config.folderLinks && cn._hc) cn.url = null;
			if (this.config.useSelection && cn.id == this.selectedNode && !this.selectedFound) {
					cn._is = true;
					this.selectedNode = n;
					this.selectedFound = true;
			}
			str += this.node(cn, n);
			if (cn._ls) break;
		}
	}
	return str;
};

// Creates the node icon, url and text
dTree.prototype.node = function(node, nodeId) {
	var str = '<div class=dTreeNode>' + this.indent(node, nodeId);
	if (this.config.useIcons) {
		if (!node.icon) node.icon = (this.root.id == node.pid) ? this.icon.root : ((node._hc) ? this.icon.folder : this.icon.node);
		if (!node.iconOpen) node.iconOpen = (node._hc) ? this.icon.folderOpen : this.icon.node;
		if (this.root.id == node.pid) {
			node.icon = this.icon.root;
			node.iconOpen = this.icon.root;
		}
		str += '<img id=""i' + this.obj + nodeId + '"" src=""' + ((node._io) ? node.iconOpen : node.icon) + '"" alt="""" />';
	}
	if (node.url) {
		str += '<a id=""s' + this.obj + nodeId + '"" class=""' + ((this.config.useSelection) ? ((node._is ? 'nodeSel' : 'node')) : 'node') + '"" href=""' + node.url + '""';
		if (node.title) str += ' title=""' + node.title + '""';
		if (node.target) str += ' target=""' + node.target + '""';
		if (this.config.useStatusText) str += ' onmouseover=""window.status=\'' + node.name + '\';return true;"" onmouseout=""window.status=\'\';return true;"" ';
		if (this.config.useSelection && ((node._hc && this.config.folderLinks) || !node._hc))
			str += ' onclick=""javascript: ' + this.obj + '.s(' + nodeId + ');""';
		str += '>';
	}
	else if ((!this.config.folderLinks || !node.url) && node._hc && node.pid != this.root.id)
		str += '<a href=""javascript: ' + this.obj + '.o(' + nodeId + ');"" class=""node"">';
	str += node.name;
	if (node.url || ((!this.config.folderLinks || !node.url) && node._hc)) str += '</a>';
	str += '</div>';
	if (node._hc) {
		str += '<div id=""d' + this.obj + nodeId + '"" class=""clip"" style=""display:' + ((this.root.id == node.pid || node._io) ? 'block' : 'none') + ';"">';
		str += this.addNode(node);
		str += '</div>';
	}
	this.aIndent.pop();
	return str;
};

// Adds the empty and line icons
dTree.prototype.indent = function(node, nodeId) {
	var str = '';
	if (this.root.id != node.pid) {
		for (var n=0; n<this.aIndent.length; n++)
			str += '<img src=""' + ( (this.aIndent[n] == 1 && this.config.useLines) ? this.icon.line : this.icon.empty ) + '"" alt="""" />';
		(node._ls) ? this.aIndent.push(0) : this.aIndent.push(1);
		if (node._hc) {
			str += '<a href=""javascript: ' + this.obj + '.o(' + nodeId + ');""><img id=""j' + this.obj + nodeId + '"" src=""';
			if (!this.config.useLines) str += (node._io) ? this.icon.nlMinus : this.icon.nlPlus;
			else str += ( (node._io) ? ((node._ls && this.config.useLines) ? this.icon.minusBottom : this.icon.minus) : ((node._ls && this.config.useLines) ? this.icon.plusBottom : this.icon.plus ) );
			str += '"" alt="""" /></a>';
		} else str += '<img src=""' + ( (this.config.useLines) ? ((node._ls) ? this.icon.joinBottom : this.icon.join ) : this.icon.empty) + '"" alt="""" />';
	}
	return str;
};

// Checks if a node has any children and if it is the last sibling
dTree.prototype.setCS = function(node) {
	var lastId;
	for (var n=0; n<this.aNodes.length; n++) {
		if (this.aNodes[n].pid == node.id) node._hc = true;
		if (this.aNodes[n].pid == node.pid) lastId = this.aNodes[n].id;
	}
	if (lastId==node.id) node._ls = true;
};

// Returns the selected node
dTree.prototype.getSelected = function() {
	var sn = this.getCookie('cs' + this.obj);
	return (sn) ? sn : null;
};

// Highlights the selected node
dTree.prototype.s = function(id) {
	if (!this.config.useSelection) return;
	var cn = this.aNodes[id];
	if (cn._hc && !this.config.folderLinks) return;
	if (this.selectedNode != id) {
		if (this.selectedNode || this.selectedNode==0) {
			eOld = document.getElementById(""s"" + this.obj + this.selectedNode);			if(eOld)
			{
				eOld.className = ""node"";			}
		}
		eNew = document.getElementById(""s"" + this.obj + id);
		eNew.className = ""nodeSel"";
		this.selectedNode = id;
		if (this.config.useCookies) this.setCookie('cs' + this.obj, cn.id);
	}
};

// Toggle Open or close
dTree.prototype.o = function(id) {
	var cn = this.aNodes[id];
	this.nodeStatus(!cn._io, id, cn._ls);
	cn._io = !cn._io;
	if (this.config.closeSameLevel) this.closeLevel(cn);
	if (this.config.useCookies) this.updateCookie();
};

// Open or close all nodes
dTree.prototype.oAll = function(status) {
	for (var n=0; n<this.aNodes.length; n++) {
		if (this.aNodes[n]._hc && this.aNodes[n].pid != this.root.id) {
			this.nodeStatus(status, n, this.aNodes[n]._ls)
			this.aNodes[n]._io = status;
		}
	}
	if (this.config.useCookies) this.updateCookie();
};

// Opens the tree to a specific node
dTree.prototype.openTo = function(nId, bSelect, bFirst) {
	if (!bFirst) {
		for (var n=0; n<this.aNodes.length; n++) {
			if (this.aNodes[n].id == nId) {
				nId=n;
				break;
			}
		}
	}
	var cn=this.aNodes[nId];
	if (cn.pid==this.root.id || !cn._p) return;
	cn._io = true;
	cn._is = bSelect;
	if (this.completed && cn._hc) this.nodeStatus(true, cn._ai, cn._ls);
	if (this.completed && bSelect) this.s(cn._ai);
	else if (bSelect) this._sn=cn._ai;
	this.openTo(cn._p._ai, false, true);
};

// Closes all nodes on the same level as certain node
dTree.prototype.closeLevel = function(node) {
	for (var n=0; n<this.aNodes.length; n++) {
		if (this.aNodes[n].pid == node.pid && this.aNodes[n].id != node.id && this.aNodes[n]._hc) {
			this.nodeStatus(false, n, this.aNodes[n]._ls);
			this.aNodes[n]._io = false;
			this.closeAllChildren(this.aNodes[n]);
		}
	}
}

// Closes all children of a node
dTree.prototype.closeAllChildren = function(node) {
	for (var n=0; n<this.aNodes.length; n++) {
		if (this.aNodes[n].pid == node.id && this.aNodes[n]._hc) {
			if (this.aNodes[n]._io) this.nodeStatus(false, n, this.aNodes[n]._ls);
			this.aNodes[n]._io = false;
			this.closeAllChildren(this.aNodes[n]);		
		}
	}
}

// Change the status of a node(open or closed)
dTree.prototype.nodeStatus = function(status, id, bottom) {
	eDiv	= document.getElementById('d' + this.obj + id);
	eJoin	= document.getElementById('j' + this.obj + id);
	if (this.config.useIcons) {
		eIcon	= document.getElementById('i' + this.obj + id);
		eIcon.src = (status) ? this.aNodes[id].iconOpen : this.aNodes[id].icon;
	}
	eJoin.src = (this.config.useLines)?
	((status)?((bottom)?this.icon.minusBottom:this.icon.minus):((bottom)?this.icon.plusBottom:this.icon.plus)):
	((status)?this.icon.nlMinus:this.icon.nlPlus);
	eDiv.style.display = (status) ? 'block': 'none';
};


// [Cookie] Clears a cookie
dTree.prototype.clearCookie = function() {
	var now = new Date();
	var yesterday = new Date(now.getTime() - 1000 * 60 * 60 * 24);
	this.setCookie('co'+this.obj, 'cookieValue', yesterday);
	this.setCookie('cs'+this.obj, 'cookieValue', yesterday);
};

// [Cookie] Sets value in a cookie
dTree.prototype.setCookie = function(cookieName, cookieValue, expires, path, domain, secure) {
	document.cookie =
		escape(cookieName) + '=' + escape(cookieValue)
		+ (expires ? '; expires=' + expires.toGMTString() : '')
		+ (path ? '; path=' + path : '')
		+ (domain ? '; domain=' + domain : '')
		+ (secure ? '; secure' : '');
};

// [Cookie] Gets a value from a cookie
dTree.prototype.getCookie = function(cookieName) {
	var cookieValue = '';
	var posName = document.cookie.indexOf(escape(cookieName) + '=');
	if (posName != -1) {
		var posValue = posName + (escape(cookieName) + '=').length;
		var endPos = document.cookie.indexOf(';', posValue);
		if (endPos != -1) cookieValue = unescape(document.cookie.substring(posValue, endPos));
		else cookieValue = unescape(document.cookie.substring(posValue));
	}
	return (cookieValue);
};

// [Cookie] Returns ids of open nodes as a string
dTree.prototype.updateCookie = function() {
	var str = '';
	for (var n=0; n<this.aNodes.length; n++) {
		if (this.aNodes[n]._io && this.aNodes[n].pid != this.root.id) {
			if (str) str += '.';
			str += this.aNodes[n].id;
		}
	}
	this.setCookie('co' + this.obj, str);
};

// [Cookie] Checks if a node id is in a cookie
dTree.prototype.isOpen = function(id) {
	var aOpen = this.getCookie('co' + this.obj).split('.');
	for (var n=0; n<aOpen.length; n++)
		if (aOpen[n] == id) return true;
	return false;
};

// If Push and pop is not implemented by the browser
if (!Array.prototype.push) {
	Array.prototype.push = function array_push() {
		for(var i=0;i<arguments.length;i++)
			this[this.length]=arguments[i];
		return this.length;
	}
};
if (!Array.prototype.pop) {
	Array.prototype.pop = function array_pop() {
		lastElement = this[this.length-1];
		this.length = Math.max(this.length-1,0);
		return lastElement;
	}
};".Replace("<%=Img%>",img)
						);
					break;
					//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				case "xmlform.js":
					context.Response.Cache.SetLastModified(new DateTime(2006,8,15));
					context.Response.Write(
						@"function XmlForm_Reset(frm)
{
	var n = frm.length;
	for(var i=0;i<n;i++)
	{
		if(frm.elements[i])
			XmlForm_ResetElementValue(frm.elements[i]);
	}
}

function XmlForm_ResetElementValue(el)
{
	if(el.tagName)
	{
		switch(el.tagName.toLowerCase())
		{
			case ""input"":
				switch(el.type.toLowerCase())
				{
					case ""text"":
					case ""password"":
						    el.value = """";
						break;
					case ""checkbox"":
					case ""radio"":
							el.checked = false;
						break;
				}
			break;
			case ""textarea"":
					el.value = """";
				break;
			case ""select"":
					el.selectedIndex = -1;
				break;
		}
	}
}

function XmlForm_GetElementValue(el)
{
	if(el.tagName)
	{
		switch(el.tagName.toLowerCase())
		{
			case ""input"":
				switch(el.type.toLowerCase())
				{
					case ""text"":
					case ""password"":
					case ""hidden"":
						return el.value;
						break;
					case ""checkbox"":
					case ""radio"":
						return el.checked ? ""1"": ""0"";
						break;
				}
			break;
			case ""textarea"":
			case ""select"":
				return el.value;
			break;
		}
	}
	return """";
}

function XmlForm_SetSelectValue(lst, val)
{
	var n = lst.options.length;
	for(var i=0;i<n;i++)
	{
		if(lst.options[i].value==val)
		{
			lst.selectedIndex = i;
		}
	}
}

function XmlForm_SetRadioValue(rad,val)
{
	var n = rad.length;
	for(var i=0;i<n;i++)
	{
		if(rad[i].value==val)
		{
			rad[i].checked = true;
		}
	}
}

function XmlForm_SetElementValue(el, val)
{
	if(el.tagName)
	{
		switch(el.tagName.toLowerCase())
		{
			case ""input"":
				switch(el.type.toLowerCase())
				{
					case ""text"":
					case ""password"":
					case ""submit"":
					case ""reset"":
					case ""button"":
					case ""hidden"":
						    el.value = val;
						break;
					case ""checkbox"":
					case ""radio"":
							el.checked = (val==1);
						break;
				}
			break;
			case ""textarea"":
					el.innerText = val;
					try
					{
						el.focus();
						el.blur();
					}
					catch(ex)
					{}
				break;
			case ""select"":
					XmlForm_SetSelectValue(el,val);
				break;
		}
	}
	else
	{
		if(el[0])
		if(el[0].type)
		if(el[0].type.toLowerCase()==""radio"")
		{
			XmlForm_SetRadioValue(el, val);
		}
	}
}

///////////////////////////////////////////////////////////////////
//common XmlHttpObject//////////////////
var XmlHttpObject = null;
function XmlForm_CreateRequest(url, func, method)
{
	if(XmlHttpObject!=null)
		return null;
	try
	{
		try
		{
			XmlHttpObject = new ActiveXObject(""Msxml2.XMLHTTP"");
		}
		catch(ex)
		{
			try
			{
				XmlHttpObject = new ActiveXObject(""Microsoft.XMLHTTP"");
			}
			catch(ex)
			{
				XmlHttpObject = new XMLHttpRequest();
			}
		}
		if(XmlHttpObject)
		{
			if(method)
				XmlHttpObject.open(method, url, false);
			else
				XmlHttpObject.open(""POST"", url, false);
			XmlHttpObject.setRequestHeader(""Content-Type"",""text/xml"");
			if(func)
			{
				XmlHttpObject.onreadystatechange = func;
			}
		}
		return XmlHttpObject;
	}	
	catch(ex)
	{
		return null;
	}
}
///////////////////////////////////////////////////////////////////
//contact with the server//////////////////////////////////////////
function XmlForm_OnSubmit(frm, url, func)
{
	try
	{
		var xmlhttp1 = XmlForm_CreateRequest(url, func);
		if(xmlhttp1)
		{
			var n = frm.length;
			var s = ""<?xml version=\""1.0\"" encoding=\""utf-8\"" ?><post><form>""+frm.id+""</form><action>submit</action>"";
			for(var i=0;i<n;i++)
			{
				var e = frm.elements[i];
				if(e.id)
				{
					if(!e.disabled)
					{
						s+= ""<""+e.id+""><![CDATA[""+XmlForm_GetElementValue(e).replace(""]]>"",""] ]>"")+""]]></""+e.id+"">"";
					}
				}
			}
			s+=""</post>"";
			xmlhttp1.send(s);
			frm.ResponseText = xmlhttp1.ResponseText;
			XmlHttpObject = null;
		}
	}
	catch(ex)
	{
		XmlHttpObject = null;
		alert(ex);
	}
}
function XmlForm_FillListGet(frm, lst, url, func)
{
	XmlForm_FillListByMethod(frm, lst, url, func, ""GET"");
}
function XmlForm_FillList(frm, lst, url, func)
{
	XmlForm_FillListByMethod(frm, lst, url, func, ""POST"");
}
function XmlForm_FillListByMethod(frm, lst, url, func, method)
{
	var s = """";
	if(method==""POST"")
		s = ""<?xml version=\""1.0\"" encoding=\""utf-8\"" ?><post><form>""+frm.id+""</form><action>list</action><list>""+lst.id+""</list></post>"";
	else
		s = ""form=""+frm.id+""&action=list&list=""+lst.id;
	var xmlhttp1 = XmlForm_CreateRequest(url, func, method);
	if(xmlhttp1)
	{
		xmlhttp1.send(s);
		var res = xmlhttp1.ResponseXml;
		if(res)
		{
			if(res.xml)
			{
				var doc	= res.documentElement;
				var are	= doc.childNodes;
				var n	= are.length;
				var oGroup = null;
				for(var i=0;i<n;i++)
				{
					var attr = are[i].attributes;
					if(attr)
					{
						try
						{
							if(attr.length==1)
							{
								var opt = document.createElement(""OPTGROUP"");
								opt.label = attr[0].text;
								lst.appendChild(opt);
							}
							else if(attr.length==2)
							{
								var opt = document.createElement(""OPTION"");
								opt.value = attr[0].text;
								opt.text = attr[1].text;
								lst.options[lst.options.length] = opt;
							}
						}
						catch(ex){alert(ex.message);}
					}
				}
			}
		}
		XmlHttpObject = null;
	}
}
function XmlForm_RefreshGet(frm, url, func)
{
	XmlForm_RefreshByMethod(frm, url, func, 'GET');
}
function XmlForm_Refresh(frm, url, func)
{
	XmlForm_RefreshByMethod(frm, url, func, 'POST');
}
function XmlForm_RefreshByMethod(frm, url, func, method)
{
	var xmlhttp1 = XmlForm_CreateRequest(url, func, method);
	if(xmlhttp1)
	{	
		var s = """";
		if(method==""POST"")
			s = ""<?xml version=\""1.0\"" encoding=\""utf-8\"" ?><post><form>""+frm.id+""</form><action>refresh</action></post>"";
		else
			s = ""form=""+frm.id+""&action=refresh"";
		xmlhttp1.send(s);
		var res = xmlhttp1.ResponseXml;
		if(res)
		{
			if(res.xml)
			{
				var doc = res.documentElement;
				var are = doc.childNodes;
				var n = are.length;
				for(var i=0;i<n;i++)
				{
					var el = document.getElementById(are[i].nodeName);
					if(el)
					{
						var val = are[i].text;
						XmlForm_SetElementValue(el, val);
					}
				}
			}
		}
		XmlHttpObject = null;
	}
}"
						);
					break;
					//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			}
		}
	}
}