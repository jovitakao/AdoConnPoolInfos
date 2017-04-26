<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdoPoolInfo.aspx.cs" Inherits="AdoConnPoolInfos.AdoPoolInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <hr />
        Min Pool Size=50;Max Pool Size=200;
        <hr />
        <asp:Button ID="btnCreateConn" runat="server" Text="Create DB Connection ... Open" 
            OnClick="btnCreateConn_OnClick" />
        <br />建立 Connection 並 Open ，Connection 放到 Session 之中
        <hr />
        <asp:Button ID="btnUsingConn" runat="server" Text="Using DB Connection { }" 
            OnClick="btnUsingConn_OnClick" />
        <br />使用 Using ... 建立 Connection 並 Open  ，中間 sleep 0.5 秒
        <hr />
        <asp:Button ID="btnClearAllPools" runat="server" Text="Clear All Pools " 
            OnClick="btnClearAllPools_OnClick" />
        <br />呼叫 Connection 的 ClearAllPools Method
        <hr />
        <asp:Button ID="btnClearPool" runat="server" Text="Clear Conn Pool " 
            OnClick="btnClearPool_OnClick" />
        <br />呼叫 Connection 的 ClearPool Method
        <hr />
        <asp:Button ID="btnCloseConns" runat="server" Text="Close Conns " 
            OnClick="btnCloseConns_OnClick" />
        <br />將放在 Session 中的 Connection , Call Close
        <hr />
        
        <asp:Button ID="btnGetCounterInfo" runat="server" Text="取得 DB Counter 資訊" 
            OnClick="btnGetCounterInfo_OnClick" />
        <br />取得 Performance Counter 相關資訊
        <br />
        <asp:Literal ID="lblCounterInfos" runat="server"></asp:Literal>
        <hr />
    </form>
</body>
</html>
