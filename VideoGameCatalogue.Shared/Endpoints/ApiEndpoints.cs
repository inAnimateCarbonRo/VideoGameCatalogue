using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace VideoGameCatalogue.Shared.Endpoints
{
    public class ApiEndpoints
    {
        private const string ApiBase = "api";
   

        public static class VideoGameEndpoints
        {
            private const string Base = $"{ApiBase}/videogame";

            public const string Get = $"{Base}/{{id}}";
            public const string GetAll = Base; 
            public const string GetAllIncludingDeleted = $"{Base}/all-including-deleted";
            public const string Create = Base;
            public const string Update = $"{Base}/{{id}}";
            public const string Delete = $"{Base}/{{id}}";
            public const string Restore = $"{Base}/restore/{{id}}";
            public const string FullDelete = $"{Base}/fulldelete/{{id}}";

        }

        public static class GenreEndpoints
        {
            private const string Base = $"{ApiBase}/genre";

            public const string Get = $"{Base}/{{id}}";
            public const string GetAll = Base;
            public const string GetAllIncludingDeleted = $"{Base}/all-including-deleted";
            public const string Create = Base;
            public const string Update = $"{Base}/{{id}}";
            public const string Delete = $"{Base}/{{id}}";
            public const string Restore = $"{Base}/restore/{{id}}";
            public const string FullDelete = $"{Base}/fulldelete/{{id}}";

        }
        public static class PlatformEndpoints
        {
            private const string Base = $"{ApiBase}/platform";

            public const string Get = $"{Base}/{{id}}";
            public const string GetAll = Base;
            public const string GetAllIncludingDeleted = $"{Base}/all-including-deleted";
            public const string Create = Base;
            public const string Update = $"{Base}/{{id}}";
            public const string Delete = $"{Base}/{{id}}";
            public const string Restore = $"{Base}/restore/{{id}}";
            public const string FullDelete = $"{Base}/fulldelete/{{id}}";
        }

        public static class CompanyEndpoints
        {
            private const string Base = $"{ApiBase}/company";

            public const string Get = $"{Base}/{{id}}";
            public const string GetAll = Base;
            public const string GetAllIncludingDeleted = $"{Base}/all-including-deleted";
            public const string Create = Base;
            public const string Update = $"{Base}/{{id}}";
            public const string Delete = $"{Base}/{{id}}";
            public const string Restore = $"{Base}/restore/{{id}}";
            public const string FullDelete = $"{Base}/fulldelete/{{id}}";
        }

    }
}
