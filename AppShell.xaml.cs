namespace CarRepairShop;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute("invoice", typeof(Pages.InvoicePage));

    }
}
