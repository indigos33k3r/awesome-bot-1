using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using awesome_bot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace awesome_bot.Giphy
{
    public class GiphyCommand : ICommandHandler
    {
        public IEnumerable<string> Commands { get; } = new HashSet<string>
        {
            "gif",
            "giphy"
        };

        public async Task Answer(IDialogContext context, Activity activity, string args)
        {
            var build = GiphyEndPoints.Random.Build(args);

            try
            {
                using (var wc = new WebClient())
                {
                    var rawData = await wc.DownloadStringTaskAsync(build);

                    var giphyResponse = GiphyResponse.FromJson(rawData);

                    var animationCard =
                        new AnimationCard(media: new List<MediaUrl>
                        {
                            new MediaUrl(giphyResponse.Data.ImageOriginalUrl)
                        });

                    var reply = activity.CreateReply();
                    reply.Attachments.Add(animationCard.ToAttachment());

                    await context.PostAsync(reply);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(string.Join(Environment.NewLine,
                    "Error during random gif search",
                    e.Message,
                    e.StackTrace));

                await context.PostAsync("Ops, something went wrong");
                await context.PostAsync("Usage: " + GetHelp());
            }
        }

        public string GetHelp() => string.Join("|", Commands) + " search text";
    }
}