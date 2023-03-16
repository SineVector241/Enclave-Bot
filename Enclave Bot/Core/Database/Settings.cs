using Newtonsoft.Json;

namespace Enclave_Bot.Core.Database
{
    public class Settings
    {
        private const string _dbFolder = "Resources";
        private const string _settingsFile = "settings.json";
        public static Structure Current { get; set; } = new Structure();

        static Settings()
        {
            try
            {
                if (!Directory.Exists(_dbFolder))
                {
                    Directory.CreateDirectory(_dbFolder);
                }
                else if (!File.Exists(_dbFolder + "/" + _settingsFile))
                {
                    string botConfigJson = JsonConvert.SerializeObject(Current, Formatting.Indented);
                    File.WriteAllText(_dbFolder + "/" + _settingsFile, botConfigJson);
                }
                else
                {
                    string settings = File.ReadAllText(_dbFolder + "/" + _settingsFile);
                    var loaded = JsonConvert.DeserializeObject<Structure>(settings);
                    if (loaded != null)
                        Current = loaded;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}]: [ERROR] => An error occured in Settings.cs \nError Info:\n{ex}");
            }
        }

        public static void ReloadSettings()
        {
            if (!Directory.Exists(_dbFolder))
            {
                Directory.CreateDirectory(_dbFolder);
            }
            else if (!File.Exists(_dbFolder + "/" + _settingsFile))
            {
                string settingsJson = JsonConvert.SerializeObject(Current, Formatting.Indented);
                File.WriteAllText(_dbFolder + "/" + _settingsFile, settingsJson);
            }
            else
            {
                string settings = File.ReadAllText(_dbFolder + "/" + _settingsFile);
                var loaded = JsonConvert.DeserializeObject<Structure>(settings);
                if (loaded != null)
                    Current = loaded;
                else
                    throw new Exception("Could not reload settings!");
            }
        }

        public static void UpdateSettings()
        {
            string settingsJson = JsonConvert.SerializeObject(Current, Formatting.Indented);
            File.WriteAllText(_dbFolder + "/" + _settingsFile, settingsJson);
        }

        public class Structure
        {
            public RoleSettings RoleSettings { get; set; } = new RoleSettings();
            public ApplicationSettings ApplicationSettings { get; set; } = new ApplicationSettings();
            public GreetingSettings GreetingSettings { get; set; } = new GreetingSettings();
            public MiscellaneousSettings MiscellaneousSettings { get; set; } = new MiscellaneousSettings();
            public LoggingSettings LoggingSettings { get; set; } = new LoggingSettings();
        }


        public class RoleSettings
        {
            public ulong UnverifiedRole { get; set; }
            public ulong VerifiedRole { get; set; }
            public ulong StaffRole { get; set; }

            public List<ulong> StaffAcceptRoles { get; set; } = new List<ulong>();
        }

        public class ApplicationSettings
        {
            public int KickAfterDenials { get; set; }
            public ulong ApplicationChannel { get; set; }
            public ulong StaffApplicationChannel { get; set; }

            public List<string> ApplicationQuestions { get; set; } = new List<string>();
            public List<string> StaffApplicationQuestions { get; set; } = new List<string>();
        }

        public class GreetingSettings
        {
            public bool SendJoinMessage { get; set; } = true;
            public bool SendLeaveMessage { get; set; } = true;
            public bool ShowProfileInThumbnail { get; set; } = true;
            public bool ShowProfileInPicture { get; set; }
            public bool MentionUser { get; set; } = true;
            public bool AddJoinRole { get; set; } = true;

            public string JoinMessageTitle { get; set; } = "Welcome [user] to [guild]!!";
            public string LeaveMessageTitle { get; set; } = "[user] has left!";
            public string JoinMessage { get; set; } = "Welcome to [guild] **[user]**";
            public string LeaveMessage { get; set; } = "We are sorry to see you go [user]. Be safe on your journeys!";

            public ulong JoinRole { get; set; }
            public ulong GreetingChannel { get; set; }
        }

        public class MiscellaneousSettings
        {
            public ulong BountyChannel { get; set; }
            public ulong SuggestionsChannel { get; set; }
        }

        public class LoggingSettings
        {
            public ulong LoggingChannel { get; set; }
            public bool LoggingEnabled { get; set; }

            //Message Logging Settings
            public bool MessageEditedEnabled { get; set; } = true;
            public bool MessageDeletedEnabled { get; set; } = true;

            //Channel/Category Logging Settings
            public bool ChannelUpdatedEnabled { get; set; } = true;
            public bool ChannelDeletedEnabled { get; set; } = true;
            public bool ChannelCreatedEnabled { get; set; } = true;

            //Voice Logging Settings
            public bool JoinedVoiceEnabled { get; set; } = true;
            public bool LeftVoiceEnabled { get; set; } = true;
            public bool MovedVoiceEnabled { get; set; } = true;

            //User Logging Settings
            public bool UserUpdatedEnabled { get; set; } = true;
            public bool UserJoinedEnabled { get; set; } = true;
            public bool UserLeftEnabled { get; set; } = true;
            public bool UserBannedEnabled { get; set; } = true;
            public bool UserUnbannedEnabled { get; set; } = true;

            //Invite Logging Settings
            public bool InviteCreatedEnabled { get; set; } = true;
            public bool InviteDeletedEnabled { get; set; } = true;

            //Role Logging Settings
            public bool RoleCreatedEnabled { get; set; } = true;
            public bool RoleDeletedEnabled { get; set; } = true;
        }
    }
}
