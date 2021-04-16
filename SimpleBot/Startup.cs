// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.12.2

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleBot.Bots;
using SimpleBot.Middleware;
using System.Text.RegularExpressions;


namespace SimpleBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            //Configuring bot middleware
            services.AddBot<PictureBot>(options =>
            {
                var middleware = options.Middleware;
                //Middleware regular expressions
                middleware.Add(new RegExpRecognizerMiddleware()
                    .AddIntent("search", new Regex("search picture(?:s)*(.*)|search pic(?:s)*(.*)", RegexOptions.IgnoreCase))
                    .AddIntent("share", new Regex("share picture(?:s)*(.*)|share pic(?:s)*(.*)", RegexOptions.IgnoreCase))
                    .AddIntent("order", new Regex("order picture(?:s)*(.*)|order print(?:s)*(.*)|order pic(?:s)*(.*)", RegexOptions.IgnoreCase))
                    .AddIntent("help", new Regex("help(.*)", RegexOptions.IgnoreCase)));
            });
            //Configuring bot custo accessors for managing bot state
            services.AddSingleton<PictureBotAccessors>(sp => 
            {
                var conversationState = sp.GetRequiredService<ConversationState>();
                    
                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                return new PictureBotAccessors(conversationState)
                {
                    PictureBotState = conversationState.CreateProperty<PictureBotState>(nameof(PictureBotState)),
                    DialogStateAccessor = conversationState.CreateProperty<DialogState>(nameof(DialogState)),
                };

            });

            services.AddSingleton(sp => {
                var luisapp = new LuisApplication(Configuration["LuisAppId"], Configuration["LuisAppKey"], Configuration["LuisEndPoint"]);
                var recognizeropts = new LuisRecognizerOptionsV3(luisapp) 
                { PredictionOptions = new Microsoft.Bot.Builder.AI.LuisV3.LuisPredictionOptions() { IncludeAllIntents = true } };
                return new LuisRecognizer(recognizeropts);
            });

            //Configuring state
            // Create the User state.
            services.AddSingleton<UserState>(sp => {
                var dataStore = sp.GetRequiredService<IStorage>();
                return new UserState(dataStore);
            });

            // Create the Conversation state.
            services.AddSingleton<ConversationState>(sp =>
            {
                var dataStore = sp.GetRequiredService<IStorage>();
                return new ConversationState(dataStore);
            });

            // Create the IStorage. (this case memory storate)
            services.AddSingleton<IStorage, MemoryStorage>(sp =>
            {
                return new MemoryStorage();
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseBotFramework()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
