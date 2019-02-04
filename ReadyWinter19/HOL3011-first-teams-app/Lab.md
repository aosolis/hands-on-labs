# 0-60: Get up and running with your very first Microsoft Teams app

## Overview and Introduction
In this lab, you will be getting your first app up and running in Teams. You should do this lab if:
- You have used Microsoft Teams
- You are interested in building apps or solutions for Teams, or Office 365 in general
- You have built Add-ins for Office or SharePoint

We will start with an app that provides basic bot and tab functionality already hosted on Azure. The lab will then include instructions for adding new functionality through the app’s manifest. For those who want to go further, you can add enhancements to the bot and deploy those changes to your own Azure subscription. 

The sample app we're using here manages the hiring process of candidates for open positions in a team – a Talent Management Application. While it looks good, the app doesn't actually do anything – we want to focus on building a Teams app and loading it into Teams, not creating a full-blown talent management application.

## Prerequisites
### Development tools
Please come with the following tools installed on your laptop:
- [Ngrok](https://ngrok.com/)
    - We recommend signing up for an account. Otherwise your tunnel will time out periodically and you will need to restart ngrok. Each time you restart, the tunnel endpoint changes.
- Visual Studio 2017
    - Install the "ASP.NET and web development" workload
- Microsoft Teams desktop client
- Browser of your choice

### Prepare your Office 365 environment
**IMPORTANT!** You must have an Office 365 tenant to complete this lab. If you don't have one, you can sign up for an Office 365 developer subscription by following the instructions [here](https://docs.microsoft.com/en-us/office/developer-program/office-365-developer-program-get-started).

>**IMPORTANT!** Because of limitations in Teams App Studio, a Microsoft Teams free tenant will **not** work for this lab. 

You will first need to enable side loading for Teams within your Office 365 environment. Open the Admin Center by visiting https://admin.microsoft.com/AdminPortal/Home#/Settings/ServicesAndAddIns from your browser.

Next, select Microsoft Teams. Under the Apps section, scroll down to External Apps and make sure that “Allow sideloading of external apps” is set to On.

![A screenshot of the Apps settings in Microsoft Teams](Images/s1_1.png)

# PART 1: Your first Teams app
In this first part of the Lab you’re going to create a Teams app using an existing base manifest.

The app package **teams-sample-app-package.zip** is located in this folder. Download it to your PC and unzip the contents to any local folder. It will contain three files:
- manifest.json
- color_icon.png
- outline_icon.png

## Step 1: Create your app using Teams App Studio
Open the Teams desktop client and log in with your test tenant's credentials. You can play around with Teams and create your own teams and channels.

To create the app package, Teams has a tool called App Studio&mdash;and it's actually a Teams app itself. Install it from the Teams app store:

![A screenshot of the Microsoft Teams app store](Images/s2_1.png)

Click on the "Store" icon at the lower left, search for "app studio", click on the "Teams App Studio" entry, "Install" button on the consent dialog, and then the bottom "Open" button on the second next dialog:

![A screenshot of Microsoft Teams App Studio](Images/s2_2.png) 

Click on the "Manifest editor" tab and the "Import an existing app" button:

![A screenshot of the Manifest editor tab in App Studio](Images/s2_3.png)

Load the **teams-sample-app-package.zip** file that you previously unzipped and then click on the "Contoso Talent" entry.

![A screenshot of the Manifest editor with the Contoso Talent Entry](Images/s2_3_5.png)

## Step 2: Modify the manifest
 
Most of the information has already been filled out for you. The following screenshots show what information to change:

### App Details
Click on the "Generate" button to generate a unique app id.
![A screenshot of App details in Microsoft Teams](Images/s2_4.png)

### Bots
This app supports a bot that users can interact with through natural language. It supports a number of commands the return rich cards.
You will update the bot information to point to the Azure-hosted instance. In the later section, you can update these entries to point to your local service (through Ngrok).

![A screenshot of updating the bot definition](Images/s2_6.png)

You can also optionally add a new command. The bot we provide supports signing into AAD and the MS Graph. Under Commands, click “Add” and enter in “login” as the command text, with “connect to Office 365” as the help text. Check both **Personal** and **Team** scopes.

![A screenshot of enabling both Personal and Team scopes](Images/s2_7.png) 

### Messaging extensions
Messaging extensions allow users to search your backend through the Teams UI, similar to how users can query Giphys and emojis. These results are returned as rich cards that can be posted into channels.
![A screenshot of Messaging Extensions](Images/s2_8.png)
 
## Step 3: Run the Sample App
You can load and test your sample app directly from App Studio. To do this, click “Test and distribute” under the Finish section in the Manifest editor. Click “Install” and select the team in which you want to test the app.
 
![A screenshot of installing the app from App studio](Images/s3_1.png)
![A screenshot of the App install diaglog](Images/s3_2.png)
 
Next, you'll see the dialog below (of course, the team name will be different). Here, it shows the General Channel:
![A screenshot of app installation](Images/s3_3.png)

You're now free to experiment with your app:
- Use the "Personal App" version via the "…" menu on the left side of Teams
- Talk to the bot in both 1:1 and channel mode. For example, in 1:1 chat, you can ask it questions like "Top candidates ABCD1234" or "New job posting"
- Use actionable messages to schedule interviews
- Create tabs and add them to channels
- Use the messaging extension to find candidate cards to enrich your conversations

Alternatively, you can also upload your custom app through the Teams UI. To do this, first download your app package locally. It will be saved to your PC’s Downloads folder.

![A screenshot of downloading the app](Images/s3_4.png) 

Next, click on the Store icon in the Teams client and then click "Upload a custom app" at the lower left – the file will be located in your Downloads folder and it's called **teams-sample-app-package.zip** (if you are using the Azure version) or **ContosoTalent.zip** if you built it yourself.
 
# PART 2: Deploying and testing locally

First, you’ll need to grab the source code, which is located in the **microsoft-teams-sample-talent-acquisition** folder with this lab.

Open the solution in Visual Studio by double-clicking on the .sln file. Leave it open for now – we’ll come back to it later.

## Step 4: Add a bot to the sample application
Go to the "Bots" section in the manifest editor and click on "Set up" to start.

In the dialog that appears, select "New bot" and name it "Contoso Talent". Under "Scope" select "Personal" and "Team". Leave the rest of the checkboxes unchecked.

![A screenshot of creating a new bot](Images/s4_1.png)

Then click on "Create bot" and wait as App Studio registers a new bot with the Microsoft Bot Framework.

The dialog will close and you'll be back on the "Bots" page. Notice that a new bot has been created. Select the app id, press Ctrl-C to copy it, and paste the id into Notepad. You'll need this ID later!

![A screenshot of the bot id](Images/s4_1_5.png)

Then click on "Generate new password". Select and copy the password that is generated, then paste it into Notepad. Just like the app id, you'll need the password later.

![A screenshot of the bot password](Images/s4_2.png)

The "messaging endpoint" is the HTTP endpoint where your bot will receive messages sent to it. Leave this blank for now&mdash;we'll come back to it later.

## Step 5: Set your App ID and Password and test your bot
Return to Visual Studio and open the Web.config file at the root of the solution. In the TeamsAppId/MicrosoftAppId/MicrosoftAppPassword sections, copy/paste the App ID and Password from Notepad. TeamsAppId doesn't have to be the same as MicrosoftAppId, but it's usually easier if it is, so use the same App ID for both. When you are done, it should look like this:

![A screenshot of Visual Studio showing Web.config from the app project](Images/s5_1.png)

Save the Web.config file.
 
## Step 6: Tunnel localhost to the Internet
Although a Microsoft Teams app is free to access information and APIs inside your firewall, some portions of it, such as the tab URL and bot endpoint, must be accessible from the Internet. The app that you will create today will be running on localhost, so we need a way to make code running on your local machine be accessible from the Internet.
We're using a tool called Ngrok (ngrok.com) for this purpose. 
In the open command prompt (or you can start a new one), type the following command (if ngrok isn't in your PATH, you'll have to prepend its installation directory):

``` ngrok http 3979 -host-header=localhost:3979 ``` 

After a bit, you should see something like this, although the http/https URLs will be different:

![A screenshot of ngrok running](Images/s6_1.png)

Copy the https: URL (not the http: URL) to the clipboard. In the example above, it's https://b26d0449.ngrok.io, but of course yours will be different. **Save the URL: you'll need it shortly.** 

You can minimize the ngrok window; it will keep running in the background.
 
## Step 7: Start your app in Visual Studio
Next, we're going to make a quick check that everything is working properly in Visual Studio. Switch to Visual Studio and click on the Run icon:

![A screenshot of starting the app using the run icon in Visual Studio](Images/s7_1.png)

Visual Studio will build the solution and open http://localhost:3979. But we're interested in what's on the Internet, so paste the URL you saved earlier into a new browser tab. You should see the same page:

 ![A screenshot of the app running in the browser](Images/s7_2.png)

## Step 8: Update your app package and test
Go back to your app in App Studio. In the "Bots" section, in the "Messaging endpoint" field, enter your ngrok URL, followed by `/api/messages`. For example, if your ngrok URL is `https://b26d0449.ngrok.io`, you would enter `https://b26d0449.ngrok.io/api/messages` as the messaging endpoint.

Press Tab to move focus out of the text box, and wait for the green checkmark to indicate that the messaging endpoint was successfully saved.

![A screenshot of the bots settings](Images/s8_1.png)

## Step 9: Run the Sample App
You can load and test your sample app directly from App Studio. To do this, click “Test and distribute” under the Finish section in the Manifest editor. Click “Install” and select the team in which you want to test the app.
 
![A screenshot of installing the app from App studio](Images/s3_1.png)
![A screenshot of the App install dialog](Images/s3_2.png)
 
Next, you'll see the dialog below (of course, the team name will be different). Here, it shows the General Channel:
![A screenshot of app installation](Images/s3_3.png)

Install the

# PART 3: A new messaging extension command

## Step 9: Make some code changes
In this step, you’ll add a new command to the messaging extension to allow searching for candidates in addition to open positions.

In Visual Studio’s solution explorer, under the **Messaging** folder, open **MessagingExtension.cs**
Under the CreateResponse() method, add the following block of code at line 93:
```csharp
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
```
This block of code is what responds to the new command to search for candidates. Rebuild your solution and rerun by hitting F5. In the next step you’ll wire up the command to your app’s manifest.

### Step 10: Add a new command to the manifest
Now you’ll add a new command under the Messaging extensions section of your app in App Studio. Provide the following field values:
- Command Id = searchCandidates
- Title = Candidates
- Description = \<whatever string you want\>
- Parameter
- Name = name
- Title = Name
- Description = \<whatever string you want\>

![A screenshot of adding a new command](Images/s9_1.png)
 
## Test your app
In Visual Studio, hit F5 to restart your local service.

Under Test and distribute, click Install to reload your app.

In Teams, go to any chat or channel conversation. Click on the “…” below the compose box to open the Contoso Talent app – you should now see your new command. Type in any string to initiate the search with your new code changes.
![A screenshot of the running app imn Microsoft Teams](Images/s9_2.png)

# PART 4: Create a new job posting
In this part, we'll add a way to create new job postings using the bot, using an [adaptive card](https://adaptivecards.io) to collect user input.

### Step 11: Handle the "new" command
Messages sent to the bot are handled by the `MessageReceivedAsync` method in `RootDialog.cs`. To keep the example relatively simple, the method simply looks for keywords in the text and acts accordingly. A more sophisticated app might use [LUIS](https://luis.ai) to detect intents and entities.

At line 143 add a handler for the "new" keyword:
```csharp
  else if (cmd.Contains("new"))
  {
      await SendCreateNewJobPostingMessage(context);
  }
```
and the corresponding `SendCreateNewJobPostingMessage` method:
```csharp
  private async Task SendCreateNewJobPostingMessage(IDialogContext context)
  {
      IMessageActivity reply = context.MakeMessage();
      reply.Attachments = new List<Attachment>();

      AdaptiveCard card = CardHelper.CreateCardForNewJobPosting();
      Attachment attachment = new Attachment()
      {
          ContentType = AdaptiveCard.ContentType,
          Content = card
      };

      reply.Attachments.Add(attachment);

      await context.PostAsync(reply);
  }
```
The handler constructs a new adaptive card from the contents of `newjobpostingtemplate.json`. The card contains several input fields that define the job posting, and two buttons: "Create posting" and "Cancel". Both buttons are `Action.Submit` actions that post data back to the bot.

#### Test it!
Run your project again, and test that your bot recognizes the command that we added by sending it the text "new". It should reply with an adaptive card. (You can try clicking on the buttons, but the bot doesn't know what to do with them yet!)

### Step 12: Handle the "Create posting" button
An [Action.Submit](https://adaptivecards.io/explorer/Action.Submit.html) action on an adaptive card takes all the input fields and merges them with the JSON specified in the action's `data` parameter. The result is then sent to the bot in the `value` property of a `message` activity.

In our example, the "Create posting" button is defined as:
```json
  {
    "type": "Action.Submit",
    "title": "Create posting",
    "data": {
      "command": "createPosting"
    }
  }
```
so the bot wil receive a message like:
```json
{
  ...
  "type": "message",
  "value": {
    "command":"createPosting",
    "jobTitle": "Senior PM",
    "jobLevel": "7",
    "jobLocation": "1"
  }
}
```

In the `HandleSubmitAction` of `RootDialog.cs`, at line XXX, add the following block of code:
```csharp
  // Confirmation of job posting message.
  else if (command != null && command.ToString() == "createPosting")
  {
      var pos = new OpenPositionsDataController().CreatePosition(
        parameters["jobTitle"].ToString(),
        int.Parse(parameters["jobLevel"].ToString()),
        Constants.Locations[int.Parse(parameters["jobLocation"].ToString())], activity.From.Name);

      await SendNewPostingConfirmationMessage(context, pos);
  }
```
Add the `SendNewPostingConfirmationMessage` method to `RootDialog.cs`:
```csharp
  private async Task SendNewPostingConfirmationMessage(IDialogContext context, OpenPosition pos)
  {
      IMessageActivity reply = context.MakeMessage();
      reply.Attachments = new List<Attachment>();
      reply.Text = $"Your position has been created.";

      ThumbnailCard positionCard = CardHelper.CreateCardForPosition(pos, false);
      reply.Attachments.Add(positionCard.ToAttachment());

      await context.PostAsync(reply);
  }
```

#### Test it!
1. Build and run your project.
2. Send "new job posting" to your bot. It should reply with an adaptive card.
3. Enter values for the title, level, and location, then press the "Create posting" button.
4. The bot should reply with the text `"Your new position has been created."` and a card for the new position.

### Step 12: Prompt for a job description file
!TODO: Motivate this step

Modify the `SendNewPostingConfirmationMessage` so that the response to the "Create posting" button prompts for a job description:
```csharp
  reply.Text = $"Your position has been created. Please also upload the job description now.";
```

When the user send the bot a message with a file attached, the bot receives a `application/vnd.microsoft.teams.file.download.info` attachment that has information about the file. Our sample app will just check the file type and respond with success to PDF and DOCX submissions. In practice you would get the file contents by making a GET request to the location in `downloadUrl`.
```json
{
  "contentType": "application/vnd.microsoft.teams.file.download.info",
  "contentUrl": "https://contoso.sharepoint.com/personal/johnadams_contoso_com/Documents/Applications/file_example.txt",
  "name": "file_example.txt",
  "content": {
    "downloadUrl" : "https://download.link",
    "uniqueId": "1150D938-8870-4044-9F2C-5BBDEBA70C9D",
    "fileType": "txt"
  }
}
```

Add code to `HandleSubmitAction` to respond to file attachments:"
```csharp
  else if (activity.Attachments.Any())
  {
      // Handle file upload scenario.
      if (activity.Attachments[0].ContentType == "application/vnd.microsoft.teams.file.download.info")
      {
          string fileName = activity.Attachments[0].Name;
          string fileType = (activity.Attachments[0].Content as JObject)["fileType"].ToString().ToLower();

          if (fileType.Contains("docx") || fileType.Contains("pdf"))
          {
              await context.PostAsync($"Job posting successfully uploaded: {fileName}");
          } else
          {
              await context.PostAsync("Invalid file type received. Please upload a PDF or Word document");
          }
      }
  }
```

#### Test it!
1. Build and run your project.
2. Send "new job posting" to your bot and create a new job posting.
4. The bot should reply with the text `"Your new position has been created. Please also upload the job description now."` and a card for the new position.
5. Send the bot a PDF or DOCX file. There are some sample description files in the `Job Descriptions` folder.
6. The bot should respond with `"Job posting successfully uploaded: ` and the name of the file that you uploaded.

If that works, repeat steps 2-5, but this time send it a different kind of file--perhaps try an Excel spreadsheet? The bot should detect the incorrect file type, and say `"Invalid file type received. Please upload a PDF or Word document"`.