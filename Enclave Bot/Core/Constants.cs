using System.Diagnostics.CodeAnalysis;
using Discord;

namespace Enclave_Bot.Core
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Constants
    {
        //Other
        public static readonly Color PrimaryColor = Color.DarkOrange;
        public static readonly Color SecondaryColor = Color.LightOrange;
        public static readonly Color ErrorColor = Color.Red;
        
        //Embed Limits
        public const int EMBED_TITLE_CHARACTER_LIMIT = 256;
        public const int EMBED_DESCRIPTION_CHARACTER_LIMIT = 4096;
        public const int EMBED_FIELDS_LIMIT = 10;
        public const int EMBED_FIELD_NAME_CHARACTER_LIMIT = 256;
        public const int EMBED_FIELD_VALUE_CHARACTER_LIMIT = 1024;
        public const int EMBED_FOOTER_EMBED_CHARACTER_LIMIT = 2048;
        public const int EMBED_AUTHOR_NAME_CHARACTER_LIMIT = 256;
        
        //Select Menu Limits
        public const int SELECT_MENU_PLACEHOLDER_CHARACTER_LIMIT = 150;
        public const int SELECT_MENU_OPTIONS_LIMIT = 25;
        public const int SELECT_MENU_OPTION_LABEL_CHARACTER_LIMIT = 100;
        public const int SELECT_MENU_OPTION_VALUE_CHARACTER_LIMIT = 100;
        public const int SELECT_MENU_OPTION_DESCRIPTION_CHARACTER_LIMIT = 100;
        
        //Application Limits
        public const int APPLICATION_TITLE_CHARACTER_LIMIT = 100;
        public const int APPLICATION_QUESTION_CHARACTER_LIMIT = 120;
        public const int APPLICATION_QUESTION_ANSWER_CHARACTER_LIMIT = 200;
        
        //App Component ID's
        public const string APPLICATION_LIST_NAVIGATE = "ALN";
        public const string APPLICATION_LIST_CREATE = "ALC";
        public const string APPLICATION_LIST_EDIT = "ALE";
        public const string APPLICATION_LIST_DELETE = "ALD";

        public const string APPLICATION_EDIT_QUESTIONS = "AEQ";
        public const string APPLICATION_EDIT_NAVIGATE_QUESTIONS = "AENQ";
        public const string APPLICATION_EDIT_CREATE_QUESTION = "AEAQ";
        public const string APPLICATION_EDIT_EDIT_QUESTION = "AEEQ";
        public const string APPLICATION_EDIT_DELETE_QUESTION = "AEDQ";

        public const string APPLICATION_EDIT_ACTIONS = "AEA";
        public const string APPLICATION_EDIT_SET_SUBMISSION_CHANNEL = "AESSC";
        public const string APPLICATION_EDIT_SET_ADD_ROLES = "AEAAR";
        public const string APPLICATION_EDIT_SET_REMOVE_ROLES = "AEARR";
        public const string APPLICATION_EDIT_SET_RETRIES = "AESR";
        public const string APPLICATION_EDIT_SET_FAIL_MODE = "AESFM";

        public const string APPLICATION_MODAL_CREATE = "AMC";
        public const string APPLICATION_MODAL_CREATE_QUESTION = "AMCQ";
        public const string APPLICATION_MODAL_EDIT_QUESTION = "AMEQ";
        public const string APPLICATION_MODAL_SET_RETRIES = "AMSR";
        
        //Server Settings Component ID's
        public const string SERVER_SETTINGS_SET_STAFF_ROLES = "SSSSR";
        
        //Server Actions Component ID's
        public const string SERVER_ACTION_LIST_NAVIGATE = "SALN";
        public const string SERVER_ACTION_LIST_CREATE = "SALC";
        public const string SERVER_ACTION_LIST_EDIT = "SALE";
        public const string SERVER_ACTION_LIST_DELETE = "SALD";
    }
}
