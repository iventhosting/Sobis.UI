using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sobis.UI;
using Sobis.UI.Docs;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSbsUI(options =>
{
    options.DefaultToastPosition = SbsToastPosition.TopRight;
    options.DefaultToastShowCloseButton = true;
    options.DefaultTheme = "dark";
});

await builder.Build().RunAsync();
