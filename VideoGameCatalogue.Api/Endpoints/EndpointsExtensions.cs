namespace VideoGameCatalogue.Api.Endpoints
{
    public static class EndpointsExtensions
    {
        public static void MapApiEndpoints(this WebApplication app)
        {
            app.MapVideoGameEndpoints();
            app.MapGenreEndpoints();
            app.MapPlatformEndpoints();
            app.MapCompanyEndpoints();
        }
    }
}
