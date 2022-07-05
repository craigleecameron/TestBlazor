using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using TestBlazor.Models.Response;
using UserInputValidationWithRegex.Models;

namespace TestBlazor.Services
{
    public class UrlSearchService
    {
        //perform a regex to get the images
        //add their absolute url to the images collection so they can be displayed
        private List<Uri> PopulateImages(string data, string searchUrl)
        {

            var imagesResponse = new List<Uri>();
            string regexImgSrc = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
            MatchCollection matchesImgSrc = Regex.Matches(data, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match m in matchesImgSrc)
            {

                string href = m.Groups[1].Value;
                if (href.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    imagesResponse.Add(new Uri(href));
                }
                else if (href.StartsWith("//", StringComparison.OrdinalIgnoreCase))
                {
                    imagesResponse.Add(new Uri("https://" + href));
                }
                else
                {
                    imagesResponse.Add(new Uri(searchUrl + "/" + href));
                }
            }
            return imagesResponse;
        }
        //get the words i.e. the text only from the HTML then split then up into individual words
        //If they dont exist in the dictionary add them if not increment their count
        private Dictionary<string, int> PopulateDictionary(string data)
        {
            var dictionaryResponse = new Dictionary<string, int>();

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(data);
            var nodes = htmlDoc.DocumentNode.SelectSingleNode("//body").DescendantsAndSelf();

            foreach (var node in nodes)
            {
                //filter out words that appear in scripts
                if (node.NodeType == HtmlNodeType.Text && node.ParentNode.Name != "script" && node.ParentNode.Name != "noscript")
                {
                    //split on whitespace characters
                    var words = HttpUtility.HtmlDecode(node.InnerText).ToUpper().Split(null);
                    foreach (var singleWord in words)
                    {
                        int count = 0;
                        if (!string.IsNullOrEmpty(singleWord))
                        {
                            if (dictionaryResponse.TryGetValue(singleWord, out count))
                            {
                                if (count > 0)
                                {
                                    dictionaryResponse[singleWord] = dictionaryResponse[singleWord] + 1;

                                }
                                else
                                {
                                    dictionaryResponse.Add(singleWord, count++);
                                }
                            }
                            else
                            {
                                dictionaryResponse.Add(singleWord, 1);
                            }
                        }
                    }
                }
            }
            return dictionaryResponse;
        }

        //populate the url search response object
        public UrlResponseModel GetUrlSearch(string SearchUrl)
        {
            var searchResponse = new UrlResponseModel();

            try
            {
                searchResponse.Images = new List<Uri>();
                searchResponse.WordCount = new Dictionary<string, int>();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SearchUrl);

                //add some security around Web Request
                request.UseDefaultCredentials = true;
                request.Proxy.Credentials = CredentialCache.DefaultCredentials;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                request.Headers.Add("user-agent", "Mozilla");

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string data = "";

               //read the response stream
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;
                    if (response.CharacterSet == null)
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    data = readStream.ReadToEnd();
                    response.Close();
                    readStream.Close();
                }
                else
                {
                    //something happened and was unable to receive a valid response code so return an error
                    searchResponse.ErrorMessage = "Invalid Response code";
                    return searchResponse;
                }

                searchResponse.WordCount = PopulateDictionary(data);

                searchResponse.Images = PopulateImages(data,SearchUrl);
                searchResponse.ShowImages = 5;

            }
            catch(WebException ex)
            {
                searchResponse.ErrorMessage = ex.Message + " This perhaps indicates an incompatibility between accepted certificate versions";

            }
            catch(Exception ex)
            {
                searchResponse.ErrorMessage = "An error has occurred....";

                //While I admit I contemplated returning the actual error message to the front end, I generally feel this is poor security practice, so I opted not to do it.
                //however if you wanted to do so uncomment the line below
                //searchResponse.ErrorMessage = ex.Message;
            }
            return searchResponse;
        }
    }
}
