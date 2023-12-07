using PlaywrightSharp;
using PlaywrightTests.Setup;

namespace PlaywrightTests;


public class LoginPageTests : IClassFixture<VitalApiPlaywrightFactory>
{
    private readonly VitalApiPlaywrightFactory _vaf;

    public LoginPageTests(VitalApiPlaywrightFactory vaf)
    {
        _vaf = vaf;
    }

    [Fact]
    public async Task Loading_The_Page_Should_Land_on_Login()
    {
        var page = await _vaf.Browser.NewPageAsync();
        await page.GoToAsync(_vaf.BaseUrl);

        GetTile(page).Should().Be("Vital");
        page.Url.Should().Be(_vaf.BaseUrl + "/login");
    }
    
    //TODO: Figure out how to get the backend to run on localhost:5261, otherwise the calls to the backend will fail
    [Fact]
    public async Task Login_Should_Redirect_To_Dashboard_With_Correct_Credentials()
    {
        var page = await _vaf.Browser.NewPageAsync();
        await page.GoToAsync(_vaf.BaseUrl + "/login");
        GetTile(page).Should().Be("Vital");
        
        //await page.FillAsync(GetByPlaceholder(page, "E-mail address"), "user@application");
        await page.FillAsync("input[placeholder='Password']", "P@ssw0rd.+");
        await page.FillAsync("input[placeholder='E-mail address']", "user@application");    
        await page.ClickAsync("button:has-text('Login')");

        await Task.Delay(60000);
        
        page.Url.Should().Be(_vaf.BaseUrl + "/dashboard");
    }

    
    [Fact]
    public async Task Dashboard_Unathorized_Redirect_To_Login()
    {
        var page = await _vaf.Browser.NewPageAsync();
        await page.GoToAsync(_vaf.BaseUrl + "/dashboard");
        
        GetTile(page).Should().Be("Vital");
        page.Url.Should().Be(_vaf.BaseUrl + "/login");
    }
    
    
    private string GetTile(IPage page)
    {
        return page.GetTitleAsync().Result;
    }
    
    private IElementHandle GetByPlaceholder(IPage page, string placeholder)
    {
        return page.QuerySelectorAsync($"[placeholder=\"{placeholder}\"]").Result;
    }
}
