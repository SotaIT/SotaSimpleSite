<%@ Application Inherits="Sota.Web.SimpleSite.Global" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // ���, ����������� ��� ������� ����������

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  ���, ����������� ��� ���������� ������ ����������

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // ���, ����������� ��� ������������� ���������������� ������

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // ���, ����������� ��� ������� ������ ������

    }

    void Session_End(object sender, EventArgs e) 
    {
        // ���, ����������� ��� ������� ����������. 
        // ����������. ������� Session_End ���������� ������ � ��� ������, ���� ��� ������ sessionstate
        // ������ �������� InProc � ����� Web.config. ���� ��� ������ ������ ������ �������� StateServer 
        // ��� SQLServer, ������� �� �����������.

    }

	public void Application_BeginRequest(Object sender, EventArgs e)
	{
	
	}
       
</script>