using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CalendarQuickstart
{
	class Program
	{
		// If modifying these scopes, delete your previously saved credentials
		// at ~/.credentials/calendar-dotnet-quickstart.json
		static string[] Scopes = { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents};
		static string ApplicationName = "Google Calendar API .NET Quickstart";
		
		static void Main(string[] args)
		{
			UserCredential credential;

			using (var stream =
				new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
			{
				// The file token.json stores the user's access and refresh tokens, and is created
				// automatically when the authorization flow completes for the first time.
				string credPath = "token.json";
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					Scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;
				Console.WriteLine("Credential file saved to: " + credPath);
			}

			// Create Google Calendar API service.
			var service = new CalendarService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});

			// Define parameters of request.
			EventsResource.ListRequest request = service.Events.List("primary");
			request.TimeMin = DateTime.Now;
			request.ShowDeleted = false;
			request.SingleEvents = true;
			request.MaxResults = 10;
			request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

			// List events.
			Events events = request.Execute();
			Console.WriteLine("Upcoming events:");
			if (events.Items != null && events.Items.Count > 0)
			{
				foreach (var eventItem in events.Items)
				{
					string when = eventItem.Start.DateTime.ToString();
					if (String.IsNullOrEmpty(when))
					{
						when = eventItem.Start.Date;
					}
					Console.WriteLine("{0} ({1})", eventItem.Summary, when);
				}
			}
			else
			{
				Console.WriteLine("No upcoming events found.");
			}
			
			Event newEvent = new Event()
			{
				Summary = "EbayFlexWorkTime",
				Location = "GFC",
				Description = "",
				Start = new EventDateTime()
				{
					DateTime = DateTime.Parse("2019-02-13T09:00:00"),
					TimeZone = "Asia/Seoul",
				},
				End = new EventDateTime()
				{
					DateTime = DateTime.Parse("2019-02-13T16:00:00"),
					TimeZone = "Asia/Seoul",
				},
				//Recurrence = new String[] { "RRULE:FREQ=DAILY;COUNT=2" },
				//Attendees = new EventAttendee[] {
				//	new EventAttendee() { Email = "lpage@example.com" },
				//	new EventAttendee() { Email = "sbrin@example.com" },
				//},
				//Reminders = new Event.RemindersData()
				//{
				//	UseDefault = false,
				//	Overrides = new EventReminder[] {
				//		new EventReminder() { Method = "email", Minutes = 24 * 60 },
				//		new EventReminder() { Method = "sms", Minutes = 10 },
				//	}
				//}
			};

			String calendarId = "primary";
			EventsResource.InsertRequest insertRequest = service.Events.Insert(newEvent, calendarId);
			Event createdEvent = insertRequest.Execute();
			Console.WriteLine("Event created: {0}", createdEvent.HtmlLink);

			Console.Read();
		}
	}
}