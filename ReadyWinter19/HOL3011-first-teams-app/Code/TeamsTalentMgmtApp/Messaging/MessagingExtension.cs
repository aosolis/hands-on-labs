using Bogus;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using TeamsTalentMgmtApp.Utils;
using TeamsTalentMgmtApp.DataModel;
using System.Globalization;
using Newtonsoft.Json.Linq;
using AdaptiveCards;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;

namespace TeamsTalentMgmtApp
{
    /// <summary>
    /// Simple class that processes an activity and responds with with set of messaging extension results.
    /// </summary>
    public class MessagingExtension
    {
        private Activity activity;

        private static string ConnectionName = ConfigurationManager.AppSettings["ConnectionName"];

        /// <summary>
        /// Used to generate image index.
        /// </summary>
        private Random random;

        public MessagingExtension(Activity activity)
        {
            this.activity = activity;
            random = new Random();
        }

        /// <summary>
        /// Helper method to generate a the messaging extension response.
        /// 
        /// Note that for this sample, we are returning generated positions for illustration purposes only.
        /// </summary>
        /// <returns></returns>
        public ComposeExtensionResponse CreateQueryResponse()
        {
            ComposeExtensionResponse response = null;

            var query = activity.GetComposeExtensionQueryData();
            JObject data = activity.Value as JObject;

            //Check to make sure a query was actually made:
            if (query.CommandId == null || query.Parameters == null)
            {
                return null;
            }
            else if (query.Parameters.Count > 0)
            {
                // query.Parameters has the parameters sent by client
                var results = new ComposeExtensionResult()
                {
                    AttachmentLayout = "list",
                    Type = "result",
                    Attachments = new List<ComposeExtensionAttachment>(),
                };

                if (query.CommandId == "searchPositions")
                {
                    OpenPositionsDataController controller = new OpenPositionsDataController();
                    IEnumerable<OpenPosition> positions;

                    if (query.Parameters[0].Name == "initialRun")
                    {
                        // Default query => list all
                        positions = controller.ListOpenPositions(10);
                    }
                    else
                    {
                        // Basic search.
                        string title = query.Parameters[0].Value.ToString().ToLower();
                        positions = controller.ListOpenPositions(10).Where(x => x.Title.ToLower().Contains(title));
                    }

                    // Generate cards for the response.
                    foreach (OpenPosition pos in positions)
                    {
                        var card = CardHelper.CreateCardForPosition(pos, true);

                        var composeExtensionAttachment = card.ToAttachment().ToComposeExtensionAttachment();
                        results.Attachments.Add(composeExtensionAttachment);
                    }
                }
                else if (query.CommandId == "searchCandidates")
                {
                    string name = query.Parameters[0].Value.ToString();
                    CandidatesDataController controller = new CandidatesDataController();

                    foreach (Candidate c in controller.GetTopCandidates("ABCD1234"))
                    {
                        c.Name = c.Name.Split(' ')[0] + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
                        var card = CardHelper.CreateSummaryCardForCandidate(c);

                        var composeExtensionAttachment = card.ToAttachment().ToComposeExtensionAttachment(CardHelper.CreatePreviewCardForCandidate(c).ToAttachment());
                        results.Attachments.Add(composeExtensionAttachment);
                    }
                }

                response = new ComposeExtensionResponse()
                {
                    ComposeExtension = results
                };
            }

            return response;
        }

        /// <summary>
        /// Return response to composeExtension/fetchTask invoke
        /// </summary>
        public JObject CreateFetchTaskResponse()
        {
            JObject parameters = activity.Value as JObject;
            if (parameters != null)
            {
                string command = parameters["commandId"].ToString();

                // Fetch dynamic adaptive card for task module.
                if (command == "newPosition")
                {
                    return new TaskModuleHelper().CreateJobPostingTaskModuleResponse();
                }
            }

            // Unknown command or incorrect structure
            return null;
        }

        /// <summary>
        /// Return response for composeExtension/submitAction invoke.
        /// </summary>
        public object CreateSubmitActionResponse()
        {
            JObject parameters = activity.Value as JObject;
            if (parameters != null)
            {
                string command = parameters["commandId"].ToString();
                JObject data = (JObject)parameters["data"];

                if (command == "newPosition")
                {
                    // Create the position and return the result as a card
                    // You can also return a Task module "continue" result if an additional turn is needed

                    OpenPosition pos = new OpenPositionsDataController().CreatePosition(
                        data["jobTitle"].ToString(),
                        int.Parse(data["jobLevel"].ToString()),
                        data["jobLocation"].ToString(),
                        activity.From.Name);
                    var positionCard = CardHelper.CreateCardForPosition(pos, false);
                    return new ComposeExtensionResponse
                    {
                        ComposeExtension = new ComposeExtensionResult
                        {
                            AttachmentLayout = AttachmentLayoutTypes.List,
                            Type = "result",
                            Attachments = new List<ComposeExtensionAttachment>
                            {
                                positionCard.ToAttachment().ToComposeExtensionAttachment(),
                            }
                        }
                    };
                }
            }

            // Unknown command or incorrect structure
            return null;
        }
    }
}