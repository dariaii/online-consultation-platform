using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace OnlineConsultationPlatform.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class Tests : PageTest
    {
        [Test]
        public async Task Meeting()
        {
            await Page.GotoAsync("http://localhost:5022/login");
            await Page.GetByLabel("Имейл").ClickAsync();
            await Page.GetByLabel("Имейл").FillAsync("dariivanova2001+s@gmail.com");
            await Page.GetByLabel("Имейл").PressAsync("Tab");
            await Page.GetByLabel("Парола").FillAsync("Password_1");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Влизане" }).ClickAsync();

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
            await Page.GetByLabel("Избери преподавател").SelectOptionAsync(new[] { "dcb710f9-9804-49d8-8fe3-abf05fffae4b" });
            await Page.GetByRole(AriaRole.Button, new() { Name = " Изпрати" }).ClickAsync();

            await Page.GetByRole(AriaRole.Link, new() { Name = "" }).ClickAsync();

            await Page.GotoAsync("http://localhost:5022/login");
            await Page.GetByLabel("Имейл").ClickAsync();
            await Page.GetByLabel("Имейл").FillAsync("dariivanova2001+t@gmail.com");
            await Page.GetByLabel("Имейл").PressAsync("Tab");
            await Page.GetByLabel("Парола").FillAsync("Password_1");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Влизане" }).ClickAsync();

            await Page.GotoAsync("http://localhost:5022/meetings/requests");

            await Page.GetByRole(AriaRole.Link, new() { Name = "Покажи детайлите" }).ClickAsync();
            await Page.Locator("form").Filter(new() { HasText = "Дата на срещата: 05 декември" }).GetByRole(AriaRole.Button).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "" }).ClickAsync();
        }
    }
}