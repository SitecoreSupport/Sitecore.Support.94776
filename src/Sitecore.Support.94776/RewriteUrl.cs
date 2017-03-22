using Sitecore.Diagnostics;
using Sitecore.ItemWebApi;
using Sitecore.Support.ItemWebApi.Serialization;
using Sitecore.ItemWebApi.Web;
using Sitecore.Pipelines.PreprocessRequest;
using Sitecore.Text;
using Sitecore.Web;
using System;
using System.Web;

namespace Sitecore.Support.ItemWebApi.Pipelines.PreprocessRequest
{
    public class RewriteUrl : Sitecore.ItemWebApi.Pipelines.PreprocessRequest.RewriteUrl
    {
        private static int GetVersion(string path)
        {
            int num;
            Assert.ArgumentNotNull(path, "path");
            string str = path.TrimStart(new char[] { '/' }).Split(new char[] { '/' })[2];
            Assert.IsTrue(str.StartsWith("v"), "Version token is wrong.");
            Assert.IsTrue(int.TryParse(str.Replace("v", string.Empty), out num), "Version not recognized.");
            return num;
        }

        public override void Process(PreprocessRequestArgs arguments)
        {
            Assert.ArgumentNotNull(arguments, "arguments");
            try
            {
                string localPath = arguments.Context.Request.Url.LocalPath;
                if (localPath.StartsWith("/-/item/"))
                {
                    Sitecore.ItemWebApi.Context context = new Sitecore.ItemWebApi.Context
                    {
                        Serializer = new JsonSerializer(),
                        Version = GetVersion(localPath),
                        ResponseOutputBuilder = new ResponseOutputBuilder()
                    };
                    Sitecore.ItemWebApi.Context.Current = context;
                    Rewrite(arguments.Context);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private static void Rewrite(HttpContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            Uri url = context.Request.Url;
            string[] sourceArray = url.LocalPath.TrimStart(new char[] { '/' }).Split(new char[] { '/' });
            int length = sourceArray.Length - 3;
            string[] destinationArray = new string[length];
            Array.Copy(sourceArray, 3, destinationArray, 0, length);
            string str2 = $"/{string.Join("/", destinationArray)}";
            string str3 = url.Query.TrimStart(new char[] { '?' });
            WebUtil.RewriteUrl(new UrlString
            {
                Path = str2,
                Query = str3
            }.ToString());
        }
    }
}
