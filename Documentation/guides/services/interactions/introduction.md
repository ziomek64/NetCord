# Introduction

## [Hosting](#tab/hosting)

With hosting, adding interactions is very easy. Use @NetCord.Hosting.Services.Interactions.InteractionServiceHostBuilderExtensions.UseInteractionService``2(Microsoft.Extensions.Hosting.IHostBuilder) to add an interaction service to your host builder. Then, use @NetCord.Hosting.Services.Interactions.InteractionServiceHostExtensions.AddInteraction* to add an interaction using the ASP.NET Core minimal APIs way and/or use @NetCord.Hosting.Services.ServicesHostExtensions.AddModules(Microsoft.Extensions.Hosting.IHost,System.Reflection.Assembly) to add modules from an assembly. You also need to use @NetCord.Hosting.Gateway.GatewayEventHandlerHostExtensions.UseGatewayEventHandlers(Microsoft.Extensions.Hosting.IHost) to bind the service event handlers.
[!code-cs[Program.cs](IntroductionHosting/Program.cs?highlight=11-17,20-28)]

## [Without Hosting](#tab/without-hosting)

First, add the following line to the using section.
[!code-cs[Program.cs](Introduction/Program.cs#L4)]

Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it. In this example, we will use @NetCord.Services.Interactions.ButtonInteractionContext.
[!code-cs[Program.cs](Introduction/Program.cs#L11-L12)]

We can add an interaction handler now.
[!code-cs[Program.cs](Introduction/Program.cs#L14-L33)]

### The Final Product

#### Program.cs
[!code-cs[Program.cs](Introduction/Program.cs)]

***

### Example Modules

#### Button Module
[!code-cs[ButtonModule.cs](Introduction/ButtonModule.cs)]

#### String Menu Module
[!code-cs[StringMenuModule.cs](Introduction/StringMenuModule.cs)]

#### User Menu Module
[!code-cs[UserMenuModule.cs](Introduction/UserMenuModule.cs)]

#### Role Menu Module
[!code-cs[RoleMenuModule.cs](Introduction/RoleMenuModule.cs)]

#### Mentionable Menu Module
[!code-cs[MentionableMenuModule.cs](Introduction/MentionableMenuModule.cs)]

#### Channel Menu Module
[!code-cs[ChannelMenuModule.cs](Introduction/ChannelMenuModule.cs)]

#### Modal Submit Module
[!code-cs[ModalSubmitModule.cs](Introduction/ModalSubmitModule.cs)]