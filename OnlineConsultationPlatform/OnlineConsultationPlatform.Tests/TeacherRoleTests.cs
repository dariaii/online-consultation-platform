using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace OnlineConsultationPlatform.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class TeacherRoleTests : PageTest
    {
        [SetUp]
        public async Task Authenticate()
        {
            await Page.GotoAsync("http://localhost:5022/login");
            await Page.GetByLabel("Имейл").ClickAsync();
            await Page.GetByLabel("Имейл").FillAsync("dariivanova2001+t@gmail.com");
            await Page.GetByLabel("Имейл").PressAsync("Tab");
            await Page.GetByLabel("Парола").FillAsync("Password_1");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Влизане" }).ClickAsync();
        }

        [Test]
        public async Task AcceptMeeting()
        {
            await Page.GotoAsync("http://localhost:5022/");
            await Page.GetByRole(AriaRole.Link, new() { Name = " Заявки" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Покажи детайлите" }).ClickAsync();
            await Page.Locator("form").Filter(new() { HasText = "Дата на срещата: 06 декември" }).GetByRole(AriaRole.Button).ClickAsync();
        }
    }
}