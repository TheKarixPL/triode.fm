# triode.fm (WIP)
triode.fm is simple self-hosted Discord bot that allows play music locally, from youtube or http link
on your Discord server.

## License
This program is licensed under GNU GPL 3.0 license

## Building
### Requirements
<ul>
    <li>.NET Core 6 SDK</li>
    <li>ASP.NET Core 6 SDK</li>
    <li>PostgreSQL v. 13 or later</li>
    <li>Discord token which can be optained via [Discord Developer Portal](https://discord.com/developers/docs/intro)</li>
</ul>

### Building
Compile project with `dotnet build`

### Configuration
In order to configure bot, you need to create file `secret.json` in root directory of program and type into it <code>{<br>
&nbsp;"Bot": {<br>
&nbsp;&nbsp;"Token": "YOUR_DISCORD_TOKEN"<br>
&nbsp;},<br>
&nbsp;"Database": {<br>
&nbsp;&nbsp;"ConnectionString": "YOUR_NPGSQL_CONNECTION_STRING"<br>
&nbsp;}<br>
}</code>