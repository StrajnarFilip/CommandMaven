namespace DependencyMaven;
using System.Net.Http;
using HtmlAgilityPack;
using System.Web;

public class CentralRequest
{
    public const string mavenCentralUrl = "https://mvnrepository.com";
    public async static Task GetBestResult(string query)
    {
        var requestString = $"{mavenCentralUrl}/search?q={HttpUtility.UrlEncode(query)}&sort=popular";
        System.Console.WriteLine(requestString);
        var httpResponse = await new HttpClient().GetAsync(requestString);
        var contentString = await httpResponse.Content.ReadAsStringAsync();
        if (contentString is null)
        {
            System.Console.WriteLine("Content is null");
            return;
        }

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(contentString);
        var htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//body//div[1]//div[3]//div[2]");
        if (htmlBody is null)
        {
            System.Console.WriteLine("Node is null");
            return;
        }
        System.Console.WriteLine(htmlBody.InnerHtml);

    }
}