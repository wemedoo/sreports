namespace sReportsV2.Common.Constants
{
    public static class PermissionNames
    {
        // general permissions
        public const string CreateUpdate = "CreateUpdate";
        public const string Delete = "Delete";
        public const string View = "View";

        // administration permissions
        // ...

        // simulator permissions
        // ...

        // designer permisssions
        public const string ShowJson = "ShowJson";
        public const string ChangeState = "ChangeState";
        public const string FindConsensus = "FindConsensus";
        public const string ViewAdministrativeData = "ViewAdministrativeData";
        public const string ViewComments = "ViewComments";
        public const string AddComment = "AddComment";
        public const string ChangeCommentStatus = "ChangeCommentStatus";

        // engine permisssions
        public const string Download = "Download";
        public const string SignFormInstance = "SignFormInstance";
        public const string ChangeFormInstanceState = "ChangeFormInstanceState";

        // thesaurus permissions
        public const string CreateCode = "CreateCode";
        public const string UMLS = "UMLS";

        // patient permissions
        public const string AddEpisodeOfCare = "AddEpisodeOfCare";
        public const string AddEncounter = "Add Encounter";

    }
}
