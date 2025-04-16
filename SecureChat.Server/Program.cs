using Microsoft.AspNetCore.ResponseCompression;
using SecureChat.Server.Hubs;
using SecureChat.Server.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("default",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
});
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IRoomRepository, RoomRepository>();
var app = builder.Build();
app.UseCors("default");
app.UseResponseCompression();

app.MapHub<ChatHub>("/chathub");
app.Run();
