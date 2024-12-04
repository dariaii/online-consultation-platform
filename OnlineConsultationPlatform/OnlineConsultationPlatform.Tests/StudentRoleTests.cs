using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace OnlineConsultationPlatform.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class StudentRoleTests : PageTest
    {
        [SetUp]
        public async Task Authenticate()
        {
            await Page.GotoAsync("http://localhost:5022/login");
            await Page.GetByLabel("Имейл").ClickAsync();
            await Page.GetByLabel("Имейл").FillAsync("dariivanova2001+s@gmail.com");
            await Page.GetByLabel("Имейл").PressAsync("Tab");
            await Page.GetByLabel("Парола").FillAsync("Password_1");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Влизане" }).ClickAsync();
        }

        [Test]
        public async Task RequestMeeting()
        {
            await Page.GotoAsync("http://localhost:5022");
            await Page.GetByRole(AriaRole.Link, new() { Name = " Заяви среща" }).ClickAsync();
            await Page.GetByLabel("Избери първа дата").ClickAsync();
            await Page.GetByText("5", new() { Exact = true }).First.ClickAsync();
            await Page.GetByLabel("Избери втора дата").ClickAsync();
            await Page.GetByText("6", new() { Exact = true }).Nth(1).ClickAsync();
            await Page.GetByLabel("Тема на срещата").ClickAsync();
            await Page.GetByLabel("Тема на срещата").FillAsync("test");
            await Page.GetByLabel("Тема на срещата").PressAsync("Tab");
            await Page.GetByLabel("Какъв е основният въпрос или проблем, за който искате да се консултирате?").FillAsync("test");
            await Page.GetByLabel("Какъв е основният въпрос или проблем, за който искате да се консултирате?").PressAsync("Tab");
            await Page.GetByLabel("Какво сте опитали досега, за да разрешите този проблем?").FillAsync("test");
            await Page.GetByLabel("Какво сте опитали досега, за да разрешите този проблем?").PressAsync("Tab");
            await Page.GetByLabel("Избери преподавател").SelectOptionAsync(new[] { "" });
            await Page.GetByRole(AriaRole.Button, new() { Name = " Изпрати" }).ClickAsync();

            await Page.GotoAsync("http://localhost:5022/meetings/confirmed");
            await Page.GetByRole(AriaRole.Link, new() { Name = "Заяви нова среща" }).ClickAsync();
        }
    }
}