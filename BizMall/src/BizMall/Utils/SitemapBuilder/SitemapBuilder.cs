using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using SimpleMvcSitemap;

namespace BizMall.Utils.SitemapBuilder
{
    public class SitemapBuilder
    {
        private readonly XNamespace NS = "http://www.sitemaps.org/schemas/sitemap/0.9";

        private List<SitemapNode> _urls;
        private List<SitemapNode> _statdata;

        public SitemapBuilder()
        {
            _urls = new List<SitemapNode>();
            _statdata = new List<SitemapNode>();
        }

        public void AddUrl(string url, DateTime? modified = null, ChangeFrequency? changeFrequency = null, decimal? priority = null)
        {
            _urls.Add(new SitemapNode(url)
            {
                LastModificationDate = modified,
                ChangeFrequency = changeFrequency,
                Priority = priority,
            });
        }

        public void AddStatisticNode(string data)
        {
            _statdata.Add( new SitemapNode(data));
        }

        public XDocument GetSitemap()
        {
            var sitemap = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),                    
                    new XElement(
                        NS + "urlset",
                        from item in _statdata select CreateStatisticsNodeElement(item),
                        from item in _urls select CreateSitemapNodeElement(item)
                    )
                );

            //sitemap.Add(new XElement("statistics", from item in _urls where item.Url.Contains("stcs_") select CreateStatisticsNodeElement(item)));

            return sitemap;
        }

        private XElement CreateStatisticsNodeElement(SitemapNode url)
        {
            return new XElement(NS + "statdata", url.Url);
        }

        private XElement CreateSitemapNodeElement(SitemapNode url)
        {
            XElement itemElement = new XElement(NS + "url", new XElement(NS + "loc", url.Url));

            if (url.LastModificationDate.HasValue)
            {
                itemElement.Add(new XElement(NS + "lastmod", url.LastModificationDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.f") + "+00:00"));
            }

            if (url.ChangeFrequency.HasValue)
            {
                itemElement.Add(new XElement(NS + "changefreq", url.ChangeFrequency.Value.ToString()));
            }

            if (url.Priority.HasValue)
            {
                itemElement.Add(new XElement(NS + "priority", url.Priority.Value.ToString("N1")));
            }

            return itemElement;
        }
    }
}
