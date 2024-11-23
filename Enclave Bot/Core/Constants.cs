using Discord;

namespace Enclave_Bot.Core
{
    public static class Constants
    {
        //Other
        public static readonly Color PrimaryColor = Color.DarkOrange;
        public static readonly Color SecondaryColor = Color.LightOrange;
        public static readonly Color ErrorColor = Color.Red;

        //Limits
        public const int ListLimit = 15;
        public const int TitleLimit = 60;
        public const int ValueLimit = 120;
        public const int DescriptionLimit = 1000;

        public const int ApplicationsLimit = 20;
        public const int ApplicationQuestionsLimit = 50;
        public const int ApplicationAddRolesLimit = 10;
        public const int ApplicationRemoveRolesLimit = 10;

        //App Component ID's
        public const string ADD_APP_QUESTION = "AAQ";
        public const string ADD_APP_QUESTION_MODAL = "AAQM";
        
        public const string REMOVE_APP_QUESTION = "RAQ";
        public const string REMOVE_APP_QUESTION_SELECTION = "RAQS";
        
        public const string EDIT_APP_QUESTION = "EAQ";
        public const string EDIT_APP_QUESTION_SELECTION = "EAQS";
        public const string EDIT_APP_QUESTION_MODAL = "EAQM";

        public const string APP_QUESTIONS_NEXT_PAGE = "AQNP";
        public const string APP_QUESTIONS_PREVIOUS_PAGE = "AQPP";

        public const string ADD_APP_ADDITION_ROLE = "AAAR";
        public const string ADD_APP_ADDITION_ROLE_SELECTION = "AAARS";
        
        public const string REMOVE_APP_ADDITION_ROLE = "RAAR";
        public const string REMOVE_APP_ADDITION_ROLE_SELECTION = "RAARS";
        
        public const string ADD_APP_REMOVAL_ROLE = "AARR";
        public const string ADD_APP_REMOVAL_ROLE_SELECTION = "AARRS";
        
        public const string REMOVE_APP_REMOVAL_ROLE = "RARR";
        public const string REMOVE_APP_REMOVAL_ROLE_SELECTION = "RARRS";
        
        public const string SET_APP_ACCEPT_MESSAGE = "SAAM";
        public const string SET_APP_ACCEPT_MESSAGE_MODAL = "SAAMM";
        
        public const string SET_APP_SUBMISSION_CHANNEL = "SASC";
        public const string SET_APP_SUBMISSION_CHANNEL_SELECTION = "SASCS";
        
        public const string SET_APP_RETRIES = "SAR";
        public const string SET_APP_RETRIES_MODAL = "SARM";
        
        public const string SET_APP_FAIL_ACTION = "SAFA";
        public const string SET_APP_FAIL_ACTION_SELECTION = "SADAS";
        
        public const string SWITCH_TO_APP_ACTIONS = "STAA";
        public const string SWITCH_TO_APP_QUESTIONS = "STAQ";

        public const string APP_LIST_NEXT_PAGE = "ALNP";
        public const string APP_LIST_PREVIOUS_PAGE = "ALPP";
    }
}
