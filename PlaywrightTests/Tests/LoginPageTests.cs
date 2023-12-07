using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightTests.Setup;

namespace PlaywrightTests;

[TestFixture]
public class Tests : PageTest
{
    private readonly VitalApiWthSpaFactory _vaf;

    public Tests()
    {
        _vaf = new VitalApiWthSpaFactory();
    }

    [SetUp]
    public void Setup()
    {
    }
    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await _vaf.InitializeAsync();
    }

    [Test]
    public async Task Loading_The_Page_Should_Land_on_Login()
    {
        await Page.GotoAsync("http://localhost:4200");

        await Expect(Page).ToHaveTitleAsync(new Regex("Vital"));
        await Expect(Page).ToHaveURLAsync(new Regex("/login"));
    }
    
    //TODO: Figure out how to get the backend to run on localhost:5261, otherwise the calls to the backend will fail
    /*[Test]
    public async Task Login_Should_Redirect_To_Dashboard_With_Correct_Credentials()
    {
        await Page.GotoAsync("http://localhost:4200/login");
        await Expect(Page).ToHaveTitleAsync(new Regex("Vital"));
        
        await Page.GetByPlaceholder("E-mail address").FillAsync("user@application");
        await Page.GetByPlaceholder("Password").FillAsync("P@ssw0rd.+");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
        
        // Add a pause before checking the URL
        await Task.Delay(TimeSpan.FromMilliseconds(6000));
        while (true)
        {
            Task.Delay(TimeSpan.FromMilliseconds(1000));
        }
        
        await Expect(Page).ToHaveURLAsync(new Regex("/dashboard"));
    }
    */
    
    [Test]
    public async Task Dashboard_Unathorized_Redirect_To_Login()
    {
        await Page.GotoAsync("http://localhost:4200/dashboard");
        
        await Expect(Page).ToHaveTitleAsync(new Regex("Vital"));
        await Expect(Page).ToHaveURLAsync(new Regex("/login"));
    }
}
