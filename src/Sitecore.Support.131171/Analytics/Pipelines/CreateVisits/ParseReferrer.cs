namespace Sitecore.Support.Analytics.Pipelines.CreateVisits
{
  using Sitecore.Analytics.Pipelines.CreateVisits;
  using Sitecore.Analytics.Pipelines.ParseReferrer;
  using Sitecore.Analytics.Tracking;
  using Sitecore.Diagnostics;
  using System;
  using System.Web;

  public class ParseReferrer : CreateVisitProcessor
  {
    private void Parse(HttpRequestBase request, CurrentInteraction visit)
    {
      Uri urlReferrer;
      Assert.ArgumentNotNull(request, "request");

      try
      {
        urlReferrer = request.UrlReferrer;
      }
      catch
      {
        Log.Warn(string.Concat(new object[] { "Visit ", visit.InteractionId, ": referrer could not be parsed (", request.Headers["Referer"], ")" }), this);
        urlReferrer = null;
      }

      if (urlReferrer == null)
      {
        visit.Keywords = string.Empty;
        visit.ReferringSite = string.Empty;
        visit.Referrer = string.Empty;
      }
      else if (urlReferrer.Host == HttpContext.Current.Request.Url.Host.ToString())
      {
        visit.Keywords = string.Empty;
        visit.ReferringSite = string.Empty;
        visit.Referrer = string.Empty;
      }
      else
      {
        visit.ReferringSite = urlReferrer.Host;
        visit.Referrer = urlReferrer.ToString();
        ParseReferrerArgs args = new ParseReferrerArgs
        {
          UrlReferrer = urlReferrer,
          Interaction = visit
        };
        ParseReferrerPipeline.Run(args);
        if (visit.Keywords == null)
        {
          visit.Keywords = string.Empty;
        }
      }
    }

    public override void Process(CreateVisitArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      this.Parse(args.Request, args.Interaction);
    }
  }
}